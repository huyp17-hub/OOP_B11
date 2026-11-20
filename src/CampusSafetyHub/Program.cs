/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Sensors;

namespace CampusSafetyHub;

/// <summary>
/// Điểm vào của chương trình Campus Safety Hub.
/// Hiện tại mới demo phần Adapter; kịch bản demo đầy đủ và bộ test
/// sẽ được bổ sung ở các task sau (Ngày 2 và Ngày 3).
/// </summary>
internal static class Program
{
    /// <summary>
    /// Hàm main: tạm thời demo nhanh bộ chuyển đổi cảm biến.
    /// </summary>
    private static void Main()
    {
        LegacySmokeDetector legacy = new();
        legacy.SetSmokeLevel(735);

        ISensor sensor = new SmokeSensorAdapter(legacy);
        Console.WriteLine($"Giá trị đọc qua ISensor: {sensor.ReadValue():F1}{sensor.GetUnit()}");
    }
}
