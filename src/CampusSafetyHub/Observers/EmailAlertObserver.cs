/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Alerts;
using CampusSafetyHub.Monitoring;

namespace CampusSafetyHub.Observers;

/// <summary>
/// Observer gửi email cảnh báo (chỉ mô phỏng, không gửi email thật).
/// [Pattern: Observer] Chỉ đọc <see cref="AlertEvent"/>.
/// Có thể cấu hình mức độ tối thiểu để gửi; mặc định gửi cả WARNING và CRITICAL.
/// </summary>
public sealed class EmailAlertObserver : IAlertObserver
{
    private readonly string _to;
    private readonly string _minSeverity;

    /// <summary>
    /// Tạo observer email.
    /// </summary>
    /// <param name="to">Địa chỉ người nhận; không được rỗng.</param>
    /// <param name="minSeverity">
    /// Mức độ tối thiểu để gửi email; mặc định <see cref="AlertSeverity.Warning"/>
    /// (gửi cả WARNING lẫn CRITICAL).
    /// </param>
    /// <exception cref="ArgumentException">Khi <paramref name="to"/> rỗng hoặc chỉ gồm khoảng trắng.</exception>
    public EmailAlertObserver(string to, string minSeverity = AlertSeverity.Warning)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentException("Địa chỉ người nhận không được rỗng.", nameof(to));
        }

        _to = to;
        _minSeverity = minSeverity;
    }

    /// <summary>
    /// Gửi (mô phỏng) email nếu mức độ cảnh báo đạt ngưỡng tối thiểu đã cấu hình.
    /// </summary>
    public void Update(AlertEvent alertEvent)
    {
        ArgumentNullException.ThrowIfNull(alertEvent);

        if (Rank(alertEvent.Severity) < Rank(_minSeverity))
        {
            return;
        }

        Console.WriteLine(
            $"[EMAIL] To: {_to} | Subject: [{alertEvent.Severity}] {alertEvent.SensorName} | " +
            $"{alertEvent.Value:F1}{alertEvent.Unit} lúc {alertEvent.OccurredAt:HH:mm:ss}");
    }

    /// <summary>
    /// Quy mức độ cảnh báo thành số để so sánh: CRITICAL cao hơn WARNING.
    /// Giá trị không xác định coi như thấp nhất.
    /// </summary>
    private static int Rank(string severity) => severity switch
    {
        AlertSeverity.Critical => 2,
        AlertSeverity.Warning => 1,
        _ => 0,
    };
}
