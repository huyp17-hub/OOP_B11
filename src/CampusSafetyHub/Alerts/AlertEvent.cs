/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Alerts;

/// <summary>
/// Dữ liệu một lần cảnh báo được phát đi. Là record bất biến nên
/// các observer chỉ đọc được chứ không sửa được nội dung cảnh báo.
/// </summary>
/// <param name="SensorName">Tên cảm biến phát ra cảnh báo.</param>
/// <param name="Value">Giá trị đo được (đã quy về đơn vị chuẩn).</param>
/// <param name="Unit">Đơn vị của giá trị, ví dụ "% smoke".</param>
/// <param name="Severity">Mức độ cảnh báo, xem <see cref="AlertSeverity"/>.</param>
/// <param name="OccurredAt">Thời điểm cảnh báo xảy ra.</param>
public sealed record AlertEvent(
    string SensorName,
    double Value,
    string Unit,
    string Severity,
    DateTime OccurredAt);
