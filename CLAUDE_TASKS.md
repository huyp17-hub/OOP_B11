# Hướng dẫn cho Claude Code – Lab 11: Campus Safety Hub

File này là bản giao việc cho Claude Code. Mỗi phiên làm việc, người dùng sẽ nói "Làm Ngày 1" (hoặc 2, 3). Claude Code chỉ làm các task của ngày đó, theo đúng thứ tự, rồi dừng lại báo cáo.

Repo GitHub: **https://github.com/huyp17-hub/OOP_B11.git** (link xem trên web: https://github.com/huyp17-hub/OOP_B11).

Đề gốc nằm ở `docs/lab_w11-pattern.pdf` (người dùng tự chép vào). Nếu đề gốc và file này mâu thuẫn, **làm theo đề gốc** và báo lại cho người dùng.

---

## 0. Quy tắc chung (áp dụng cho mọi task)

### Thông tin sinh viên
Mọi file `.cs` phải mở đầu đúng như sau:
```csharp
/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/
```

### Quy ước code
- C#, .NET 8 trở lên (dùng phiên bản SDK có trên máy, kiểm tra bằng `dotnet --version`). Ứng dụng Console, không có giao diện đồ họa, cơ sở dữ liệu, mạng hay gửi email thật.
- Bật `<Nullable>enable</Nullable>`, build phải **0 warning, 0 error**.
- Mỗi lớp, interface và thành viên public có comment `///` bằng tiếng Việt, nói rõ vai trò. Chỗ nào áp dụng mẫu thiết kế thì ghi `[Pattern: ...]` trong comment của lớp.
- Một file một kiểu, tên file trùng tên kiểu. Đặt namespace theo thư mục.
- Lớp nào không cần kế thừa thì để `sealed`. Field không đổi sau constructor thì để `readonly`.
- Kiểm tra tham số đầu vào: tham chiếu `null` thì ném `ArgumentNullException`.
- **Không được sửa** lớp `LegacySmokeDetector` (giữ nguyên code của đề, chỉ được thêm header sinh viên).

### Quy trình git cho mỗi task
1. Làm xong task thì chạy `dotnet build`, rồi chạy test nếu đã có (`dotnet run --project src/CampusSafetyHub -- test`).
2. Build hoặc test lỗi thì sửa cho xong. **Không commit code đang lỗi.**
3. `git add` đúng các file của task (không add `bin/`, `obj/`), rồi commit với đúng nội dung ghi trong task.
4. Cuối mỗi ngày: `git push`. Remote `origin` là `https://github.com/huyp17-hub/OOP_B11.git`, nhánh `main`. **Không tự tạo repo khác.**
5. Không dùng `git push --force`, không sửa lại các commit đã có.

### Giọng văn tài liệu
README và báo cáo viết bằng tiếng Việt, giọng sinh viên tự viết: xưng "em", câu tự nhiên, ngắn gọn, không dùng văn quảng cáo, không lạm dụng in đậm.

### Kết thúc mỗi ngày
Báo cho người dùng: đã làm các task nào, kết quả build/test, danh sách commit đã push (`git log --oneline` của ngày), và những gì cần người dùng tự kiểm tra hoặc quyết định.

---

## 1. Yêu cầu của đề (tóm tắt)

Chương trình Console mô phỏng trung tâm giám sát an toàn cho một phòng máy:
- tích hợp cảm biến khói đời cũ vào giao diện chuẩn mới (**Adapter**);
- phát cảnh báo cho nhiều bên nhận, có thể đăng ký hoặc hủy đăng ký lúc chạy (**Observer**);
- điều phối các thiết bị ứng phó mà các thiết bị không phụ thuộc trực tiếp vào nhau (**Mediator**).

Luồng chính:
```
LegacySmokeDetector → SmokeSensorAdapter --ISensor--> SensorMonitor
SensorMonitor --AlertEvent--> DashboardObserver, EmailAlertObserver, AuditLogObserver, EmergencyBridgeObserver
EmergencyBridgeObserver --Notify FIRE_DETECTED--> EmergencyMediator → Alarm, DoorController, VentilationController
```

Ngưỡng cảnh báo (giá trị khói sau khi đổi ra %):

| Giá trị | Mức độ |
|---|---|
| Dưới 50% | Bình thường, không phát cảnh báo |
| Từ 50% đến dưới 70% | `WARNING` |
| Từ 70% trở lên | `CRITICAL` |

### Cấu trúc thư mục mục tiêu
```
/
├─ CampusSafetyHub.sln
├─ README.md
├─ .gitignore
├─ CLAUDE_TASKS.md
├─ docs/
│  ├─ lab_w11-pattern.pdf
│  ├─ class-diagram.mmd / class-diagram.png
│  ├─ sequence-fire.mmd / sequence-fire.png
│  ├─ demo-output.txt
│  ├─ test-output.txt
│  └─ report/ (BaoCao_Lab11_202418915.docx, .pdf)
└─ src/CampusSafetyHub/
   ├─ CampusSafetyHub.csproj
   ├─ Program.cs
   ├─ Sensors/      ISensor, LegacySmokeDetector, SmokeSensorAdapter
   ├─ Alerts/       AlertEvent, AlertSeverity, AlertThresholds
   ├─ Monitoring/   IAlertObserver, SensorMonitor
   ├─ Observers/    DashboardObserver, EmailAlertObserver, AuditLogObserver, EmergencyBridgeObserver
   ├─ Emergency/    IEmergencyMediator, EmergencyMediator, EmergencyComponent,
   │                Alarm, DoorController, VentilationController, EmergencyEvents
   └─ Tests/        TestRunner (bộ test tự viết, không cần thư viện ngoài)
```

---

## 2. Ngày 1 – Thứ Sáu 20/11: Khởi tạo, Adapter, Observer lõi

### Task 1.1 – Khởi tạo repo và project
- `git init` (nếu chưa có), `git branch -M main`, `git remote add origin https://github.com/huyp17-hub/OOP_B11.git` (nếu chưa có remote), `dotnet new gitignore`.
- Repo trên GitHub có thể đã có sẵn commit (ví dụ README do GitHub tạo). Nếu có thì chạy `git pull origin main --allow-unrelated-histories` trước lần push đầu tiên, và giải quyết xung đột nếu có. **Không push `--force`.**
- `dotnet new sln -n CampusSafetyHub`, `dotnet new console -n CampusSafetyHub -o src/CampusSafetyHub`, rồi `dotnet sln add`.
- Sửa `.csproj`: bật `Nullable`, `ImplicitUsings` để `enable` cũng được.
- Tạo các thư mục như cấu trúc ở trên. `README.md` lúc này chỉ cần tiêu đề, một đoạn giới thiệu và cách chạy.
- **Xong khi:** `dotnet build` không lỗi, không cảnh báo.
- **Commit:** `chore: khởi tạo solution Campus Safety Hub`

### Task 1.2 – Adapter
- `Sensors/ISensor.cs`: đúng như đề, gồm `double ReadValue();` và `string GetUnit();`.
- `Sensors/LegacySmokeDetector.cs`: chép **nguyên văn** code của đề (thang 0..1000, `GetSmokeLevel`, `GetDescription`, `SetSmokeLevel` có `Math.Clamp`). Chỉ thêm header sinh viên và một comment ghi "lớp có sẵn, không được sửa".
- `Sensors/SmokeSensorAdapter.cs`: `sealed`, cài đặt `ISensor`, nhận `LegacySmokeDetector` qua constructor (null thì ném `ArgumentNullException`), **chứa** đối tượng cũ chứ không kế thừa (object adapter). Hàm `ReadValue()` trả `GetSmokeLevel() / 10.0`, `GetUnit()` trả `"% smoke"`. Không lưu bản sao trạng thái của lớp cũ: mỗi lần đọc đều gọi lại lớp cũ.
- Trong `Program.cs`, viết thử đoạn demo của đề: mức 735 phải ra `73.5` và `% smoke`.
- **Xong khi:** demo in đúng `73.5` / `% smoke`.
- **Commit:** `feat(adapter): SmokeSensorAdapter chuyển thang 0..1000 sang %`

### Task 1.3 – AlertEvent, IAlertObserver, SensorMonitor
- `Alerts/AlertEvent.cs`: `public sealed record AlertEvent(string SensorName, double Value, string Unit, string Severity, DateTime OccurredAt);`. Đây là record bất biến nên observer không sửa được.
- `Alerts/AlertSeverity.cs`: hằng số `Warning = "WARNING"`, `Critical = "CRITICAL"`. Giữ `Severity` kiểu `string` để khớp chữ ký của đề.
- `Alerts/AlertThresholds.cs`: `sealed class` có `WarningThreshold = 50`, `CriticalThreshold = 70` (mặc định theo đề) và hàm `string? Classify(double value)`. Đây là **nơi duy nhất** quyết định giá trị nào vượt ngưỡng (trả lời câu hỏi A.4).
- `Monitoring/IAlertObserver.cs`: `void Update(AlertEvent alertEvent);`
- `Monitoring/SensorMonitor.cs` theo khung của đề, có thêm:
  - Constructor nhận `AlertThresholds` (tùy chọn, mặc định 50/70) và một `TextWriter` để ghi lỗi (tùy chọn, mặc định `Console.Error`).
  - `Attach`: null thì ném lỗi; **không cho đăng ký trùng**.
  - `Detach`: gỡ observer; gỡ observer chưa đăng ký thì không lỗi. Trả `bool` cho biết có gỡ được hay không.
  - Thuộc tính chỉ đọc `ObserverCount`.
  - `CheckSensor(string sensorName, ISensor sensor)`: kiểm tra tham số, đọc giá trị, phân loại qua `AlertThresholds`, dưới ngưỡng thì không phát. Trả `AlertEvent?` để tiện kiểm thử.
  - `Notify` duyệt trên `observers.ToArray()`. **Mỗi observer được bọc trong `try/catch` riêng:** một observer lỗi thì ghi dòng `[MONITOR] Observer <tên kiểu> lỗi: <thông điệp>` rồi vẫn thông báo tiếp cho các observer còn lại.
- **Xong khi:** build sạch. Viết thử trong `Program.cs`: một observer tạm in ra màn hình, ứng với các mức khói 300, 550, 735 thì phải ra không cảnh báo, WARNING, CRITICAL. Xong task thì xóa observer tạm đi.
- **Commit:** `feat(observer): AlertEvent, IAlertObserver và SensorMonitor có xử lý lỗi từng observer`

### Cuối Ngày 1
- `git push`. Báo cáo theo mục 0.

---

## 3. Ngày 2 – Thứ Bảy 21/11: Các observer, Mediator, kịch bản demo

### Task 2.1 – Bốn observer
Tất cả đều `sealed`, cài đặt `IAlertObserver`, chỉ **đọc** `AlertEvent`.
- `DashboardObserver`: in đúng dạng của đề: `[DASHBOARD] {SensorName}: {Value:F1}{Unit} [{Severity}]`.
- `EmailAlertObserver`: constructor nhận địa chỉ người nhận (không rỗng). Chỉ gửi khi mức độ đạt mức tối thiểu cấu hình được (mặc định gửi cả WARNING và CRITICAL). In dạng mô phỏng `[EMAIL] To: {to} | Subject: [{Severity}] {SensorName} | {Value:F1}{Unit} lúc {OccurredAt:HH:mm:ss}`. Không gửi email thật.
- `AuditLogObserver`: lưu từng cảnh báo thành một dòng log vào danh sách nội bộ, lộ ra ngoài dạng `IReadOnlyList<string> Entries` để kiểm thử. Đồng thời in `[AUDIT] ...`.
- `EmergencyBridgeObserver`: constructor nhận `IEmergencyMediator` (là interface, không phải lớp cụ thể). **Chỉ** chuyển cảnh báo `CRITICAL` thành `mediator.Notify(this, EmergencyEvents.FireDetected)`; các mức khác bỏ qua. Lúc này chưa có Mediator nên làm task 2.2 trước rồi quay lại viết lớp này, hoặc viết hai lớp chung một lượt.
- **Commit:** `feat(observer): Dashboard, Email, AuditLog observer`

### Task 2.2 – Mediator và các thiết bị ứng phó
- `Emergency/EmergencyEvents.cs`: hằng số `FireDetected = "FIRE_DETECTED"`, `AlarmReset = "ALARM_RESET"`.
- `Emergency/IEmergencyMediator.cs`: `void Notify(object sender, string eventCode);`
- `Emergency/EmergencyComponent.cs`: lớp `abstract`, giữ tham chiếu `IEmergencyMediator?` dạng `protected`, có hàm `SetMediator(IEmergencyMediator)`. Các thiết bị **không** giữ tham chiếu tới nhau.
- `Emergency/Alarm.cs`: trạng thái `IsActive`. `Activate()` in `[ALARM] Còi báo động BẬT`. `Reset()` tắt còi, in thông báo, rồi gọi `mediator.Notify(this, AlarmReset)`. Lớp này **không biết** `DoorController` tồn tại (trả lời câu hỏi A.3).
- `Emergency/DoorController.cs`: trạng thái `IsUnlocked`. `UnlockAll()` mở khóa cửa thoát hiểm, `LockAll()` khóa lại, cả hai đều in trạng thái.
- `Emergency/VentilationController.cs`: trạng thái `IsRunning`. `Shutdown()` tắt hệ thống thông gió để khói không lan, `Resume()` bật lại, cả hai đều in trạng thái.
- `Emergency/EmergencyMediator.cs`: `sealed`, cài đặt `IEmergencyMediator`. Nhận 3 thiết bị qua constructor, rồi gọi `SetMediator(this)` cho từng thiết bị. Trong `Notify`:
  - `FIRE_DETECTED`: bật còi, mở khóa cửa, tắt thông gió (in tiêu đề `[MEDIATOR] FIRE_DETECTED from ...`).
  - `ALARM_RESET`: khóa cửa lại, bật lại thông gió.
  - Mã sự kiện lạ: in cảnh báo và bỏ qua, không ném lỗi.
  - Đang trong trạng thái cháy mà nhận thêm `FIRE_DETECTED` thì không thực hiện lại các lệnh, chỉ ghi `đã xử lý`.
- Viết `EmergencyBridgeObserver` ở bước này nếu task 2.1 chưa viết.
- **Commit:** `feat(mediator): EmergencyMediator điều phối Alarm, DoorController, VentilationController`

### Task 2.3 – Kịch bản demo trong Program.cs
`Program.cs` nhận tham số: không có tham số thì chạy demo, `test` thì chạy bộ test (bộ test làm ở Ngày 3; hôm nay cứ để in "chưa có test").

Kịch bản demo, mỗi bước in một tiêu đề rõ ràng:
1. Chạy Adapter: cảm biến cũ đang ở mức 735, đọc qua `ISensor` ra `73.5 % smoke`.
2. Dựng hệ thống: monitor; 4 observer; mediator với 3 thiết bị; cầu nối (bridge) giữa observer và mediator.
3. Khói 300 (30%): không có cảnh báo.
4. Khói 550 (55%): WARNING. Dashboard, Email, Audit nhận được; bridge bỏ qua.
5. Khói 820 (82%): CRITICAL. Tất cả nhận được, mediator bật còi, mở cửa, tắt thông gió.
6. Gỡ `EmailAlertObserver`, đọc lại 820: không còn dòng EMAIL nào.
7. Thử `Attach` cùng một observer hai lần: số observer không tăng.
8. Bảo vệ an ninh `Reset()` còi: mediator khóa cửa, bật lại thông gió.
9. In toàn bộ nhật ký Audit.
- Lưu kết quả chạy vào `docs/demo-output.txt`.
- **Commit:** `feat: kịch bản demo đầy đủ Adapter → Observer → Mediator`

### Cuối Ngày 2
- `git push`. Báo cáo theo mục 0.

---

## 4. Ngày 3 – Chủ nhật 22/11: Kiểm thử, sơ đồ, README, báo cáo

### Task 3.1 – Bộ kiểm thử
`Tests/TestRunner.cs` là bộ test tự viết (không dùng xUnit, để chạy bằng `dotnet run -- test` và dễ chụp kết quả). Có các hàm hỗ trợ `Assert`, `AssertThrows<T>`, và hàm bắt nội dung in ra Console. In `[PASS]` / `[FAIL]` cho từng test, dòng tổng kết ở cuối, và trả mã thoát bằng số test FAIL. Mỗi test dùng đối tượng mới, không phụ thuộc test khác.

| Mã | Nội dung kiểm thử |
|---|---|
| T1 | Adapter: mức 735 → `73.5`, đơn vị `% smoke` |
| T2 | Adapter đọc theo thời gian thực: `SetSmokeLevel(400)` trên lớp cũ thì `ReadValue()` ra `40.0` (không sao chép trạng thái) |
| T3 | Adapter: các giá trị biên 0 → 0, 1000 → 100; truyền `null` vào constructor → `ArgumentNullException` |
| T4 | Phân loại ngưỡng: 49.9 → không cảnh báo, 50 → WARNING, 69.9 → WARNING, 70 → CRITICAL |
| T5 | `Attach` hai lần cùng một observer → chỉ nhận thông báo một lần, `ObserverCount` = 1 |
| T6 | `Detach` thì không nhận nữa; `Detach` một observer chưa đăng ký thì không lỗi |
| T7 | Một observer tự `Detach` chính nó ngay trong lúc nhận thông báo → vòng lặp không lỗi, các observer khác vẫn nhận đủ |
| T8 | Một observer ném exception → các observer còn lại vẫn nhận đúng, và có dòng log lỗi |
| T9 | `AlertEvent` là bất biến: không có setter public, hai observer nhận cùng một đối tượng có dữ liệu giống nhau |
| T10 | Bridge: WARNING thì không gọi mediator; CRITICAL thì gọi `FIRE_DETECTED` (dùng một mediator giả để đếm số lần gọi) |
| T11 | Mediator: `FIRE_DETECTED` → còi bật, cửa mở, thông gió tắt |
| T12 | Mediator: `Alarm.Reset()` → còi tắt, cửa khóa, thông gió chạy lại; `Alarm` không có field kiểu `DoorController` (kiểm tra bằng reflection) |
| T13 | Mediator: mã sự kiện lạ không làm thay đổi trạng thái thiết bị; `FIRE_DETECTED` lặp lại không bật còi lần nữa |
| T14 | Tích hợp: khói 820 đi từ đầu đến cuối → đủ 4 observer nhận và mediator xử lý đúng |

- Kiểm tra độ tin cậy của bộ test: tạm thời sửa ngưỡng CRITICAL từ 70 thành 75, xác nhận T4 báo FAIL, rồi **hoàn tác** thay đổi. Ghi lại kết quả lần thử này để đưa vào báo cáo, **không commit** bản code bị sửa.
- Lưu kết quả chạy test vào `docs/test-output.txt`.
- **Commit:** `test: 14 test case cho Adapter, Observer, Mediator`

### Task 3.2 – Sơ đồ
- `docs/class-diagram.mmd` (Mermaid `classDiagram`): đủ các lớp và interface, ký hiệu đúng quan hệ: realization, kế thừa (các thiết bị kế thừa `EmergencyComponent`), composition/association (adapter → legacy, monitor → observers, mediator → các thiết bị, bridge → `IEmergencyMediator`), dependency (`«create»`/`«use»`).
- `docs/sequence-fire.mmd` (Mermaid `sequenceDiagram`): luồng khói 820 → adapter → monitor → các observer → bridge → mediator → 3 thiết bị.
- Xuất ra PNG: `npx -y @mermaid-js/mermaid-cli -i docs/class-diagram.mmd -o docs/class-diagram.png -w 2400` (làm tương tự cho sơ đồ tuần tự). Nếu máy không có Node thì báo người dùng và chỉ giữ file `.mmd`.
- Mở ảnh PNG ra xem lại một lần, sửa chỗ bị chồng chéo hoặc thiếu lớp.
- **Commit:** `docs: sơ đồ lớp và sơ đồ tuần tự luồng cháy`

### Task 3.3 – README
Các mục:
1. Giới thiệu, cách chạy (`dotnet run --project src/CampusSafetyHub`, thêm `-- test`).
2. Cấu trúc thư mục.
3. Sơ đồ lớp (nhúng code mermaid và ảnh PNG), sơ đồ tuần tự.
4. Bảng mô tả trách nhiệm của từng lớp (theo phần A của đề).
5. Mỗi mẫu thiết kế: nằm ở đâu, giải quyết vấn đề gì, lợi ích, hạn chế.
6. Trả lời 4 câu hỏi thiết kế của phần A:
   - Adapter nên kế thừa hay chứa `LegacySmokeDetector`? → Chứa. Lý do: lớp cũ là `sealed`, không kế thừa được; chứa thì giấu được API cũ và đổi được đối tượng cũ khác.
   - `SensorMonitor` có nên biết `EmailAlertObserver` cụ thể không? → Không, chỉ biết `IAlertObserver`.
   - `Alarm` có cần tham chiếu đến `DoorController` không? → Không, mọi phối hợp đi qua mediator.
   - Ai quyết định ngưỡng? → `AlertThresholds`, được `SensorMonitor` dùng; observer không tự quyết định.
7. Kết quả demo và kết quả test (chép từ `docs/`).
- **Commit:** `docs: README đầy đủ, trả lời câu hỏi thiết kế`

### Task 3.4 – Báo cáo Word và PDF
Tạo `docs/report/BaoCao_Lab11_202418915.docx` và bản `.pdf`. Cách làm gợi ý: viết `docs/report/report.md` rồi chuyển bằng `pandoc report.md -o BaoCao_Lab11_202418915.docx`. Bản PDF xuất từ Word/LibreOffice; nếu máy không có công cụ thì báo người dùng tự mở file `.docx` rồi Save As PDF.

Nội dung báo cáo (theo yêu cầu nộp bài):
- **Trang đầu:** tên bài, họ tên, mã sinh viên, link GitHub: https://github.com/huyp17-hub/OOP_B11 (viết dạng link bấm được).
- **Danh sách lớp:** bảng gồm tên lớp, loại (class/interface/record), vai trò, mẫu thiết kế liên quan.
- **Sơ đồ quan hệ:** ảnh sơ đồ lớp và sơ đồ tuần tự, kèm bảng giải thích từng quan hệ.
- **Kết quả chạy:** nội dung `docs/demo-output.txt`, có giải thích ngắn cho từng bước.
- **Kiểm thử:** bảng T1–T14 (tình huống, mong đợi, thực tế, PASS/FAIL), nội dung `docs/test-output.txt`, và kết quả lần thử sửa ngưỡng ở task 3.1.
- **Trả lời câu hỏi thiết kế.**
- Mở file ra xem lại: ảnh hiển thị được, bảng không vỡ, không có chỗ trống chưa điền.
- **Commit:** `docs: báo cáo Lab 11 (docx, pdf)`

### Task 3.5 – Kiểm tra cuối
- Clone repo về một thư mục tạm khác, chạy `dotnet build` và `dotnet run -- test` để bảo đảm người khác tải về là chạy được.
- Kiểm tra mọi file `.cs` đều có header sinh viên (ví dụ dùng `grep -L "Mã sinh viên: 202418915"`).
- `git push`. Báo cáo theo mục 0, gửi kèm link repo và danh sách file cần nộp.

---

## 5. Danh sách nộp bài
- [ ] Mã nguồn trên GitHub, mọi file `.cs` có header sinh viên và comment đầy đủ
- [ ] Báo cáo Word/PDF có link GitHub
- [ ] Danh sách lớp và sơ đồ quan hệ
- [ ] Kết quả chạy và kết quả kiểm thử
