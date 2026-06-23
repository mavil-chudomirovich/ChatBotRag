using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagChatbot.Business.Interfaces;
using RagChatbot.DataAccess.EntityModels;
using RagChatbot.DataAccess.Interfaces;
using System.Text.Json;
using System.Security.Claims;
using System.Threading;

namespace RagChatbot.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ISubjectService _subjectService;
        private readonly IVectorSearchService _vectorSearchService;
        private readonly IAiService _aiService;
        private readonly IDocumentService _documentService;
        private readonly IAppUserRepository _userRepository;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IChatService chatService,
            ISubjectService subjectService,
            IVectorSearchService vectorSearchService,
            IAiService aiService,
            IDocumentService documentService,
            IAppUserRepository userRepository,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _subjectService = subjectService;
            _vectorSearchService = vectorSearchService;
            _aiService = aiService;
            _documentService = documentService;
            _userRepository = userRepository;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }

        private bool IsSimpleGreeting(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return true;
            var cleanMsg = msg.Trim().ToLower().Replace("?", "").Replace(".", "").Replace("!", "");

            var greetingKeywords = new HashSet<string>
            {
                "chào", "chào bạn", "hello", "hi", "hey", "alo", "chào bot", "chào ad", "xin chào", "hi ad", "hi bot"
            };

            return greetingKeywords.Contains(cleanMsg);
        }

        [HttpGet("LoadSubjectHistory")]
        public async Task<IActionResult> LoadSubjectHistory(int subjectId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var subject = await _subjectService.GetByIdAsync(subjectId);
                if (subject == null)
                {
                    return BadRequest("Môn học không tồn tại.");
                }

                var session = await _chatService.GetSessionBySubjectIdAsync(subjectId, userId);

                if (session != null)
                {
                    var messagesList = await _chatService.GetSessionMessagesAsync(session.Id);
                    var messages = messagesList.OrderBy(m => m.Timestamp).Select(m => new
                    {
                        role = m.Role,
                        content = m.Content,
                        citations = m.Citations
                    }).ToList();

                    return Ok(new { SessionId = session.Id.ToString(), Messages = messages });
                }
                
                return Ok(new { SessionId = "", Messages = new List<object>() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading history");
                return StatusCode(500, "Không thể tải lịch sử chat.");
            }
        }

        public class SendMessageRequest
        {
            public string? SessionIdStr { get; set; }
            public int SubjectId { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<int>? DocumentIds { get; set; }
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                var subject = await _subjectService.GetByIdAsync(request.SubjectId);
                if (subject == null)
                {
                    return BadRequest(new { error = "Môn học không tồn tại." });
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    var today = DateTime.UtcNow.Date;

                    if (user.LastQueryDate.Date < today)
                    {
                        user.DailyQueryCount = 0;
                        user.LastQueryDate = DateTime.UtcNow;
                    }

                    if (user.LastActiveDate.Date < today)
                    {
                        user.TodayChatCount = 0;
                        user.LastActiveDate = today;
                    }

                    if (user.Role == "Student" && user.Subscription == AppUser.SubscriptionType.Free)
                    {
                        if (user.TodayChatCount >= 20)
                        {
                            return BadRequest(new { error = "Bạn đã hết 20 lượt hỏi miễn phí của ngày hôm nay. Hãy nâng cấp gói Premium để chat không giới hạn nhé! 👑" });
                        }
                        user.TodayChatCount++;
                    }

                    bool isExemptFrom50Limit = user.Role == "Admin" || (user.Role == "Student" && user.Subscription == AppUser.SubscriptionType.Premium);
                    if (!isExemptFrom50Limit)
                    {
                        if (user.DailyQueryCount >= 50)
                        {
                            return BadRequest(new { error = "Bạn đã vượt quá giới hạn 50 câu hỏi/ngày. Vui lòng quay lại vào ngày mai." });
                        }
                    }

                    user.DailyQueryCount++;
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                }

                Guid sessionId;
                if (!Guid.TryParse(request.SessionIdStr, out sessionId))
                {
                    var title = request.Message.Length > 50 ? request.Message.Substring(0, 50) + "..." : request.Message;
                    var session = await _chatService.CreateSessionAsync(request.SubjectId, userId, title);
                    sessionId = session.Id;
                }

                var userMessage = new RagChatbot.Business.DTOs.CreateChatMessageDto
                {
                    SessionId = sessionId,
                    Role = "user",
                    Content = request.Message
                };
                var savedUserMsg = await _chatService.AddMessageAsync(userMessage);

                var history = await _chatService.GetRecentSessionMessagesAsync(sessionId, 10, savedUserMsg.Id);

                var allDocs = await _documentService.GetBySubjectIdAsync(request.SubjectId);
                bool hasActiveDocs = allDocs.Any(d => d.Status == "Indexed" && d.IsActive);

                if (!hasActiveDocs)
                {
                    var noDocFallback = "Hiện tại môn học chưa có tài liệu học tập được kích hoạt trên hệ thống. Vui lòng quay lại sau hoặc liên hệ Bộ môn phụ trách để biết thêm chi tiết.";
                    var assistantMsgFallback = new RagChatbot.Business.DTOs.CreateChatMessageDto
                    {
                        SessionId = sessionId,
                        Role = "assistant",
                        Content = noDocFallback,
                        Citations = "[]"
                    };
                    await _chatService.AddMessageAsync(assistantMsgFallback);
                    return Ok(new { response = noDocFallback, citations = "[]", sessionId = sessionId.ToString() });
                }

                string standaloneQuery = request.Message;
                bool isGreeting = IsSimpleGreeting(request.Message);
                List<RagChatbot.Business.DTOs.DocumentChunkDto> similarChunks = new List<RagChatbot.Business.DTOs.DocumentChunkDto>();

                if (!isGreeting)
                {
                    standaloneQuery = await _aiService.RewriteQueryAsync(request.Message, history);
                    var questionEmbedding = await _aiService.GenerateEmbeddingAsync(standaloneQuery);

                    var docIds = request.DocumentIds?.Where(id => id > 0).ToList();
                    similarChunks = await _vectorSearchService.SearchSimilarChunksAsync(request.SubjectId, standaloneQuery, questionEmbedding, topK: 15, documentIds: docIds);
                }
                else
                {
                    var greetingResponse = "Chào bạn! Mình là trợ lý thông minh. Mình có thể giúp gì cho bạn hôm nay?";
                    var assistantMsgGreeting = new RagChatbot.Business.DTOs.CreateChatMessageDto
                    {
                        SessionId = sessionId,
                        Role = "assistant",
                        Content = greetingResponse,
                        Citations = "[]"
                    };
                    await _chatService.AddMessageAsync(assistantMsgGreeting);
                    return Ok(new { response = greetingResponse, citations = "[]", sessionId = sessionId.ToString() });
                }

                if (!isGreeting && similarChunks.Count == 0)
                {
                    var fallbackMessage = "Hệ thống không tìm thấy thông tin trong các tài liệu đã chọn.";
                    var assistantMsg = new RagChatbot.Business.DTOs.CreateChatMessageDto
                    {
                        SessionId = sessionId,
                        Role = "assistant",
                        Content = fallbackMessage,
                        Citations = "[]"
                    };
                    await _chatService.AddMessageAsync(assistantMsg);
                    return Ok(new { response = fallbackMessage, citations = "[]", sessionId = sessionId.ToString() });
                }

                var contextBuilder = new System.Text.StringBuilder();
                var citationsList = new List<object>();
                var seenCitations = new HashSet<string>();

                foreach (var chunk in similarChunks)
                {
                    var dispName = string.IsNullOrWhiteSpace(chunk.Document?.DisplayName) ? chunk.Document?.FileName : chunk.Document?.DisplayName;
                    contextBuilder.AppendLine($"[{dispName}] - Trang {chunk.PageNumber}");
                    contextBuilder.AppendLine(chunk.Content);
                    contextBuilder.AppendLine("---");

                    var citationKey = $"{dispName}_{chunk.PageNumber}";
                    if (!seenCitations.Contains(citationKey))
                    {
                        seenCitations.Add(citationKey);
                        citationsList.Add(new
                        {
                            FileName = dispName,
                            Page = chunk.PageNumber,
                            ContentSnippet = chunk.Content.Length > 100 ? chunk.Content.Substring(0, 100) + "..." : chunk.Content
                        });
                    }
                }

                var contextString = contextBuilder.ToString();
                var citationsJson = JsonSerializer.Serialize(citationsList);

                var systemPrompt = $@"Bạn là trợ lý học tập thông minh. Bạn có thể trò chuyện, chào hỏi thân thiện.
Tuy nhiên, đối với các câu hỏi tìm kiếm thông tin, bạn phải tuân thủ nghiêm ngặt GROUNDING_RULE: Chỉ sử dụng thông tin từ [NGỮ CẢNH TÀI LIỆU] dưới đây.
Tuyệt đối không sử dụng kiến thức bên ngoài. Nếu không có thông tin trong ngữ cảnh, hãy trả lời: 'Hệ thống không tìm thấy thông tin trong các tài liệu đã chọn'.

[NGỮ CẢNH TÀI LIỆU]:
{contextString}
";

                var finalResponseStr = await _aiService.GetChatResponseAsync(systemPrompt, standaloneQuery, history, cancellationToken);

                if (finalResponseStr.Contains("Hệ thống không tìm thấy thông tin trong các tài liệu đã chọn"))
                {
                    citationsJson = "[]";
                }

                var assistantMessage = new RagChatbot.Business.DTOs.CreateChatMessageDto
                {
                    SessionId = sessionId,
                    Role = "assistant",
                    Content = finalResponseStr,
                    Citations = citationsJson
                };
                await _chatService.AddMessageAsync(assistantMessage);

                return Ok(new { response = finalResponseStr, citations = citationsJson, sessionId = sessionId.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new { error = "An error occurred while processing your message." });
            }
        }
    }
}
