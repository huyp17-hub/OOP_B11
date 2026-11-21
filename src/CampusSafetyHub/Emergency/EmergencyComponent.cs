/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Emergency;

/// <summary>
/// Lớp cơ sở cho mọi thiết bị ứng phó (còi, cửa, thông gió).
/// [Pattern: Mediator] Mỗi thiết bị chỉ giữ tham chiếu tới
/// <see cref="IEmergencyMediator"/>, KHÔNG giữ tham chiếu tới thiết bị khác.
/// Mọi phối hợp giữa các thiết bị đều đi qua trung gian.
/// </summary>
public abstract class EmergencyComponent
{
    /// <summary>Trung gian điều phối; được gán qua <see cref="SetMediator"/>.</summary>
    protected IEmergencyMediator? Mediator { get; private set; }

    /// <summary>
    /// Gán trung gian điều phối cho thiết bị này.
    /// </summary>
    /// <param name="mediator">Trung gian; không được null.</param>
    /// <exception cref="ArgumentNullException">Khi <paramref name="mediator"/> là null.</exception>
    public void SetMediator(IEmergencyMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        Mediator = mediator;
    }
}
