using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public interface IBTBrain : IDebuggableObject
    {
        Blackboard<FastName> LinkedBlackboard { get; }
    }
}
