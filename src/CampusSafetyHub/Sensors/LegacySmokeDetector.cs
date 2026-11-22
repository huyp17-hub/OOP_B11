/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

namespace CampusSafetyHub.Sensors;

/// <summary>
/// Cảm biến khói đời cũ có sẵn từ hệ thống trước đây.
/// LỚP NÀY CÓ SẴN, KHÔNG ĐƯỢC SỬA: giữ nguyên code của đề, chỉ thêm header sinh viên
/// và comment. Nó đo mức khói theo thang nội bộ 0..1000 và có API riêng,
/// không trùng với giao diện <see cref="ISensor"/> của hệ thống mới.
/// </summary>
public sealed class LegacySmokeDetector
{
    private int smokeLevel;

    public LegacySmokeDetector(int smokeLevel)
    {
        this.smokeLevel = smokeLevel;
    }

    public int GetSmokeLevel() => smokeLevel; // 0..1000

    public string GetDescription() => "Legacy smoke detector";

    public void SetSmokeLevel(int value)
    {
        smokeLevel = Math.Clamp(value, 0, 1000);
    }
}
