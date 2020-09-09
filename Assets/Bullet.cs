using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    private const float MAX_TRAVEL_DISTANCE = 10f;
    private const float BULLET_SPEED = 20f;
    public Vector2 BulletDirection { get; set; }

    public Vector3 StartingPosition { get ; set; }
    public float BulletTravelDistance { get; private set; }
    public bool HasExpired { get { return BulletTravelDistance >= MAX_TRAVEL_DISTANCE; } }

    // Start is called before the first frame update
    void Start()
    {
        var temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        BulletDirection = new Vector2(temp.x, temp.y);
        //Debug.Log(BulletDirection.normalized);
        StartingPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.
        GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y) + BulletDirection.normalized * Time.deltaTime * BULLET_SPEED);
     
        if (Mathf.Abs(Vector3.Distance(StartingPosition, transform.position)) > MAX_TRAVEL_DISTANCE )
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

}
