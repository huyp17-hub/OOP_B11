/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Sensors;

/// <summary>
/// Bộ chuyển đổi để cảm biến khói đời cũ dùng được qua giao diện mới.
/// [Pattern: Adapter] Đây là object adapter: lớp này CHỨA một
/// <see cref="LegacySmokeDetector"/> (không kế thừa), nhờ vậy giấu được
/// API cũ và có thể thay bằng một thiết bị cũ khác.
/// </summary>
public sealed class SmokeSensorAdapter : ISensor
{
    private readonly LegacySmokeDetector _legacyDetector;

    /// <summary>
    /// Tạo adapter bọc quanh một cảm biến đời cũ.
    /// </summary>
    /// <param name="legacyDetector">Cảm biến cũ cần được chuyển đổi; không được null.</param>
    /// <exception cref="ArgumentNullException">Khi <paramref name="legacyDetector"/> là null.</exception>
    public SmokeSensorAdapter(LegacySmokeDetector legacyDetector)
    {
        _legacyDetector = legacyDetector ?? throw new ArgumentNullException(nameof(legacyDetector));
    }

    /// <summary>
    /// Đọc giá trị khói đã quy về phần trăm (thang 0..1000 chia cho 10).
    /// Mỗi lần gọi đều đọc lại từ cảm biến cũ, không lưu bản sao trạng thái.
    /// </summary>
    public double ReadValue()
    {
        return _legacyDetector.GetSmokeLevel() / 10.0;
    }

    /// <summary>
    /// Đơn vị của giá trị trả về là phần trăm khói.
    /// </summary>
    public string GetUnit()
    {
        return "% smoke";
    }
}
