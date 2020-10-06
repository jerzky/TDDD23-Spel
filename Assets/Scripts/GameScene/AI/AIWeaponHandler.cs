using UnityEngine;

public class AIWeaponHandler
{
    private readonly AudioSource _audioSource;
    private const float MaxRange = 20f;
    private const float DistanceFromAi = 1.5f;
    private readonly SimpleTimer _haltTimer = new SimpleTimer(2f);
    private readonly SimpleTimer _shootTimer = new SimpleTimer(0.5f);
    private readonly AI _ai;
    private readonly int _damage = 50;

    public AIWeaponHandler(AudioSource audioSource)
    {
        _audioSource = audioSource;
    }
    /// <summary>This method will return true if does not stand still or if it shoots </summary>
    public bool Shoot(Vector2 aiPosition, Vector2 playerPos, bool needsToHalt = true)
    {
        if (!_haltTimer.Tick() && needsToHalt)
        {
            _shootTimer.Reset();
            return true;
        }

        

        if (!_shootTimer.Tick()) 
            return false;
       
        if (!needsToHalt) _shootTimer.Reset();
        var direction = playerPos - aiPosition;

        var startPosition = aiPosition + direction.normalized * DistanceFromAi;

        var finalDirection = playerPos - startPosition;

        Bullet.Generate(20f, 30f, _damage, Bullet.ShooterType.AI, finalDirection, startPosition,
            Quaternion.FromToRotation(startPosition, finalDirection));
        _audioSource.Play();

        _haltTimer.Reset();
      
        
        return false;
    }
}