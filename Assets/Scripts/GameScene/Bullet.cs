using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    private float maxTravelDistance;
    private float muzzleVelocity;
    public Vector2 BulletDirection { get; set; }

    public Vector3 StartingPosition { get ; set; }
    public float BulletTravelDistance { get; private set; }
    public bool HasExpired { get { return BulletTravelDistance >= maxTravelDistance; } }
    bool bulletInfoSet = false;
    int damage = 50;
    // Start is called before the first frame update
    void Start()
    {
        var temp = WeaponController.Instance.WeaponEnd.transform.position - PlayerController.Instance.transform.position;
        BulletDirection = new Vector2(temp.x, temp.y);
        StartingPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!bulletInfoSet)
            return;
        //transform.
        GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y) + BulletDirection.normalized * Time.deltaTime * muzzleVelocity);
     
        if (Mathf.Abs(Vector3.Distance(StartingPosition, transform.position)) > maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("humanoid"))
        {
            collision.collider.GetComponent<AI>().Injure(damage, new Vector3(BulletDirection.x, BulletDirection.y, 0f));
        }
        else if(collision.collider.CompareTag("Player"))
        {
            PlayerController.Instance.Injure(damage);
        }
        Destroy(gameObject);
    }

    public void SetBulletInfo(float muzzleVelocity, float maxTravelDistance, int damage)
    {
        this.muzzleVelocity = muzzleVelocity;
        this.maxTravelDistance = maxTravelDistance;
        this.damage = damage;
        Debug.Log(muzzleVelocity + " " + maxTravelDistance);
        bulletInfoSet = true;
    }

}
