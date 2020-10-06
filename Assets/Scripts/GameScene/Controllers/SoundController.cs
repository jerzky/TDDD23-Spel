using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateSound(Sound sound)
    {
        var colliders = Physics2D.OverlapCircleAll(sound.origin, sound.radius);
        ((GameObject)Instantiate(Resources.Load("Prefabs/soundcircle"), sound.origin, Quaternion.identity, null)).GetComponent<soundcircletest>().Radius = sound.radius;

        foreach(var v in colliders)
        {
            if(v.CompareTag("humanoid"))
            {
                v.GetComponent<AI>().Alert(sound);
            }
        }
    }
}
