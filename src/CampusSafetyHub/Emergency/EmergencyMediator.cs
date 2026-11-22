/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Emergency;

/// <summary>
/// Trung gian điều phối ứng phó khẩn cấp.
/// [Pattern: Mediator] Đây là concrete mediator: nó biết cả ba thiết bị và
/// quyết định chúng phối hợp với nhau thế nào, nhờ vậy các thiết bị không
/// phụ thuộc trực tiếp vào nhau.
/// </summary>
public sealed class EmergencyMediator : IEmergencyMediator
{
    private readonly Alarm _alarm;
    private readonly DoorController _door;
    private readonly VentilationController _ventilation;

    /// <summary>Hệ thống có đang trong trạng thái cháy hay không.</summary>
    private bool _fireActive;

    /// <summary>
    /// Tạo trung gian và tự gán chính mình làm mediator cho cả ba thiết bị.
    /// </summary>
    /// <param name="alarm">Còi báo động; không được null.</param>
    /// <param name="door">Bộ điều khiển cửa; không được null.</param>
    /// <param name="ventilation">Bộ điều khiển thông gió; không được null.</param>
    /// <exception cref="ArgumentNullException">Khi một tham số là null.</exception>
    public EmergencyMediator(Alarm alarm, DoorController door, VentilationController ventilation)
    {
        ArgumentNullException.ThrowIfNull(alarm);
        ArgumentNullException.ThrowIfNull(door);
        ArgumentNullException.ThrowIfNull(ventilation);

        _alarm = alarm;
        _door = door;
        _ventilation = ventilation;

        _alarm.SetMediator(this);
        _door.SetMediator(this);
        _ventilation.SetMediator(this);
    }

    /// <summary>
    /// Nhận sự kiện và điều phối các thiết bị tương ứng.
    /// </summary>
    /// <param name="sender">Bên phát ra sự kiện.</param>
    /// <param name="eventCode">Mã sự kiện, xem <see cref="EmergencyEvents"/>.</param>
    /// <exception cref="ArgumentNullException">Khi <paramref name="eventCode"/> là null.</exception>
    public void Notify(object sender, string eventCode)
    {
        ArgumentNullException.ThrowIfNull(eventCode);

        switch (eventCode)
        {
            case EmergencyEvents.FireDetected:
                HandleFireDetected(sender);
                break;

            case EmergencyEvents.AlarmReset:
                HandleAlarmReset();
                break;

            default:
                Console.WriteLine($"[MEDIATOR] Mã sự kiện lạ '{eventCode}', bỏ qua.");
                break;
        }
    }

    /// <summary>
    /// Xử lý phát hiện cháy: bật còi, mở cửa, tắt thông gió. Nếu đang trong
    /// trạng thái cháy thì không thực hiện lại, chỉ ghi nhận đã xử lý.
    /// </summary>
    private void HandleFireDetected(object sender)
    {
        if (_fireActive)
        {
            Console.WriteLine(
                $"[MEDIATOR] FIRE_DETECTED từ {sender.GetType().Name}: đã xử lý, bỏ qua.");
            return;
        }

        _fireActive = true;
        Console.WriteLine($"[MEDIATOR] FIRE_DETECTED từ {sender.GetType().Name}");
        _alarm.Activate();
        _door.UnlockAll();
        _ventilation.Shutdown();
    }

    /// <summary>
    /// Xử lý tắt còi: khóa cửa lại và bật lại thông gió, kết thúc trạng thái cháy.
    /// </summary>
    private void HandleAlarmReset()
    {
        Console.WriteLine("[MEDIATOR] ALARM_RESET");
        _door.LockAll();
        _ventilation.Resume();
        _fireActive = false;
    }
}
