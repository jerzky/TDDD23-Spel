using UnityEngine;

public class AIWeaponHandler
{
    private const float AIWeaponVelocity = 20f;
    private const float MaxRange = 20f;
    private const float DistanceFromAi = 0.5f;
    private readonly AI _ai;
    private readonly AudioSource _audioSource;
    private readonly int _damage = 50;
    private readonly SimpleTimer _shootTimer;

    public AIWeaponHandler(AudioSource audioSource, float shootTime)
    {
        _audioSource = audioSource;
        _shootTimer = new SimpleTimer(shootTime);
    }

    public bool ShootWithAimbot(Vector2 aiPosition, Vector2 playerPos, bool ignoreTimer = false)
    {
        if (!_shootTimer.Tick() && !ignoreTimer)
            return false;

        if (!ignoreTimer)
            _shootTimer.Reset();

        var playerDir = PlayerController.Instance.MovementDirection;
        var playerSpeed = PlayerController.Instance.CurrentSpeed;
        var direction = playerPos - aiPosition;
        var bulletLocation = aiPosition + direction.normalized * DistanceFromAi;


        var targetOffset = playerPos - bulletLocation;

        // The distance from the missile to the target
        var targetDistance = targetOffset.magnitude;

        // Normalize the offset vector into a direction - same as doing TargetOffset.normalized
        var targetDirection = targetOffset / targetDistance;

        // How fast the target and missle are moving relative to one another (if the missile
        // hasn't been fired yet, use TargetDirection * FiringSpeed for his velocity)
        // Another way to think of this is how fast the missile would be moving relative to
        // the target if the target wasn't moving at all
        var relativeVelocity = AIWeaponVelocity * targetDirection.normalized - playerSpeed * playerDir.normalized;

        // How fast the target is moving away from the missile
        var relativeSpeed = Vector2.Dot(relativeVelocity, targetDirection);

        // If RelativeSpeed is negative, that means the target is traveling faster than the
        // missile and the missile cannot catch up to it.
        // For this case, you can just fake an estimated intercept time so the missile at
        // least makes a decent attempt

        float interceptTime;
        if (relativeSpeed <= 0.0)
            interceptTime = 1.0f;
        else
            interceptTime = targetDistance / relativeSpeed;

        // We now have an estimate of how long it will take our missile to catch up to the
        // target - plug it in to his physics equation to predict where the target will be.
        var interceptLocation = playerPos + playerSpeed * playerDir.normalized * interceptTime;

        // Aim the missile towards this location
        var aimDirection = (interceptLocation - bulletLocation).normalized;


        var xDis = interceptLocation.x - bulletLocation.x;
        var yDis = interceptLocation.y - bulletLocation.y;
        var angle = -Mathf.Atan2(xDis, yDis) * 180 / Mathf.PI;

        Bullet.Generate(AIWeaponVelocity, 30f, _damage, Bullet.ShooterType.AI, aimDirection, bulletLocation,
            Quaternion.Euler(0f, 0f, angle));
        _audioSource.Play();
        return true;
    }


    /// <summary>This method will return true if does not stand still or if it shoots </summary>
    public bool Shoot(Vector2 aiPosition, Vector2 playerPos, bool ignoreTimer = false)
    {
        return ShootWithAimbot(aiPosition, playerPos, ignoreTimer);

        if (!_shootTimer.Tick() && !ignoreTimer)
            return false;

        if (!ignoreTimer)
            _shootTimer.Reset();

        var direction = playerPos - aiPosition;

        var startPosition = aiPosition + direction.normalized * DistanceFromAi;

        var finalDirection = playerPos - startPosition;

        var xDis = playerPos.x - startPosition.x;
        var yDis = playerPos.y - startPosition.y;
        var angle = -Mathf.Atan2(xDis, yDis) * 180 / Mathf.PI;

        Bullet.Generate(AIWeaponVelocity, 30f, _damage, Bullet.ShooterType.AI, finalDirection, startPosition,
            Quaternion.Euler(0f, 0f, angle));
        _audioSource.Play();
        return true;
    }


    public void ResetShootTimer()
    {
        _shootTimer.Reset();
    }
}