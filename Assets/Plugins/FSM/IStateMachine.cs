using System;
using UnityEngine;

namespace HandyFSM
{
    public interface IStateMachine
    {
        void Resume();
        void Pause();
        void Stop();

        void RequestStateChange(State state, StateChangeMode mode = StateChangeMode.Respectfully);
        void RequestStateChange<T>(StateChangeMode mode = StateChangeMode.Respectfully) where T : State;

        void EndState(State target = null);
        void EndState<T>() where T : State;

        T GetState<T>() where T : State;
        bool TryGetState<T>(out State state) where T : State;

        T As<T>() where T : MonoBehaviour, IStateMachine;
    }
}