using Godot;
using Insanity.Scripts.Shared;

namespace Insanity.Scripts.Enemies
{
    public partial class RangedEnemy : EnemyBody2D
    {
        [Export] private float _moveSpeed = 60.0f;
        [Export] private float _aggroRange = 520.0f;
        [Export] private float _stopRange = 200.0f;
        [Export] private float _attackRange = 420.0f;
        [Export] private float _attackCooldown = 1.25f;
        [Export] private float _projectileSpeed = 300.0f;
        [Export] private PackedScene _projectileScene;

        private float _timeSinceAttack = 99.0f;

        public override void _PhysicsProcess(double delta)
        {
            _timeSinceAttack += (float)delta;
            Vector2 velocity = ApplyBasicGravity(delta, Velocity);
            var player = GetPlayer();

            if (player != null)
            {
                EnemyStepResult step = GameplayRules.StepEnemy(
                    new EnemyConfig(EnemyKind.Ranged, MaxHealth, 4, _moveSpeed, _aggroRange, _stopRange, _attackRange, _attackCooldown, _projectileSpeed, 0.0f, 0),
                    GlobalPosition.X,
                    player.GlobalPosition.X,
                    _timeSinceAttack
                );

                velocity.X = step.VelocityX;
                if (step.ShouldAttack)
                {
                    _timeSinceAttack = 0.0f;
                    SpawnProjectile(player.GlobalPosition);
                }
            }
            else
            {
                velocity.X = 0.0f;
            }

            Velocity = velocity;
            MoveAndSlide();
        }

        private void SpawnProjectile(Vector2 targetPosition)
        {
            if (_projectileScene == null)
            {
                return;
            }

            var instance = _projectileScene.Instantiate();
            if (instance is EnemyProjectile projectile)
            {
                projectile.GlobalPosition = GlobalPosition + new Vector2(0.0f, -16.0f);
                projectile.Initialize(targetPosition - projectile.GlobalPosition, _projectileSpeed);
                GetTree().CurrentScene.AddChild(projectile);
            }
            else
            {
                instance.QueueFree();
            }
        }
    }
}
