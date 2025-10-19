using UnityEngine;

namespace _Project.Code.Gameplay.PlayerController._Base
{
    public interface IMotor
    {
        Vector2 Velocity { get; }

        void Move(Vector2 direction, float speed);
        void Stop();
    }
}
