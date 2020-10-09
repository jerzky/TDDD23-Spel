using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SimpleTimer
{
    private readonly float _max;
    private float _current;
    public bool Done { get { return _current <= 0; } }
    public float Max { get => _max; }
    public float CurrentTime { get => _current; }


    public SimpleTimer(float max)
    {
        _max = max;
        //_current = max;
        _current = 0;
    }

    public bool TickFixedAndReset()
    {
        return TickAndReset(Time.fixedDeltaTime);
    }

    public bool TickAndReset()
    {
        return TickAndReset(Time.deltaTime);
    }

    public bool TickAndReset(float time)
    {
        if(Tick(time))
        {
            Reset();
            return true;
        }
        return false;
    }

    public bool Tick()
    {
        return Tick(Time.deltaTime);
    }

    public bool TickFixed()
    {
        return Tick(Time.fixedDeltaTime);
    }

    public bool Tick(float time)
    {
        _current -= time;
        return _current <= 0;
    }

    public void Reset()
    {
        ResetTo(_max);
    }

    public void ResetTo(float time)
    {
        _current = time;
    }

    
}