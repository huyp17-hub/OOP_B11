/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Alerts;
using CampusSafetyHub.Emergency;
using CampusSafetyHub.Monitoring;

namespace CampusSafetyHub.Observers;

/// <summary>
/// Cầu nối giữa hệ thống cảnh báo (Observer) và hệ thống ứng phó (Mediator).
/// [Pattern: Observer + Mediator] Là observer nhận cảnh báo, nhưng CHỈ chuyển
/// cảnh báo mức CRITICAL thành sự kiện <see cref="EmergencyEvents.FireDetected"/>
/// cho <see cref="IEmergencyMediator"/>. Nó phụ thuộc vào interface của mediator
/// chứ không phải lớp cụ thể.
/// </summary>
public sealed class EmergencyBridgeObserver : IAlertObserver
{
    private readonly IEmergencyMediator _mediator;

    /// <summary>
    /// Tạo cầu nối tới một trung gian ứng phó.
    /// </summary>
    /// <param name="mediator">Trung gian điều phối; không được null.</param>
    /// <exception cref="ArgumentNullException">Khi <paramref name="mediator"/> là null.</exception>
    public EmergencyBridgeObserver(IEmergencyMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        _mediator = mediator;
    }

    /// <summary>
    /// Chuyển cảnh báo CRITICAL thành sự kiện cháy; các mức khác bỏ qua.
    /// </summary>
    public void Update(AlertEvent alertEvent)
    {
        ArgumentNullException.ThrowIfNull(alertEvent);

        if (alertEvent.Severity == AlertSeverity.Critical)
        {
            _mediator.Notify(this, EmergencyEvents.FireDetected);
        }
    }
}
