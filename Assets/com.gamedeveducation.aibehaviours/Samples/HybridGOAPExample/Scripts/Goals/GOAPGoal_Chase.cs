using HybridGOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class GOAPGoal_Chase : GOAPGoalBase
    {
        [SerializeField] int DesireToChaseStartDistance = 1;
        [SerializeField] int DesireToChaseEndDistnce = 50;

        public override void PrepareForPlanning()
        {
            Vector3 TargetLocation = GetTargetLocation();

            if (TargetLocation == CommonCore.Constants.InvalidVector3Position)
                Priority = GoalPriority.DoNotRun;
            else
            {
                Vector3 CurrentLocation = LinkedBlackboard.GetVector3(CommonCore.Names.CurrentLocation);
                float DistanceToTargetSq = (CurrentLocation - TargetLocation).sqrMagnitude;

                if (DistanceToTargetSq > (DesireToChaseEndDistnce * DesireToChaseEndDistnce))
                    Priority = GoalPriority.DoNotRun;
                else if (DistanceToTargetSq < (DesireToChaseStartDistance * DesireToChaseStartDistance))
                    Priority = GoalPriority.High;
                else
                {
                    float InterpolationFactor = Mathf.InverseLerp(DesireToChaseStartDistance,
                                                                  DesireToChaseEndDistnce,
                                                                  Mathf.Sqrt(DistanceToTargetSq));
                    Priority = Mathf.FloorToInt(Mathf.Lerp(GoalPriority.High, GoalPriority.Low, InterpolationFactor));
                }
            }
        }

        Vector3 GetTargetLocation()
        {
            Vector3 TargetLocation = CommonCore.Constants.InvalidVector3Position;

            // attempt to get target object
            GameObject TargetGO = null;
            LinkedBlackboard.TryGet(CommonCore.Names.Awareness_BestTarget, out TargetGO, null);

            if (TargetGO != null)
            {
                TargetLocation = TargetGO.transform.position;
                // if chase is running, the targte becomes the focused target, so mark it as the final target object
                LinkedBlackboard.Set(CommonCore.Names.Target_GameObject, TargetGO);
            }
            else // attempt to get target position; fail to find an awareness target
            {
                // reset target object and position
                LinkedBlackboard.Set(CommonCore.Names.Target_GameObject, (GameObject) null); 
                LinkedBlackboard.TryGet(CommonCore.Names.Target_Position, out TargetLocation, CommonCore.Constants.InvalidVector3Position);
            }

            return TargetLocation;
        }
    }
}
