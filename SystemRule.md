# ĐẶC TẢ QUY TẮC & PHÂN QUYỀN HỆ THỐNG
## SYSTEM RULES & RBAC — EDTECH AI CHATBOT RAG

### 1. TỔNG QUAN HỆ THỐNG
* Hệ thống AI Chatbot phục vụ truy xuất kiến thức (RAG) được thiết kế theo mô hình quản lý học liệu mở và công khai (tương tự hệ thống FLM - Framework for Learning Management) của FPT. 
* Toàn bộ danh mục môn học, syllabus, slide bài giảng chuẩn và tài liệu tham khảo được công khai hoàn toàn cho toàn bộ Học sinh trong nhà trường nhằm thúc đẩy tinh thần tự học, tra cứu chéo và chuẩn bị bài trước kỳ học.
* Dữ liệu cấu trúc theo phân cấp quản lý: Hệ thống → Bộ môn → Môn học → Kho Học liệu Chuẩn (Active).
* Hệ thống áp dụng Multi-tenant / Role-Based Access Control (RBAC) ở lớp quản trị (Admin, Head of Department, Lecturer) nhằm phân định rõ trách nhiệm đóng góp dữ liệu, nhưng áp dụng cơ chế mở hoàn toàn đối với tầng end-user (Học sinh) để tối ưu khả năng tiếp cận tri thức.

### 2. MA TRẬN PHÂN QUYỀN (PERMISSION MATRIX)

| Tính năng / Quyền hạn | Admin | Head of Department | Học sinh |
| :--- | :---: | :---: | :---: |
| Quản lý tài khoản toàn hệ thống | ✔ | – | – |
| Tạo và quản lý Bộ môn | ✔ | – | – |
| Quản lý Trưởng bộ môn (HOD) | ✔ | – | – |
| Quản lý Môn học (Tạo/Sửa/Xóa) | ✔ | ✔ (Thuộc bộ môn) | – |
| Thêm tài liệu học tập vào môn | ✔ | ✔ (Thuộc bộ môn) | – |
| Xóa tài liệu học tập khỏi môn | ✔ | ✔ (Thuộc bộ môn) | – |
| Bật / Tắt trạng thái tài liệu | ✔ | ✔ (Thuộc bộ môn) | – |
| Đổi tên hiển thị tài liệu | ✔ | ✔ (Thuộc bộ môn) | – |
| Xem danh sách tài liệu | ✔ | ✔ | – (*1) |
| Đọc chi tiết tài liệu (View Document) | ✔ | ✔ | ✔ (Premium) |
| Xem nội dung Document Chunks | ✔ | ✔ (Thuộc bộ môn) | – |
| Xem toàn bộ danh mục môn học trường | ✔ | ✔ | ✔ (*2) |
| Chat với AI (Bất kỳ môn nào) | – | – | ✔ (*2) |
| Lọc tài liệu khi chat (Theo Chương/Tên) | – | – | ✔ (*1) |
| Quản lý Ví (Wallet) / Nạp tiền | – | – | ✔ |

**Chú thích:**
* ✔ = Có quyền | ✔ (Thuộc bộ môn) = Chỉ thao tác trên dữ liệu thuộc Bộ môn mình quản lý | – = Không có quyền.
* (*1) Học sinh không xem được đường dẫn file vật lý gốc, nhưng được xem bảng danh mục "Tên hiển thị tài liệu" trên giao diện Browse để lựa chọn cấu hình bộ lọc RAG khi chat.
* (*2) Học sinh không cần đăng ký (enroll) từng môn, hệ thống cho phép truy cập, tra cứu và hỏi đáp AI tự do trên toàn bộ danh mục môn học hiện hành.
* (*3) Học sinh không cần đăng ký (enroll) từng môn, hệ thống cho phép truy cập, tra cứu và hỏi đáp AI tự do trên toàn bộ danh mục môn học hiện hành.

### 3. ĐẶC TẢ USECASE CHI TIẾT THEO MÃ NGUỒN (SOURCE CODE ACTORS)

#### 3.1 Admin — Quản trị viên
* Nắm quyền kiểm soát hạ tầng, quản trị tài khoản và cấu trúc danh mục Bộ môn/Môn học ở mức cao nhất.
* **Quản lý Tài khoản (User Management):** Cấp phát, vô hiệu hóa tài khoản, đặt lại mật khẩu, import danh sách người dùng qua file Excel.
* **Quản lý Cấu trúc Tổ chức:** Khởi tạo, sửa, xóa các Bộ môn (Departments). Tạo và quản lý Trưởng bộ môn (HOD), thăng cấp/giáng cấp HOD thành Lecturer.
* **Quản lý Tài liệu (Super User):** Được quyền Upload tài liệu cho mọi môn học. Can thiệp, kiểm tra hệ thống, xóa tài liệu lỗi, đổi tên hiển thị, hoặc bật/tắt (Toggle Active/Inactive) trạng thái học liệu bất kể môn học đó thuộc bộ môn nào.

#### 3.2 Head of Department (Trưởng bộ môn)
* Người kiểm soát nội dung, chất lượng đào tạo và phân công giảng dạy của một Bộ môn chuyên ngành cụ thể.
* **Quản lý Môn học:** Tạo mới, đổi tên hoặc xóa các Môn học **thuộc Bộ môn** mà mình phụ trách.
* **Phân công giảng dạy:** Gán hoặc gỡ (Assign/Remove) các Lecturer thuộc bộ môn mình quản lý vào từng Môn học cụ thể.
* **Quản lý Học liệu (Document Management):** Có toàn quyền Upload (Google Drive / Local), Xóa, Đổi tên, và Bật/Tắt trạng thái tài liệu **đối với các môn học thuộc bộ môn của mình**. Không được phép can thiệp vào tài liệu của bộ môn khác.

#### 3.3 Học sinh — Student
* End-user thụ hưởng tài nguyên kiến thức công khai của hệ thống.
* **Tra cứu môn học tự do:** Sau khi đăng nhập, Học sinh được xem toàn bộ danh mục các bộ môn và môn học hiện có thông qua giao diện Browse.
* **Hỏi đáp AI linh hoạt:** Tự do chọn bất kỳ môn học nào để bắt đầu phiên hỏi đáp thời gian thực với trợ lý AI RAG chuyên môn.
* **Hệ thống Chat & Quota:** Học sinh được tương tác với AI thông qua SignalR Hub. Tài khoản Free sẽ bị giới hạn số lượng tin nhắn hoặc các tính năng nâng cao, trong khi tài khoản Premium được miễn giới hạn.
* **Cấu hình bộ lọc tri thức:** Giao diện cung cấp danh sách tài liệu chuẩn (đã được Indexed và Active). Học sinh có thể lọc để AI chỉ trả lời trong phạm vi các tài liệu được chọn.
* **Đọc tài liệu trực tiếp:** Tài khoản Premium có quyền tải/đọc trực tiếp nội dung file vật lý (PDF/DOCX) thông qua endpoint ViewDocument. Tính năng này bị khóa đối với tài khoản Free.
* **Quản lý Ví (Wallet):** Học sinh có hệ thống ví điện tử để nạp tiền và thanh toán các gói nâng cấp Subscription.

### 4. CÁC QUY TẮC NGHIỆP VỤ CỐT LÕI (CORE BUSINESS RULES)

* **BR-01: Quy tắc Phân lập Khoa/Bộ môn (Department Isolation Rule)** — Trưởng bộ môn (HOD) bị giới hạn chặt chẽ trong không gian bộ môn của mình. Các thao tác quản lý môn học, phân công giảng viên hay can thiệp tài liệu sẽ bị hệ thống từ chối (Access Denied) nếu đối tượng mục tiêu không thuộc `DepartmentId` của HOD đó.
* **BR-02: Quy tắc Vòng đời & Trạng thái Học liệu (Lifecycle & State Rule)** — Học liệu khi tải lên hệ thống bắt buộc trải qua trạng thái trung gian. Khi vừa upload, file giữ trạng thái [Pending]. Hệ thống Background Job tiến hành chunking và embedding; nếu thành công sẽ tự động chuyển đổi sang [Indexed] và có thể được [Active] để RAG quét dữ liệu. Lỗi sẽ báo [Failed].
* **BR-03: Quy tắc Bối cảnh RAG môn học mở (RAG Open Context Rule)** — 
  * Quét toàn bộ dữ liệu nằm trong Môn học học sinh đang tương tác (Tuyệt đối không quét chéo môn).
  * Tài liệu phải ở trạng thái Active (Bật) và Indexed.
  * Trong trường hợp Học sinh áp dụng tính năng bộ lọc tài liệu, RAG chỉ thực hiện truy xuất thông tin trong tập con các file được chọn.
  * **Quy tắc xử lý rỗng (Fallback)**: Nếu môn học chưa có tài liệu, hệ thống ngắt luồng và hiển thị thông báo Fallback, không gọi LLM để tránh bịa đặt kiến thức (Hallucination).
* **BR-04: Quy tắc Tiếp cận Tự do (Open Access Rule)** — Mô hình mở rộng - Gỡ bỏ rào cản Đăng ký môn học. Học sinh có toàn quyền tra cứu danh mục học liệu và chat với AI tại tất cả các môn học mà không cần phê duyệt.
* **BR-05: Quy tắc Lưu trữ Kép (Fallback Storage Rule)** — Quá trình upload tài liệu ưu tiên lưu trữ đám mây thông qua Google Drive API. Nếu kết nối Drive thất bại, hệ thống tự động Fallback ghi file vào phân vùng Local Storage của server để đảm bảo luồng công việc không bị gián đoạn.
* **BR-06: Quy tắc Xử lý Tháp dòng (Cascade Deletion Rule)** — Xóa Bộ môn / Môn học (bởi Admin hoặc HOD) sẽ tận dụng cơ chế Cascade Delete của EF Core để tự động xóa toàn bộ tài liệu (Documents), vector chỉ mục (DocumentChunks), và lịch sử chat (ChatSessions) liên quan, dọn dẹp triệt để database.

### 5. QUY TẮC BẢO MẬT & PHI TIẾT LỘ

#### 5.1 Audit Log (Nhật ký kiểm toán)
* Hệ thống ghi nhận tập trung thông qua `IAuditLogService` cho các thao tác nhạy cảm:
* **Hành động của Admin**: Tạo/import tài khoản người dùng, tạo bộ môn, xóa tài liệu từ Dashboard.
* **Hành động của Admin/HOD**: Upload tài liệu mới (lưu vết ID file và môn học), Xóa tài liệu.
* Mỗi log record bao gồm: Actor ID, Action Name, Target ID, và Chi tiết hành động thực hiện.

#### 5.2 Quy định Hiển thị Trích dẫn Nguồn (Citation Constraints)
* Khi AI Chatbot đưa ra câu trả lời, hệ thống bắt buộc render trích dẫn nguồn ngay dưới câu trả lời. 
* Định dạng hiển thị được chuẩn hóa ưu tiên `DisplayName`. Nếu `DisplayName` trống, hệ thống fallback hiển thị `FileName`.
* Giấu kín hoàn toàn đường dẫn lưu trữ vật lý (FilePath trên Drive hoặc Local) khỏi giao diện Học sinh.

### 6. ĐỊNH HƯỚNG MỞ RỘNG TƯƠNG LAI (FUTURE ROADMAP)
* Giới hạn Dung lượng Lưu trữ Học liệu theo từng Bộ môn.
* Thống kê & Dashboard Thông minh cho Trưởng bộ môn theo dõi tiến độ sử dụng AI của sinh viên trong môn học.