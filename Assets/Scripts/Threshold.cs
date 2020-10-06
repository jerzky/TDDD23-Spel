public class Threshold
{
    private readonly float _thresholdMax;
    private readonly float _thresholdMin;
    private bool _currentValue;

    public Threshold(float thresholdMax, float thresholdMin)
    {
        _thresholdMax = thresholdMax;
        _thresholdMin = thresholdMin;
    }


    public bool GetValue(float f)
    {
        if (f >= _thresholdMax) _currentValue = true;
        if (f <= _thresholdMin) _currentValue = false;
        return _currentValue;
    }
}