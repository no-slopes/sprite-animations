using UnityEngine;

namespace HandyFSM
{
    public interface IState
    {
        bool Interruptible { get; }
        string Name { get; }
        StateMachine Machine { get; }

        bool ShouldTransition(out IState target);

        void Initialize(StateMachine machine);

        void OnEnter();
        void OnExit();
        void Tick();
        void FixedTick();
        void LateTick();

        void OnCollisionEnter2D(Collision2D collision);
        void OnCollisionStay2D(Collision2D collision);
        void OnCollisionExit2D(Collision2D collision);
        void OnTriggerEnter2D(Collider2D other);
        void OnTriggerStay2D(Collider2D other);
        void OnTriggerExit2D(Collider2D other);
    }
}