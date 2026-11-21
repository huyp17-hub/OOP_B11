/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Alerts;
using CampusSafetyHub.Monitoring;

namespace CampusSafetyHub.Observers;

/// <summary>
/// Observer hiển thị cảnh báo lên bảng điều khiển (mô phỏng bằng Console).
/// [Pattern: Observer] Chỉ đọc <see cref="AlertEvent"/>, không sửa.
/// </summary>
public sealed class DashboardObserver : IAlertObserver
{
    /// <summary>
    /// In cảnh báo ra bảng điều khiển theo đúng định dạng của đề.
    /// </summary>
    public void Update(AlertEvent alertEvent)
    {
        ArgumentNullException.ThrowIfNull(alertEvent);
        Console.WriteLine(
            $"[DASHBOARD] {alertEvent.SensorName}: {alertEvent.Value:F1}{alertEvent.Unit} [{alertEvent.Severity}]");
    }
}
