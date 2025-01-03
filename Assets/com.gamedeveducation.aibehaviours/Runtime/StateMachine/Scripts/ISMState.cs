using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public enum ESMStateStatus
    {
        Uninitialized,
        Running,
        Failed,
        Finished
    }

    public interface ISMState : IDebuggable
    {
        ESMStateStatus CurrentStatus { get; }

        ISMState AddTransition(ISMTransition InTransition, ISMState InNewState);

        void AddDefaultTransition(ISMState InFinishedState, ISMState InFailedState);

        void EvaluateTransitions(Blackboard<FastName> InBlackboard, out ISMState OutNextState);

        ESMStateStatus OnEnter(Blackboard<FastName> InBlackboard);
        ESMStateStatus OnTick(Blackboard<FastName> InBlackboard, float InDeltaTime);
        void OnExit(Blackboard<FastName> InBlackboard);
    }
}
