/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Alerts;

/// <summary>
/// Các hằng số mức độ cảnh báo. Giữ kiểu chuỗi để khớp chữ ký
/// <c>Severity</c> của <see cref="AlertEvent"/> theo yêu cầu đề.
/// </summary>
public static class AlertSeverity
{
    /// <summary>Mức cảnh báo vừa: khói từ 50% đến dưới 70%.</summary>
    public const string Warning = "WARNING";

    /// <summary>Mức cảnh báo nguy hiểm: khói từ 70% trở lên.</summary>
    public const string Critical = "CRITICAL";
}
