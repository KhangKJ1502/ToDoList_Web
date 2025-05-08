Sơ Lược Về Project: 
+ Ứng dụng To-Do List là một hệ thống quản lý công việc cá nhân, cho phép người dùng tạo, quản lý, sắp xếp các nhiệm vụ (Tasks) và các công việc con (Subtasks), đồng thời gán nhãn (Tags), thiết lập lời nhắc (Reminders).
+ Ứng dụng được xây dựng theo mô hình Clean Architecture nhằm đảm bảo:
   -Tách biệt các lớp xử lý nghiệp vụ, dữ liệu và giao diện, từ đó dễ bảo trì, mở rộng và kiểm thử.
   -Tăng khả năng tái sử dụng mã nguồn giữa các lớp, ví dụ như dịch vụ xử lý nghiệp vụ có thể sử dụng cho cả Web API và giao diện MVC.
+  Một số tính năng nổi bật của ứng dụng
   -Quản lý Task/Subtask dạng phân cấp.
   -Gán nhiều Tags cho mỗi Task (mối quan hệ nhiều-nhiều).
   -Đặt lời nhắc (Reminders) cho Task theo thời gian cụ thể.
   - Giao diện thân thiện, dễ sử dụng và có phân quyền người dùng.
