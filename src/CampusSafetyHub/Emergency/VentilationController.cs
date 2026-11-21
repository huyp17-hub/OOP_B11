/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Emergency;

/// <summary>
/// Bộ điều khiển hệ thống thông gió. [Pattern: Mediator] Chỉ nhận lệnh từ
/// trung gian, không tự biết về còi hay cửa.
/// </summary>
public sealed class VentilationController : EmergencyComponent
{
    /// <summary>Hệ thống thông gió đang chạy hay không.</summary>
    public bool IsRunning { get; private set; } = true;

    /// <summary>
    /// Tắt thông gió để khói không bị thổi lan ra các khu vực khác.
    /// </summary>
    public void Shutdown()
    {
        IsRunning = false;
        Console.WriteLine("[VENT] Tắt hệ thống thông gió để khói không lan");
    }

    /// <summary>
    /// Bật lại thông gió khi tình huống đã an toàn.
    /// </summary>
    public void Resume()
    {
        IsRunning = true;
        Console.WriteLine("[VENT] Bật lại hệ thống thông gió");
    }
}
