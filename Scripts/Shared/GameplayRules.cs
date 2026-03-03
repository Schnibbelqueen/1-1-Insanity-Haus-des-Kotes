using System;

namespace Insanity.Scripts.Shared
{
    public enum EnemyKind
    {
        Dummy,
        Melee,
        Ranged,
        Explosive,
    }

    public readonly record struct EnemyConfig(
        EnemyKind Kind,
        int MaxHealth,
        int ContactDamage,
        float MoveSpeed,
        float AggroRange,
        float StopRange,
        float AttackRange,
        float Cooldown,
        float ProjectileSpeed,
        float ExplosionRange,
        int ExplosionDamage
    );

    public readonly record struct EnemyStepResult(float VelocityX, bool ShouldAttack, bool ShouldExplode);

    public static class GameplayRules
    {
        public static float ResolveMoveSpeed(float baseSpeed, float sprintMultiplier, bool sprinting)
        {
            float multiplier = sprinting ? Math.Max(1.0f, sprintMultiplier) : 1.0f;
            return Math.Max(0.0f, baseSpeed) * multiplier;
        }

        public static float ResolveHorizontalVelocity(
            float currentVelocityX,
            float inputX,
            float baseSpeed,
            float sprintMultiplier,
            bool sprinting
        )
        {
            float speed = ResolveMoveSpeed(baseSpeed, sprintMultiplier, sprinting);
            if (Math.Abs(inputX) > 0.001f)
            {
                return Math.Clamp(inputX, -1.0f, 1.0f) * speed;
            }

            return MoveToward(currentVelocityX, 0.0f, speed);
        }

        public static float ResolveFacingDirection(float currentFacing, float inputX)
        {
            if (inputX < -0.001f)
            {
                return -1.0f;
            }

            if (inputX > 0.001f)
            {
                return 1.0f;
            }

            return currentFacing < 0.0f ? -1.0f : 1.0f;
        }

        public static float ResolveShotRotation(float facingDirection)
        {
            return facingDirection < 0.0f ? MathF.PI : 0.0f;
        }

        public static bool CanUseCooldown(float elapsed, float cooldown)
        {
            return elapsed >= Math.Max(0.0f, cooldown);
        }

        public static int ApplyDamage(int currentHealth, int damage)
        {
            return Math.Max(0, currentHealth - Math.Max(0, damage));
        }

        public static int ClampHealth(int health, int maxHealth)
        {
            return Math.Clamp(health, 0, Math.Max(1, maxHealth));
        }

        public static float DistanceX(float selfX, float targetX)
        {
            return Math.Abs(targetX - selfX);
        }

        public static bool IsWithinRange(float selfX, float targetX, float range)
        {
            return DistanceX(selfX, targetX) <= Math.Max(0.0f, range);
        }

        public static float ResolveChaseVelocity(float selfX, float targetX, float moveSpeed, float stopRange)
        {
            float delta = targetX - selfX;
            if (Math.Abs(delta) <= Math.Max(0.0f, stopRange))
            {
                return 0.0f;
            }

            return Math.Sign(delta) * Math.Abs(moveSpeed);
        }

        public static EnemyStepResult StepEnemy(EnemyConfig config, float selfX, float targetX, float cooldownElapsed)
        {
            if (!IsWithinRange(selfX, targetX, config.AggroRange))
            {
                return new EnemyStepResult(0.0f, false, false);
            }

            float velocityX = ResolveChaseVelocity(selfX, targetX, config.MoveSpeed, config.StopRange);
            bool canAttack = IsWithinRange(selfX, targetX, config.AttackRange) && CanUseCooldown(cooldownElapsed, config.Cooldown);
            bool shouldExplode = config.Kind == EnemyKind.Explosive && canAttack;
            return new EnemyStepResult(velocityX, canAttack, shouldExplode);
        }

        private static float MoveToward(float from, float to, float delta)
        {
            if (from < to)
            {
                return Math.Min(from + Math.Abs(delta), to);
            }

            if (from > to)
            {
                return Math.Max(from - Math.Abs(delta), to);
            }

            return to;
        }
    }
}
