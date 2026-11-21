/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Emergency;

/// <summary>
/// Bộ điều khiển cửa thoát hiểm. [Pattern: Mediator] Chỉ nhận lệnh từ
/// trung gian, không tự biết về còi hay thông gió.
/// </summary>
public sealed class DoorController : EmergencyComponent
{
    /// <summary>Cửa thoát hiểm đang mở khóa hay không.</summary>
    public bool IsUnlocked { get; private set; }

    /// <summary>
    /// Mở khóa toàn bộ cửa thoát hiểm để người thoát ra ngoài.
    /// </summary>
    public void UnlockAll()
    {
        IsUnlocked = true;
        Console.WriteLine("[DOOR] Mở khóa toàn bộ cửa thoát hiểm");
    }

    /// <summary>
    /// Khóa lại toàn bộ cửa khi hết tình huống khẩn cấp.
    /// </summary>
    public void LockAll()
    {
        IsUnlocked = false;
        Console.WriteLine("[DOOR] Khóa lại toàn bộ cửa");
    }
}
