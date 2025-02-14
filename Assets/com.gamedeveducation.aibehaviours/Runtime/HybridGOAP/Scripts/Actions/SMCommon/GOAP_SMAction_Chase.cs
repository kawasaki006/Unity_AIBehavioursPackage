using StateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAP
{
    [AddComponentMenu("AI/GOAP/FSM Actions/FSM Action: Chase")]
    public class GOAP_SMAction_Chase : GOAPAction_FSM
    {
        [Tooltip("Controls how far from our goal location that we will search for a valid location on the NavMesh.")]
        [SerializeField] float ValidNavMeshSearchRange = 2.0f;

        [Tooltip("Controls how close the AI needs to get to the destination to consider it reached.")]
        [SerializeField] float StoppingDistance = 1f;

        public override float CalculateCost()
        {
            return 20.0f;
        }

        protected override void ConfigureFSM()
        {
            var ChildStates = new List<ISMState>();
            ChildStates.Add(new SMState_CalculateMoveLocation(Navigation, ValidNavMeshSearchRange, GetTargetLocation, GetEndOrientation, true));
            ChildStates.Add(new SMState_MoveTo(Navigation, StoppingDistance, true));

            AddState(new SMState_Parallel(ChildStates));

            AddDefaultTransitions();
        }

        protected override ECharacterResources GetRequiredResources()
        {
            return ECharacterResources.Legs | ECharacterResources.Torso;
        }

        protected override void PopulateSupportedGoalTypes()
        {
            SupportedGoalTypes = new System.Type[] { typeof(GOAPGoal_Chase) };
        }

        Vector3 GetEndOrientation()
        {
            Vector3 CurrentLocation = LinkedBlackboard.GetVector3(CommonCore.Names.CurrentLocation);
            Vector3 TargetLocation = GetTargetLocation();

            if ((TargetLocation == CommonCore.Constants.InvalidVector3Position) ||
                (CurrentLocation == CommonCore.Constants.InvalidVector3Position))
                return CommonCore.Constants.InvalidVector3Position;

            return (TargetLocation - CurrentLocation).normalized;
        }

        Vector3 GetTargetLocation()
        {
            Vector3 TargetLocation = CommonCore.Constants.InvalidVector3Position;

            // attempt to get the target object
            GameObject TargetGO = null;
            LinkedBlackboard.TryGet(CommonCore.Names.Awareness_BestTarget, out TargetGO, null);

            if (TargetGO != null)
            {
                TargetLocation = TargetGO.transform.position;
                LinkedBlackboard.Set(CommonCore.Names.Target_GameObject, TargetGO);
            }
            else
            {
                LinkedBlackboard.Set(CommonCore.Names.Target_GameObject, (GameObject)null);
                LinkedBlackboard.TryGet(CommonCore.Names.Target_Position, out TargetLocation, CommonCore.Constants.InvalidVector3Position);
            }

            return TargetLocation;
        }
    }
}
