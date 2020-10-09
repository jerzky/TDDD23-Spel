using UnityEngine;

public class AIWeaponHandler
{
    private readonly AudioSource _audioSource;
    private const float MaxRange = 20f;
    private const float DistanceFromAi = 0.5f;
    private readonly SimpleTimer _shootTimer;
    private readonly AI _ai;
    private readonly int _damage = 50;

    public AIWeaponHandler(AudioSource audioSource, float shootTime)
    {
        _audioSource = audioSource;
        _shootTimer = new SimpleTimer(shootTime);
    }
    /// <summary>This method will return true if does not stand still or if it shoots </summary>
    public bool Shoot(Vector2 aiPosition, Vector2 playerPos)
    { 

        if (!_shootTimer.Tick()) 
            return false;
       
        _shootTimer.Reset();
        var direction = playerPos - aiPosition;

        var startPosition = aiPosition + direction.normalized * DistanceFromAi;

        var finalDirection = playerPos - startPosition;

        float xDis = playerPos.x - startPosition.x;
        float yDis = playerPos.y - startPosition.y;
        float angle = -Mathf.Atan2(xDis, yDis) * 180 / Mathf.PI;

        Bullet.Generate(20f, 30f, _damage, Bullet.ShooterType.AI, finalDirection, startPosition,
            Quaternion.Euler(0f, 0f, angle));
        _audioSource.Play(); 
        return true;
    }


    public void ResetShootTimer()
    {
        _shootTimer.Reset();
    }
}