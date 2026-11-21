/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Emergency;

/// <summary>
/// Còi báo động. [Pattern: Mediator] Lớp này KHÔNG biết tới
/// <c>DoorController</c> hay <c>VentilationController</c>; khi tắt còi nó chỉ
/// báo cho trung gian bằng sự kiện <see cref="EmergencyEvents.AlarmReset"/>,
/// trung gian mới là nơi quyết định khóa cửa và bật lại thông gió.
/// </summary>
public sealed class Alarm : EmergencyComponent
{
    /// <summary>Còi đang kêu hay không.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Bật còi báo động. Nếu còi đã bật thì giữ nguyên.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        Console.WriteLine("[ALARM] Còi báo động BẬT");
    }

    /// <summary>
    /// Tắt còi báo động và báo cho trung gian để kết thúc trạng thái khẩn cấp.
    /// </summary>
    public void Reset()
    {
        IsActive = false;
        Console.WriteLine("[ALARM] Còi báo động TẮT");
        Mediator?.Notify(this, EmergencyEvents.AlarmReset);
    }
}
