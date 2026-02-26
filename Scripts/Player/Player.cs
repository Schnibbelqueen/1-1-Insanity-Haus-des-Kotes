using Godot;
using System;

namespace Insanity.Scripts.Player
{
    // Code-Autoren; Bitte bei Änderungen eintragen
    // CODED BY FREDEUN
    //
    public partial class Player : CharacterBody2D
    {
        [Export] private float _speed = 300.0f;
        [Export] private float _jumpVelocity = -400.0f;

        private Vector2 direction;
    
        public override void _PhysicsProcess(double delta)
        {
            Vector2 velocity = Velocity;
    
            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }
    
            // Handle Jump.
            if (Input.IsActionJustPressed("jump") && IsOnFloor())
            {
                velocity.Y = _jumpVelocity;
            }
    
            // Get the input direction and handle the movement/deceleration.
            // As good practice, you should replace UI actions with custom gameplay actions.
            direction = Input.GetVector("left", "right", "up", "down");
            if (direction != Vector2.Zero)
            {
                velocity.X = direction.X * _speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
            }
    
            Velocity = velocity;
            MoveAndSlide();
        }
        public override void _Process(double delta)
        {
            if (direction.X != 0)
            {
                GetNode<Sprite2D>("Sprite2D").FlipH = direction.X < 0;
            }
        }
    }
}

