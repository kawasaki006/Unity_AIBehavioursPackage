using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public interface IBTFlowNode : IBTNode, IEnumerable
    {
        IBTNode AddChild(IBTNode InNode);
    }

}
