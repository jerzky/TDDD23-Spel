using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sound
{
    public enum SoundType { Human, Weapon, Construction }
    public Vector2 origin;
    public float radius;
    public SoundType soundType;
    public Sound(Vector2 origin, float radius, SoundType soundType)
    {
        this.origin = origin;
        this.radius = radius;
        this.soundType = soundType;
    }
}



public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    SimpleTimer timer = new SimpleTimer(0.149f);

    private class ContinousSound
    {
        public SimpleTimer timer;
        public float timesLeft;
        public Sound sound;
        public ContinousSound(Sound sound, float timesLeft, SimpleTimer timer)
        {
            this.sound = sound;
            this.timesLeft = timesLeft;
            this.timer = timer;
        }

    }

    Dictionary<uint, ContinousSound> continousSounds = new Dictionary<uint, ContinousSound>();
    uint continousSoundsIDcounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        timer.Tick();
        continousSounds.ToList().RemoveAll(RemoveAndRun);
    }

    private bool RemoveAndRun(KeyValuePair<uint, ContinousSound> val)
    {
        if (!val.Value.timer.TickAndReset()) 
            return false;
        GenerateSound(val.Value.sound);
        return --val.Value.timesLeft <= 0;
    }
    public void GenerateSound(Sound sound)
    {
        var colliders = Physics2D.OverlapCircleAll(sound.origin, sound.radius);

        foreach(var v in colliders)
        {
            if(v.CompareTag("humanoid"))
            {
                v.GetComponent<AI>().Alert(sound);
            }
        }

        if (!timer.Done)
            return;
        timer.Reset();
        ((GameObject)Instantiate(Resources.Load("Prefabs/soundcircle"), sound.origin, Quaternion.identity, null)).GetComponent<soundcircletest>().Radius = sound.radius;
    }

    public uint GenerateContinousSound(Sound sound, float time = Mathf.Infinity, float interval = 0.5f)
    {
        GenerateSound(sound);
        continousSounds.Add(continousSoundsIDcounter, new ContinousSound(sound, (time / interval), new SimpleTimer(interval)));
        return continousSoundsIDcounter++;
    }

    public void CancelContinousSound(uint ID)
    {
        continousSounds.Remove(ID);
    }
}
