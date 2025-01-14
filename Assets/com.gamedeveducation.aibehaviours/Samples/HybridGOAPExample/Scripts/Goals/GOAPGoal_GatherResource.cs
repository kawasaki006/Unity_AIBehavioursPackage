using HybridGOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HybridGOAPExample
{
    public class GOAPGoal_GatherResource : GOAPGoalBase
    {
        [SerializeField] UnityEvent<GameObject, System.Action<float>> OnGetGatherResourceDesire;

        public override void PrepareForPlanning()
        {
            float DesireToGather = float.MinValue;

            // unwanted dependency, will be changed later
            SimpleResourceWrangler.Instance.GetGatherResourceDesire(Self, (float InDesire) =>
            {
                DesireToGather = InDesire;
            });

            if (DesireToGather <= 0)
                Priority = GoalPriority.DoNotRun;
            else
                Priority = Mathf.FloorToInt(Mathf.Lerp(GoalPriority.Medium, GoalPriority.High, DesireToGather));
        }
    }
}
