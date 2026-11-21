/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Emergency;

/// <summary>
/// Các mã sự kiện khẩn cấp dùng để trao đổi qua mediator.
/// Giữ kiểu chuỗi hằng số để các bên gửi/nhận dùng chung một giá trị.
/// </summary>
public static class EmergencyEvents
{
    /// <summary>Phát hiện cháy: cần bật còi, mở cửa thoát hiểm, tắt thông gió.</summary>
    public const string FireDetected = "FIRE_DETECTED";

    /// <summary>Đã tắt còi báo động: cần khóa cửa lại và bật lại thông gió.</summary>
    public const string AlarmReset = "ALARM_RESET";
}
