/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Sensors;

/// <summary>
/// Cảm biến khói đời cũ có sẵn từ hệ thống trước đây.
/// LỚP NÀY CÓ SẴN, KHÔNG ĐƯỢC SỬA (chỉ thêm header sinh viên).
/// Nó đo mức khói theo thang nội bộ 0..1000 và có API riêng,
/// không trùng với giao diện <see cref="ISensor"/> của hệ thống mới.
/// </summary>
public class LegacySmokeDetector
{
    private int _smokeLevel;

    /// <summary>
    /// Lấy mức khói hiện tại theo thang cũ 0..1000.
    /// </summary>
    public int GetSmokeLevel()
    {
        return _smokeLevel;
    }

    /// <summary>
    /// Đặt mức khói theo thang cũ; giá trị bị kẹp về khoảng 0..1000.
    /// </summary>
    public void SetSmokeLevel(int level)
    {
        _smokeLevel = Math.Clamp(level, 0, 1000);
    }

    /// <summary>
    /// Mô tả trạng thái bằng văn bản theo cách của thiết bị cũ.
    /// </summary>
    public string GetDescription()
    {
        if (_smokeLevel < 500)
        {
            return "Low smoke";
        }

        if (_smokeLevel < 700)
        {
            return "Medium smoke";
        }

        return "High smoke";
    }
}
