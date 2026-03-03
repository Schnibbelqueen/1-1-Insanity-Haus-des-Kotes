using Godot;

namespace Insanity.Scripts.Enemies
{
    public partial class EnemyProjectile : Area2D
    {
        [Export] private float _speed = 260.0f;
        [Export] private float _lifetime = 4.0f;

        private Vector2 _direction = Vector2.Right;
        private float _age;

        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }

        public override void _PhysicsProcess(double delta)
        {
            _age += (float)delta;
            GlobalPosition += _direction * _speed * (float)delta;

            if (_age >= _lifetime)
            {
                QueueFree();
            }
        }

        public void Initialize(Vector2 direction, float speed)
        {
            if (direction.LengthSquared() > 0.0f)
            {
                _direction = direction.Normalized();
            }

            if (speed > 0.0f)
            {
                _speed = speed;
            }
        }

        private void OnBodyEntered(Node2D body)
        {
            QueueFree();
        }
    }
}
