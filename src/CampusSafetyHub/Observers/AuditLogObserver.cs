/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Alerts;
using CampusSafetyHub.Monitoring;

namespace CampusSafetyHub.Observers;

/// <summary>
/// Observer ghi nhật ký kiểm toán: lưu mỗi cảnh báo thành một dòng log.
/// [Pattern: Observer] Chỉ đọc <see cref="AlertEvent"/>.
/// </summary>
public sealed class AuditLogObserver : IAlertObserver
{
    private readonly List<string> _entries = new();

    /// <summary>Các dòng log đã ghi, chỉ cho đọc để phục vụ kiểm thử.</summary>
    public IReadOnlyList<string> Entries => _entries;

    /// <summary>
    /// Ghi cảnh báo vào nhật ký nội bộ và in ra màn hình.
    /// </summary>
    public void Update(AlertEvent alertEvent)
    {
        ArgumentNullException.ThrowIfNull(alertEvent);

        string line =
            $"{alertEvent.OccurredAt:HH:mm:ss} {alertEvent.SensorName} " +
            $"{alertEvent.Value:F1}{alertEvent.Unit} [{alertEvent.Severity}]";
        _entries.Add(line);
        Console.WriteLine($"[AUDIT] {line}");
    }
}
