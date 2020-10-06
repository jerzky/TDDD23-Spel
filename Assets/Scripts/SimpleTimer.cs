using UnityEngine;

public class SimpleTimer
{
    private readonly float _max;
    private float _current;

    public SimpleTimer(float max)
    {
        _max = max;
        _current = max;
    }

    public bool Tick()
    {
        _current -= Time.deltaTime;
        return _current <= 0;
    }

    public void Reset()
    {
        _current = _max;
    }
}