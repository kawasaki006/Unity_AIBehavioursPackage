using HybridGOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class GOAPGoal_ReturnHome : GOAPGoalBase
    {
        [SerializeField] float MissingHomeStartDistance = 50f;
        [SerializeField] float MissingHomePeaksDistance = 100f;
        [SerializeField] float MissingHomeResetDistance = 10f;

        public override void PrepareForPlanning()
        {
            Vector3 HomeLocation = GetHomeLocation();

            if (HomeLocation == CommonCore.Constants.InvalidVector3Position)
                Priority = GoalPriority.DoNotRun;
            else
            {
                Vector3 CurrentLocation = LinkedBlackboard.GetVector3(CommonCore.Names.CurrentLocation);

                float DistanceToHomeSq = (HomeLocation - CurrentLocation).sqrMagnitude;

                // persistant desire to return home
                int NewPriority = Priority;
                if (DistanceToHomeSq < MissingHomeStartDistance * MissingHomeStartDistance)
                    NewPriority = GoalPriority.DoNotRun;
                else if (DistanceToHomeSq >= MissingHomePeaksDistance * MissingHomePeaksDistance)
                    NewPriority = GoalPriority.High;
                else
                {
                    float InterpolationFactor = Mathf.InverseLerp(MissingHomeStartDistance, MissingHomePeaksDistance, Mathf.Sqrt(DistanceToHomeSq));
                    NewPriority = Mathf.FloorToInt(Mathf.Lerp(GoalPriority.Low, GoalPriority.High, InterpolationFactor));
                }

                if (DistanceToHomeSq >= MissingHomeResetDistance)
                    Priority = Mathf.Max(Priority, NewPriority);
                else
                    Priority = NewPriority;
            }
        }

        Vector3 GetHomeLocation()
        {
            Vector3 HomeLocation = CommonCore.Constants.InvalidVector3Position;

            LinkedBlackboard.TryGet(CommonCore.Names.HomeLocation, out HomeLocation, CommonCore.Constants.InvalidVector3Position);

            return HomeLocation;
        }
    }
}
