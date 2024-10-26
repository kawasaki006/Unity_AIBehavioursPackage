using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public interface IBTService : IDebuggable
    {
        GameObject Self { get; }
        IBehaviourTree OwningTree { get; }
        Blackboard<FastName> LinkedBlackBoard { get; }

        void SetOwningTree(IBehaviourTree InOwningTree);

        bool Tick(float InDeltaTime);
    }

}
