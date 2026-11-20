/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Sensors;

/// <summary>
/// Giao diện cảm biến chuẩn mới mà cả hệ thống dựa vào.
/// Mọi cảm biến (kể cả cảm biến đời cũ thông qua Adapter) đều phải
/// cung cấp một giá trị đọc được và đơn vị của giá trị đó.
/// </summary>
public interface ISensor
{
    /// <summary>
    /// Đọc giá trị hiện tại của cảm biến (đã quy về đơn vị chuẩn).
    /// </summary>
    double ReadValue();

    /// <summary>
    /// Trả về đơn vị của giá trị đọc được, ví dụ "% smoke".
    /// </summary>
    string GetUnit();
}
