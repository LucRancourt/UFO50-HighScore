using UnityEngine;

using _Project.Code.Core.Events;


namespace _Project.Code.Gameplay.Input
{
    public struct MoveInputEvent : IEvent
    {
        public Vector2 Input;
    }

    public struct FireInputEvent : IEvent 
    {
        public bool isFiring;
    }

    public struct PauseInputEvent : IEvent { }
}
