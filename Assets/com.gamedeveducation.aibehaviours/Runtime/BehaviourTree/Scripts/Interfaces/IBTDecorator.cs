using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public interface IBTDecorator : IDebuggable
    {
        GameObject Self { get; }
        IBehaviourTree OwningTree { get; }
        Blackboard<FastName> LinkedBlackBoard { get; }
        bool CanPostProcessTickResult { get; }

        void SetOwningTree(IBehaviourTree InOwningTree);

        bool Tick(float InDeltaTime);
        EBTNodeResult PostProcessTickResult(EBTNodeResult InResult);
    }

}
