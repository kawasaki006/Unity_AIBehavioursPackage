using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public interface IBehaviourTree : IDebuggable
    {
        INavigationInterface NavigationInterface { get; }
        Blackboard<FastName> LinkedBlackboard { get; }
        IBTNode RootNode { get; }

        void Initialize(INavigationInterface InNavigationInterface, Blackboard<FastName> InBlackboard, string InRootNodeName = "Root");

        void Reset();

        IBTNode AddChildToRootNode<NodeType>(NodeType InNode) where NodeType : IBTNode;

        EBTNodeResult Tick(float InDeltaTime);
    }

}
