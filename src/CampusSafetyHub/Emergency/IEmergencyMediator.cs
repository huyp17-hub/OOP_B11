/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Emergency;

/// <summary>
/// Giao diện của trung gian điều phối ứng phó khẩn cấp.
/// [Pattern: Mediator] Các thiết bị chỉ biết tới interface này, không biết
/// nhau. Khi một thiết bị (hoặc cầu nối từ observer) có sự kiện thì gọi
/// <see cref="Notify"/>, trung gian sẽ quyết định điều phối các thiết bị khác.
/// </summary>
public interface IEmergencyMediator
{
    /// <summary>
    /// Nhận một sự kiện khẩn cấp từ một bên gửi và điều phối ứng phó.
    /// </summary>
    /// <param name="sender">Đối tượng phát ra sự kiện.</param>
    /// <param name="eventCode">Mã sự kiện, xem <see cref="EmergencyEvents"/>.</param>
    void Notify(object sender, string eventCode);
}
