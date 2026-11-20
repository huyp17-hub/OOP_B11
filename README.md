# Campus Safety Hub – Lab 11 (OOP)

Bài lab môn Lập trình hướng đối tượng. Em viết một chương trình Console mô phỏng
trung tâm giám sát an toàn cho một phòng máy, áp dụng ba mẫu thiết kế:

- Adapter: gắn cảm biến khói đời cũ vào giao diện cảm biến chuẩn mới.
- Observer: phát cảnh báo tới nhiều bên nhận, có thể đăng ký/hủy lúc chạy.
- Mediator: điều phối các thiết bị ứng phó (còi, cửa, thông gió) mà chúng không
  phụ thuộc trực tiếp vào nhau.

- Sinh viên: Phạm Quốc Huy
- Mã sinh viên: 202418915
- GitHub: https://github.com/huyp17-hub/OOP_B11

## Cách chạy

Cần .NET SDK 8.0 trở lên.

```bash
# Chạy demo
dotnet run --project src/CampusSafetyHub

# Chạy bộ test
dotnet run --project src/CampusSafetyHub -- test
```

Phần hướng dẫn chi tiết, sơ đồ lớp và trả lời câu hỏi thiết kế sẽ được bổ sung
ở các bước sau.
