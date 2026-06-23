using System;

namespace RagChatbot.DataAccess.EntityModels
{
    public class HodTerm
    {
        public int Id { get; set; }
        
        public int AppUserId { get; set; }
        public AppUser User { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public DateTime StartAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndAt { get; set; }
    }
}
