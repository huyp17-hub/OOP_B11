/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Alerts;
using CampusSafetyHub.Emergency;
using CampusSafetyHub.Monitoring;
using CampusSafetyHub.Observers;
using CampusSafetyHub.Sensors;
using CampusSafetyHub.Tests;

namespace CampusSafetyHub;

/// <summary>
/// Điểm vào của chương trình Campus Safety Hub.
/// Không có tham số thì chạy kịch bản demo; tham số "test" thì chạy bộ test
/// (bộ test được bổ sung ở Ngày 3).
/// </summary>
internal static class Program
{
    /// <summary>
    /// Hàm main: chọn chạy demo hay chạy test theo tham số dòng lệnh.
    /// </summary>
    private static int Main(string[] args)
    {
        if (args.Length > 0 && args[0].Equals("test", StringComparison.OrdinalIgnoreCase))
        {
            return TestRunner.Run();
        }

        RunDemo();
        return 0;
    }

    /// <summary>
    /// Kịch bản demo đầy đủ: Adapter → Observer → Mediator.
    /// </summary>
    private static void RunDemo()
    {
        PrintHeader("1. Adapter: đọc cảm biến khói đời cũ qua giao diện chuẩn");
        LegacySmokeDetector legacy = new(735);
        ISensor sensor = new SmokeSensorAdapter(legacy);
        Console.WriteLine(
            $"Cảm biến cũ mức 735 → đọc qua ISensor: {sensor.ReadValue():F1}{sensor.GetUnit()}");

        PrintHeader("2. Dựng hệ thống giám sát và ứng phó");
        SensorMonitor monitor = new();

        DashboardObserver dashboard = new();
        EmailAlertObserver email = new("truongphong@campus.edu");
        AuditLogObserver audit = new();

        Alarm alarm = new();
        DoorController door = new();
        VentilationController ventilation = new();
        EmergencyMediator mediator = new(alarm, door, ventilation);
        EmergencyBridgeObserver bridge = new(mediator);

        monitor.Attach(dashboard);
        monitor.Attach(email);
        monitor.Attach(audit);
        monitor.Attach(bridge);
        Console.WriteLine($"Đã gắn {monitor.ObserverCount} observer vào monitor.");

        PrintHeader("3. Khói 30% → dưới ngưỡng, không cảnh báo");
        legacy.SetSmokeLevel(300);
        AlertEventIsNull(monitor.CheckSensor("SmokeSensor-01", sensor));

        PrintHeader("4. Khói 55% → WARNING (Dashboard, Email, Audit nhận; bridge bỏ qua)");
        legacy.SetSmokeLevel(550);
        monitor.CheckSensor("SmokeSensor-01", sensor);

        PrintHeader("5. Khói 82% → CRITICAL (tất cả nhận; mediator bật còi, mở cửa, tắt thông gió)");
        legacy.SetSmokeLevel(820);
        monitor.CheckSensor("SmokeSensor-01", sensor);

        PrintHeader("6. Gỡ EmailAlertObserver rồi đọc lại 82% → không còn dòng EMAIL");
        monitor.Detach(email);
        Console.WriteLine($"Số observer sau khi gỡ: {monitor.ObserverCount}");
        monitor.CheckSensor("SmokeSensor-01", sensor);

        PrintHeader("7. Attach cùng một observer hai lần → số observer không tăng");
        Console.WriteLine($"Trước: {monitor.ObserverCount}");
        monitor.Attach(dashboard);
        Console.WriteLine($"Sau khi Attach lại dashboard: {monitor.ObserverCount}");

        PrintHeader("8. Bảo vệ an ninh tắt còi → mediator khóa cửa, bật lại thông gió");
        alarm.Reset();

        PrintHeader("9. Toàn bộ nhật ký Audit");
        foreach (string entry in audit.Entries)
        {
            Console.WriteLine($"  - {entry}");
        }
    }

    /// <summary>In một tiêu đề phần cho dễ đọc.</summary>
    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine("================================================================");
        Console.WriteLine(title);
        Console.WriteLine("================================================================");
    }

    /// <summary>In xác nhận khi không có cảnh báo nào được phát.</summary>
    private static void AlertEventIsNull(AlertEvent? alertEvent)
    {
        Console.WriteLine(alertEvent is null
            ? "→ Không phát cảnh báo (dưới ngưỡng)."
            : $"→ Có cảnh báo: {alertEvent.Severity}");
    }
}
