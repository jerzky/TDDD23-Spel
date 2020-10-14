using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class soundcircletest : MonoBehaviour
{
    private float speed = 35f;
    private float radius = 25f;
    private bool hasStarted = false;
    public float Radius 
    { 
        get 
        {
            return radius; 
        } 
        set 
        { 
            speed = value + 10f; 
            radius = value; 
            hasStarted = true;
            if (radius < 2.5f)
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("NoSpriteAtlasTextures/soundcircleblack10");
        } 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
            return;
        transform.localScale += new Vector3(1f, 1f, 0f) * speed * Time.deltaTime;
        if (transform.localScale.x > radius*2)
            Destroy(gameObject);
    }
}
