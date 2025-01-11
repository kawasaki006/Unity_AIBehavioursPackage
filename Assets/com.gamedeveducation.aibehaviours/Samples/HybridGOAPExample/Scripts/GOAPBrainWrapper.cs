using HybridGOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HybridGOAPExample
{
    public class GOAPBrainWrapper : GOAPBrainBase
    {
        BaseNavigation NavSystem;

        protected override void ConfigureBlackboard()
        {
            
        }

        protected override void ConfigureBrain()
        {
            NavSystem = GetComponent<BaseNavigation>();
        }

        public void FindNearestNavigableLocation(Vector3 InSearchLocation, float InSearchRange, System.Action<Vector3> InCallbackFn)
        {
            if (NavSystem == null)
                InCallbackFn(CommonCore.Constants.InvalidVector3Position);

            Vector3 FoundPosition = CommonCore.Constants.InvalidVector3Position;

            if (NavSystem.FindNearestPoint(InSearchLocation, InSearchRange, out FoundPosition))
                InCallbackFn(FoundPosition);
            else
                InCallbackFn(CommonCore.Constants.InvalidVector3Position);
        }

        public void SetMoveLocation(Vector3 InDestination, float StoppingDistance, System.Action<bool> InCallbackFn)
        {
            if (NavSystem == null)
                InCallbackFn(false);

            bool bResult = NavSystem.SetDestination(InDestination);
            if (bResult)
            {
                NavSystem.SetDestinationReachedThreshold(StoppingDistance);
                InCallbackFn(true);
            }
            else
                InCallbackFn(false);
        }

        public void IsPathfindingOrMovingFn(System.Action<bool> InCallbackFn)
        {
            if (NavSystem == null)
                InCallbackFn(false);

            InCallbackFn(NavSystem.IsFindingOrFollowingPath);
        }

        public void IsAtDestinationFn(System.Action<bool> InCallbackFn)
        {
            if (NavSystem == null)
                InCallbackFn(false);

            InCallbackFn(NavSystem.IsAtDestination);
        }

        public void StopMovingFn(System.Action<bool> InCallbackFn)
        {
            if (NavSystem == null)
                InCallbackFn(false);

            NavSystem.StopMovement();
            InCallbackFn(true);
        }

        // hooked to awareness system event OnBestTargetChanged 
        public void SetDetectedTarget(GameObject InTarget)
        {
            GameObject CurrentAwarenessTarget = null;
            CurrentBlackboard.TryGet(CommonCore.Names.Awareness_BestTarget, out CurrentAwarenessTarget, null);

            // if we're changing targets?
            if ((CurrentAwarenessTarget != null) && (CurrentAwarenessTarget != InTarget))
            {
                GameObject CurrentTarget = null;
                CurrentBlackboard.TryGet(CommonCore.Names.Target_GameObject, out CurrentTarget, null);

                if (CurrentTarget == CurrentAwarenessTarget)
                    CurrentBlackboard.Set(CommonCore.Names.Target_GameObject, (GameObject) null);
            }

            CurrentBlackboard.Set(CommonCore.Names.Awareness_BestTarget, InTarget);
        }
    }
}
