/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using System.Reflection;
using CampusSafetyHub.Alerts;
using CampusSafetyHub.Emergency;
using CampusSafetyHub.Monitoring;
using CampusSafetyHub.Observers;
using CampusSafetyHub.Sensors;

namespace CampusSafetyHub.Tests;

/// <summary>
/// Bộ kiểm thử tự viết (không dùng thư viện ngoài) cho Adapter, Observer, Mediator.
/// Chạy bằng <c>dotnet run --project src/CampusSafetyHub -- test</c>.
/// In [PASS]/[FAIL] cho từng test, có dòng tổng kết và trả mã thoát bằng số test FAIL.
/// Mỗi test tạo đối tượng mới, không phụ thuộc vào test khác.
/// </summary>
public static class TestRunner
{
    private static int _passed;
    private static int _failed;

    /// <summary>
    /// Chạy toàn bộ test. Trả về số test FAIL để dùng làm mã thoát của chương trình.
    /// </summary>
    public static int Run()
    {
        _passed = 0;
        _failed = 0;

        Console.WriteLine("================================================================");
        Console.WriteLine("BỘ KIỂM THỬ CAMPUS SAFETY HUB");
        Console.WriteLine("================================================================");

        Check("T1", "Adapter: mức 735 → 73.5, đơn vị '% smoke'", T1);
        Check("T2", "Adapter đọc theo thời gian thực (không sao chép trạng thái)", T2);
        Check("T3", "Adapter: biên 0→0, 1000→100; null ctor → ArgumentNullException", T3);
        Check("T4", "Phân loại ngưỡng: 49.9 none, 50 WARNING, 69.9 WARNING, 70 CRITICAL", T4);
        Check("T5", "Attach hai lần cùng observer → nhận một lần, ObserverCount = 1", T5);
        Check("T6", "Detach thì không nhận nữa; Detach observer chưa đăng ký không lỗi", T6);
        Check("T7", "Observer tự Detach chính mình khi nhận → không lỗi, observer khác vẫn nhận", T7);
        Check("T8", "Một observer ném exception → observer còn lại vẫn nhận, có log lỗi", T8);
        Check("T9", "AlertEvent bất biến; hai observer nhận cùng một đối tượng", T9);
        Check("T10", "Bridge: WARNING không gọi mediator; CRITICAL gọi FIRE_DETECTED", T10);
        Check("T11", "Mediator: FIRE_DETECTED → còi bật, cửa mở, thông gió tắt", T11);
        Check("T12", "Mediator: Alarm.Reset() → còi tắt, cửa khóa, thông gió chạy; Alarm không có field DoorController", T12);
        Check("T13", "Mediator: sự kiện lạ không đổi trạng thái; FIRE_DETECTED lặp lại không bật còi lần nữa", T13);
        Check("T14", "Tích hợp: khói 820 đi từ đầu đến cuối → 4 observer nhận, mediator xử lý đúng", T14);

        Console.WriteLine("================================================================");
        Console.WriteLine($"TỔNG KẾT: {_passed} PASS, {_failed} FAIL / {_passed + _failed} test.");
        Console.WriteLine("================================================================");
        return _failed;
    }

    // ---------------------------------------------------------------- Tests

    /// <summary>T1 – Adapter quy đổi 735 sang 73.5 và trả đúng đơn vị.</summary>
    private static void T1()
    {
        ISensor sensor = AdapterAt(735);
        AssertDouble(73.5, sensor.ReadValue(), "ReadValue");
        AssertEqual("% smoke", sensor.GetUnit(), "GetUnit");
    }

    /// <summary>T2 – Đổi giá trị trên lớp cũ thì adapter đọc ra ngay (không lưu bản sao).</summary>
    private static void T2()
    {
        LegacySmokeDetector legacy = new(0);
        ISensor sensor = new SmokeSensorAdapter(legacy);
        legacy.SetSmokeLevel(400);
        AssertDouble(40.0, sensor.ReadValue(), "ReadValue sau khi đổi 400");
        legacy.SetSmokeLevel(900);
        AssertDouble(90.0, sensor.ReadValue(), "ReadValue sau khi đổi 900");
    }

    /// <summary>T3 – Giá trị biên và kiểm tra null ở constructor.</summary>
    private static void T3()
    {
        AssertDouble(0.0, AdapterAt(0).ReadValue(), "biên 0");
        AssertDouble(100.0, AdapterAt(1000).ReadValue(), "biên 1000");
        AssertThrows<ArgumentNullException>(
            () => _ = new SmokeSensorAdapter(null!), "null ctor");
    }

    /// <summary>T4 – Ranh giới phân loại ngưỡng (nguồn quyết định duy nhất là AlertThresholds).</summary>
    private static void T4()
    {
        AlertThresholds t = new();
        AssertEqual((string?)null, t.Classify(49.9), "49.9");
        AssertEqual(AlertSeverity.Warning, t.Classify(50), "50");
        AssertEqual(AlertSeverity.Warning, t.Classify(69.9), "69.9");
        AssertEqual(AlertSeverity.Critical, t.Classify(70), "70");
    }

    /// <summary>T5 – Không cho đăng ký trùng; observer trùng chỉ nhận một lần.</summary>
    private static void T5()
    {
        SensorMonitor monitor = new();
        RecordingObserver r = new();
        monitor.Attach(r);
        monitor.Attach(r);
        AssertEqual(1, monitor.ObserverCount, "ObserverCount sau Attach trùng");
        Capture(() => monitor.CheckSensor("S", new FixedSensor(80)));
        AssertEqual(1, r.Count, "số lần nhận");
    }

    /// <summary>T6 – Detach dừng nhận; Detach observer lạ không gây lỗi.</summary>
    private static void T6()
    {
        SensorMonitor monitor = new();
        RecordingObserver r = new();
        monitor.Attach(r);
        AssertEqual(true, monitor.Detach(r), "Detach observer đã đăng ký trả true");
        Capture(() => monitor.CheckSensor("S", new FixedSensor(80)));
        AssertEqual(0, r.Count, "không còn nhận sau Detach");
        AssertEqual(false, monitor.Detach(new RecordingObserver()), "Detach observer chưa đăng ký trả false, không lỗi");
    }

    /// <summary>T7 – Observer tự gỡ mình trong lúc nhận không làm hỏng vòng lặp.</summary>
    private static void T7()
    {
        SensorMonitor monitor = new();
        SelfDetachObserver self = new(monitor);
        RecordingObserver other = new();
        monitor.Attach(self);
        monitor.Attach(other);
        Capture(() => monitor.CheckSensor("S", new FixedSensor(80)));
        AssertEqual(1, self.Count, "self nhận đúng một lần");
        AssertEqual(1, other.Count, "observer khác vẫn nhận đủ");
        AssertEqual(1, monitor.ObserverCount, "self đã tự gỡ");
    }

    /// <summary>T8 – Một observer ném lỗi không làm hỏng các observer còn lại; có dòng log lỗi.</summary>
    private static void T8()
    {
        StringWriter errorLog = new();
        SensorMonitor monitor = new(errorWriter: errorLog);
        monitor.Attach(new ThrowingObserver());
        RecordingObserver good = new();
        monitor.Attach(good);
        Capture(() => monitor.CheckSensor("S", new FixedSensor(80)));
        AssertEqual(1, good.Count, "observer tốt vẫn nhận");
        string log = errorLog.ToString();
        Assert(log.Contains("[MONITOR]") && log.Contains(nameof(ThrowingObserver)),
            "phải có dòng log lỗi nêu tên kiểu observer");
    }

    /// <summary>T9 – AlertEvent không có setter public; mọi observer nhận cùng một đối tượng.</summary>
    private static void T9()
    {
        foreach (PropertyInfo p in typeof(AlertEvent).GetProperties())
        {
            MethodInfo? setter = p.SetMethod;
            bool isInitOnly = setter is not null && setter.ReturnParameter
                .GetRequiredCustomModifiers()
                .Any(m => m.Name == "IsExternalInit");
            Assert(setter is null || isInitOnly,
                $"thuộc tính {p.Name} không được có setter public thường");
        }

        SensorMonitor monitor = new();
        CaptureObserver o1 = new();
        CaptureObserver o2 = new();
        monitor.Attach(o1);
        monitor.Attach(o2);
        AlertEvent? emitted = null;
        Capture(() => emitted = monitor.CheckSensor("S", new FixedSensor(80)));
        Assert(ReferenceEquals(o1.Last, o2.Last), "hai observer phải nhận cùng một đối tượng");
        Assert(ReferenceEquals(emitted, o1.Last), "đối tượng phát ra chính là đối tượng observer nhận");
        AssertEqual(o1.Last, o2.Last, "dữ liệu cảnh báo giống nhau");
    }

    /// <summary>T10 – Cầu nối chỉ chuyển CRITICAL thành FIRE_DETECTED cho mediator.</summary>
    private static void T10()
    {
        CountingMediator mediator = new();
        EmergencyBridgeObserver bridge = new(mediator);
        bridge.Update(new AlertEvent("S", 55, "% smoke", AlertSeverity.Warning, DateTime.Now));
        AssertEqual(0, mediator.NotifyCount, "WARNING không gọi mediator");
        bridge.Update(new AlertEvent("S", 82, "% smoke", AlertSeverity.Critical, DateTime.Now));
        AssertEqual(1, mediator.NotifyCount, "CRITICAL gọi mediator một lần");
        AssertEqual(EmergencyEvents.FireDetected, mediator.LastEvent, "mã sự kiện là FIRE_DETECTED");
    }

    /// <summary>T11 – FIRE_DETECTED bật còi, mở cửa, tắt thông gió.</summary>
    private static void T11()
    {
        Alarm alarm = new();
        DoorController door = new();
        VentilationController ventilation = new();
        EmergencyMediator mediator = new(alarm, door, ventilation);
        Capture(() => mediator.Notify(new object(), EmergencyEvents.FireDetected));
        AssertEqual(true, alarm.IsActive, "còi bật");
        AssertEqual(true, door.IsUnlocked, "cửa mở");
        AssertEqual(false, ventilation.IsRunning, "thông gió tắt");
    }

    /// <summary>T12 – Reset còi khóa cửa và bật lại thông gió; Alarm không giữ field DoorController.</summary>
    private static void T12()
    {
        Alarm alarm = new();
        DoorController door = new();
        VentilationController ventilation = new();
        EmergencyMediator mediator = new(alarm, door, ventilation);
        Capture(() => mediator.Notify(new object(), EmergencyEvents.FireDetected));
        Capture(alarm.Reset);
        AssertEqual(false, alarm.IsActive, "còi tắt");
        AssertEqual(false, door.IsUnlocked, "cửa khóa lại");
        AssertEqual(true, ventilation.IsRunning, "thông gió chạy lại");

        FieldInfo[] fields = typeof(Alarm).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(fields.All(f => f.FieldType != typeof(DoorController)),
            "Alarm không được giữ tham chiếu trực tiếp tới DoorController");
    }

    /// <summary>T13 – Sự kiện lạ không đổi trạng thái; FIRE_DETECTED lặp lại không bật còi lần nữa.</summary>
    private static void T13()
    {
        Alarm alarm = new();
        DoorController door = new();
        VentilationController ventilation = new();
        EmergencyMediator mediator = new(alarm, door, ventilation);

        Capture(() => mediator.Notify(new object(), "SU_KIEN_LA"));
        AssertEqual(false, alarm.IsActive, "sự kiện lạ không bật còi");
        AssertEqual(false, door.IsUnlocked, "sự kiện lạ không mở cửa");
        AssertEqual(true, ventilation.IsRunning, "sự kiện lạ không tắt thông gió");

        Capture(() => mediator.Notify(new object(), EmergencyEvents.FireDetected));
        string second = Capture(() => mediator.Notify(new object(), EmergencyEvents.FireDetected));
        Assert(!second.Contains("Còi báo động BẬT"), "lần cháy thứ hai không bật còi lại");
        Assert(second.Contains("đã xử lý"), "lần cháy thứ hai chỉ ghi nhận đã xử lý");
    }

    /// <summary>T14 – Dòng chảy tích hợp đầy đủ với khói 820 (82%).</summary>
    private static void T14()
    {
        LegacySmokeDetector legacy = new(0);
        legacy.SetSmokeLevel(820);
        ISensor sensor = new SmokeSensorAdapter(legacy);

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

        string output = Capture(() => monitor.CheckSensor("SmokeSensor-01", sensor));

        Assert(output.Contains("[DASHBOARD]"), "Dashboard nhận cảnh báo");
        Assert(output.Contains("[EMAIL]"), "Email nhận cảnh báo");
        AssertEqual(1, audit.Entries.Count, "Audit ghi đúng một dòng");
        AssertEqual(true, alarm.IsActive, "bridge → mediator bật còi");
        AssertEqual(true, door.IsUnlocked, "bridge → mediator mở cửa");
        AssertEqual(false, ventilation.IsRunning, "bridge → mediator tắt thông gió");
    }

    // ---------------------------------------------------------- Khung chạy

    /// <summary>Chạy một test, bắt ngoại lệ để in [PASS]/[FAIL] và đếm kết quả.</summary>
    private static void Check(string id, string description, Action body)
    {
        try
        {
            body();
            _passed++;
            Console.WriteLine($"[PASS] {id}: {description}");
        }
        catch (Exception ex)
        {
            _failed++;
            Console.WriteLine($"[FAIL] {id}: {description} -> {ex.Message}");
        }
    }

    // --------------------------------------------------------- Hàm hỗ trợ

    /// <summary>Tạo adapter bọc một cảm biến cũ đặt sẵn ở mức cho trước.</summary>
    private static ISensor AdapterAt(int legacyLevel)
    {
        LegacySmokeDetector legacy = new(0);
        legacy.SetSmokeLevel(legacyLevel);
        return new SmokeSensorAdapter(legacy);
    }

    /// <summary>Khẳng định điều kiện đúng, ngược lại ném lỗi để test FAIL.</summary>
    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>Khẳng định hai giá trị bằng nhau.</summary>
    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException(
                $"{message}: mong đợi '{expected?.ToString() ?? "null"}', thực tế '{actual?.ToString() ?? "null"}'");
        }
    }

    /// <summary>Khẳng định hai số thực bằng nhau trong sai số nhỏ.</summary>
    private static void AssertDouble(double expected, double actual, string message)
    {
        if (Math.Abs(expected - actual) > 1e-9)
        {
            throw new InvalidOperationException($"{message}: mong đợi {expected}, thực tế {actual}");
        }
    }

    /// <summary>Khẳng định một hành động ném đúng loại ngoại lệ mong đợi.</summary>
    private static void AssertThrows<TException>(Action action, string message)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"{message}: ném {ex.GetType().Name} thay vì {typeof(TException).Name}");
        }

        throw new InvalidOperationException($"{message}: không ném ngoại lệ nào");
    }

    /// <summary>Chạy một hành động và thu lại toàn bộ nội dung in ra Console.</summary>
    private static string Capture(Action action)
    {
        TextWriter original = Console.Out;
        StringWriter buffer = new();
        Console.SetOut(buffer);
        try
        {
            action();
        }
        finally
        {
            Console.SetOut(original);
        }

        return buffer.ToString();
    }

    // -------------------------------------------------- Đối tượng giả cho test

    /// <summary>Cảm biến giả trả về một giá trị cố định, dùng cho test.</summary>
    private sealed class FixedSensor : ISensor
    {
        private readonly double _value;

        public FixedSensor(double value) => _value = value;

        public double ReadValue() => _value;

        public string GetUnit() => "% smoke";
    }

    /// <summary>Observer đếm số lần được gọi.</summary>
    private sealed class RecordingObserver : IAlertObserver
    {
        public int Count { get; private set; }

        public void Update(AlertEvent alertEvent) => Count++;
    }

    /// <summary>Observer giữ lại cảnh báo cuối cùng nhận được.</summary>
    private sealed class CaptureObserver : IAlertObserver
    {
        public AlertEvent? Last { get; private set; }

        public void Update(AlertEvent alertEvent) => Last = alertEvent;
    }

    /// <summary>Observer luôn ném lỗi, để kiểm tra monitor vẫn tiếp tục.</summary>
    private sealed class ThrowingObserver : IAlertObserver
    {
        public void Update(AlertEvent alertEvent) =>
            throw new InvalidOperationException("Lỗi giả lập trong observer");
    }

    /// <summary>Observer tự gỡ chính mình khỏi monitor ngay khi nhận cảnh báo.</summary>
    private sealed class SelfDetachObserver : IAlertObserver
    {
        private readonly SensorMonitor _monitor;

        public SelfDetachObserver(SensorMonitor monitor) => _monitor = monitor;

        public int Count { get; private set; }

        public void Update(AlertEvent alertEvent)
        {
            Count++;
            _monitor.Detach(this);
        }
    }

    /// <summary>Mediator giả đếm số lần được gọi và nhớ mã sự kiện cuối.</summary>
    private sealed class CountingMediator : IEmergencyMediator
    {
        public int NotifyCount { get; private set; }

        public string? LastEvent { get; private set; }

        public void Notify(object sender, string eventCode)
        {
            NotifyCount++;
            LastEvent = eventCode;
        }
    }
}
