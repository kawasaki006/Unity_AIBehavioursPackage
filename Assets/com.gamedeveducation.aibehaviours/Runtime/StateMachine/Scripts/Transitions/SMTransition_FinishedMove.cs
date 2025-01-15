using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class SMTransition_FinishedMove : SMTransitionBase
    {
        INavigationInterface Navigation;

        public SMTransition_FinishedMove(INavigationInterface InNavigation)
        {
            Navigation = InNavigation;
        }

        protected override ESMTransitionResult EvaluateInternal(ISMState InCurrentState, Blackboard<FastName> InBlackboard)
        {
            var Self = GetOwner(InBlackboard);

            return Navigation.HasReachedDestination(Self) ? ESMTransitionResult.Valid : ESMTransitionResult.Invalid;
        }
    }
}
