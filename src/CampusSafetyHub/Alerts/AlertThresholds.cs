/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Alerts;

/// <summary>
/// Nơi DUY NHẤT quyết định một giá trị khói thuộc mức cảnh báo nào.
/// Observer không được tự ý quyết định ngưỡng; mọi phân loại đi qua đây.
/// </summary>
public sealed class AlertThresholds
{
    /// <summary>Ngưỡng bắt đầu cảnh báo WARNING (mặc định 50%).</summary>
    public double WarningThreshold { get; }

    /// <summary>Ngưỡng bắt đầu cảnh báo CRITICAL (mặc định 70%).</summary>
    public double CriticalThreshold { get; }

    /// <summary>
    /// Tạo bộ ngưỡng. Mặc định theo đề: WARNING = 50, CRITICAL = 70.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Khi ngưỡng CRITICAL nhỏ hơn ngưỡng WARNING.
    /// </exception>
    public AlertThresholds(double warningThreshold = 50, double criticalThreshold = 70)
    {
        if (criticalThreshold < warningThreshold)
        {
            throw new ArgumentException(
                "Ngưỡng CRITICAL phải lớn hơn hoặc bằng ngưỡng WARNING.",
                nameof(criticalThreshold));
        }

        WarningThreshold = warningThreshold;
        CriticalThreshold = criticalThreshold;
    }

    /// <summary>
    /// Phân loại giá trị khói thành mức cảnh báo.
    /// Trả về null nếu dưới ngưỡng (không cần phát cảnh báo),
    /// ngược lại trả về <see cref="AlertSeverity.Warning"/> hoặc
    /// <see cref="AlertSeverity.Critical"/>.
    /// </summary>
    public string? Classify(double value)
    {
        if (value >= CriticalThreshold)
        {
            return AlertSeverity.Critical;
        }

        if (value >= WarningThreshold)
        {
            return AlertSeverity.Warning;
        }

        return null;
    }
}
