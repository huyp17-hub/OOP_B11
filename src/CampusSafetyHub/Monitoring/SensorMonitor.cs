/*************************************************
Mã sinh viên: 202418915
Họ tên:       Phạm Quốc Huy
*************************************************/

using CampusSafetyHub.Alerts;
using CampusSafetyHub.Sensors;

namespace CampusSafetyHub.Monitoring;

/// <summary>
/// Trung tâm giám sát: đọc cảm biến, phân loại ngưỡng rồi phát cảnh báo.
/// [Pattern: Observer] Lớp này là subject (chủ thể) giữ danh sách các
/// <see cref="IAlertObserver"/> và thông báo cho họ khi có cảnh báo.
/// Nó chỉ biết tới interface, không biết observer cụ thể nào.
/// </summary>
public sealed class SensorMonitor
{
    private readonly List<IAlertObserver> _observers = new();
    private readonly AlertThresholds _thresholds;
    private readonly TextWriter _errorWriter;

    /// <summary>
    /// Tạo trung tâm giám sát.
    /// </summary>
    /// <param name="thresholds">Bộ ngưỡng cảnh báo; mặc định 50/70 theo đề.</param>
    /// <param name="errorWriter">Nơi ghi lỗi khi observer ném ngoại lệ; mặc định <see cref="Console.Error"/>.</param>
    public SensorMonitor(AlertThresholds? thresholds = null, TextWriter? errorWriter = null)
    {
        _thresholds = thresholds ?? new AlertThresholds();
        _errorWriter = errorWriter ?? Console.Error;
    }

    /// <summary>Số observer đang đăng ký.</summary>
    public int ObserverCount => _observers.Count;

    /// <summary>
    /// Đăng ký một observer. Không cho đăng ký trùng cùng một đối tượng.
    /// </summary>
    /// <exception cref="ArgumentNullException">Khi <paramref name="observer"/> là null.</exception>
    public void Attach(IAlertObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    /// <summary>
    /// Hủy đăng ký một observer. Gỡ observer chưa đăng ký không gây lỗi.
    /// </summary>
    /// <returns>True nếu có gỡ được, False nếu observer vốn không có trong danh sách.</returns>
    /// <exception cref="ArgumentNullException">Khi <paramref name="observer"/> là null.</exception>
    public bool Detach(IAlertObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        return _observers.Remove(observer);
    }

    /// <summary>
    /// Đọc một cảm biến, phân loại ngưỡng và phát cảnh báo nếu vượt ngưỡng.
    /// </summary>
    /// <param name="sensorName">Tên cảm biến (không được null).</param>
    /// <param name="sensor">Cảm biến cần đọc (không được null).</param>
    /// <returns>
    /// Cảnh báo đã phát, hoặc null nếu giá trị dưới ngưỡng (không phát).
    /// Trả về cảnh báo để tiện cho việc kiểm thử.
    /// </returns>
    /// <exception cref="ArgumentNullException">Khi tham số là null.</exception>
    public AlertEvent? CheckSensor(string sensorName, ISensor sensor)
    {
        ArgumentNullException.ThrowIfNull(sensorName);
        ArgumentNullException.ThrowIfNull(sensor);

        double value = sensor.ReadValue();
        string? severity = _thresholds.Classify(value);
        if (severity is null)
        {
            return null;
        }

        AlertEvent alertEvent = new(sensorName, value, sensor.GetUnit(), severity, DateTime.Now);
        Notify(alertEvent);
        return alertEvent;
    }

    /// <summary>
    /// Thông báo cảnh báo cho toàn bộ observer. Duyệt trên bản sao danh sách
    /// để observer có thể tự gỡ chính mình trong lúc nhận. Mỗi observer
    /// được bọc try/catch riêng: một observer lỗi thì ghi log và vẫn tiếp tục
    /// thông báo cho các observer còn lại.
    /// </summary>
    private void Notify(AlertEvent alertEvent)
    {
        foreach (IAlertObserver observer in _observers.ToArray())
        {
            try
            {
                observer.Update(alertEvent);
            }
            catch (Exception ex)
            {
                _errorWriter.WriteLine(
                    $"[MONITOR] Observer {observer.GetType().Name} lỗi: {ex.Message}");
            }
        }
    }
}
