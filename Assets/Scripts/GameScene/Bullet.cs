using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    public enum ShooterType
    {
        AI,
        Player
    }


    private float _maxTravelDistance;
    private float _muzzleVelocity;
    private int _damage = 50;
    bool _bulletInfoSet = false;
    public Vector2 BulletDirection { get; set; }

    public Vector3 StartingPosition { get ; set; }
    public float BulletTravelDistance { get; private set; }

    public ShooterType Shooter { get; private set; }


    public bool HasExpired => BulletTravelDistance >= _maxTravelDistance;


    // Start is called before the first frame update
    void Start()
    {
        //var temp = WeaponController.Instance.WeaponEnd.transform.position - PlayerController.Instance.transform.position;
       // BulletDirection = new Vector2(temp.x, temp.y);
        StartingPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_bulletInfoSet)
            return;
        //transform.
        GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y) + BulletDirection.normalized * Time.deltaTime * _muzzleVelocity);
     
        if (Mathf.Abs(Vector3.Distance(StartingPosition, transform.position)) > _maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("humanoid"))
        {
            if(Shooter == ShooterType.Player)
                collision.collider.GetComponent<AI>().Injure(_damage, new Vector3(BulletDirection.x, BulletDirection.y, 0f));
        }
        else if(collision.collider.CompareTag("Player"))
        {
            PlayerController.Instance.Injure(_damage);
        }
        Destroy(gameObject);
    }

    public void SetBulletInfo(float muzzleVelocity, float maxTravelDistance, int damage, Vector2 bulletDirection, ShooterType shooter)
    {
        _muzzleVelocity = muzzleVelocity;
        _maxTravelDistance = maxTravelDistance;
        _damage = damage;
        Shooter = shooter;
        BulletDirection = bulletDirection;
        Debug.Log(muzzleVelocity + " " + maxTravelDistance);
        _bulletInfoSet = true;
    }



    private static GameObject _standardPrefab;
    
    
    public static GameObject Generate(float muzzleVelocity, float bulletMaxTravelDistance, int damage, ShooterType shooter, Vector2 direction, Vector2 position, Quaternion rotation)
    {
        if(_standardPrefab == null)
            _standardPrefab = Resources.Load<GameObject>("Prefabs/bullet");

        var bullet = Instantiate(_standardPrefab, position, rotation, WeaponController.Instance.bulletHolder.transform);

        bullet.GetComponent<Bullet>().SetBulletInfo(muzzleVelocity, bulletMaxTravelDistance, damage, direction, shooter);
        return bullet;
    }

}
