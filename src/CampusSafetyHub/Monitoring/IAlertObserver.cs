/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Alerts;

namespace CampusSafetyHub.Monitoring;

/// <summary>
/// Bên nhận cảnh báo trong mẫu Observer. Mỗi observer đăng ký với
/// <see cref="SensorMonitor"/> và được gọi lại khi có cảnh báo mới.
/// </summary>
public interface IAlertObserver
{
    /// <summary>
    /// Được gọi khi có cảnh báo mới. Observer chỉ nên đọc dữ liệu cảnh báo.
    /// </summary>
    void Update(AlertEvent alertEvent);
}
