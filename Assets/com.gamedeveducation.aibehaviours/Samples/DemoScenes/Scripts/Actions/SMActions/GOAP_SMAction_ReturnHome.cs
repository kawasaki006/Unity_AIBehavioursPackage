using HybridGOAP;
using StateMachine;
using UnityEngine;

namespace DemoScenes
{
    [AddComponentMenu("AI/GOAP/FSM Actions/FSM Action: Return Home")]
    public class GOAP_SMAction_ReturnHome : GOAPAction_FSM
    {
        [Tooltip("Controls how far from our goal location that we will search for a valid location on the NavMesh.")]
        [SerializeField] float ValidNavMeshSearchRange = 2.0f;

        [Tooltip("Controls how close the AI needs to get to the destination to consider it reached.")]
        [SerializeField] float StoppingDistance = 0.1f;

        public override float CalculateCost()
        {
            return 20f;
        }

        protected override void ConfigureFSM()
        {
            var State_GetHomeLocation = AddState(new SMState_CalculateMoveLocation(Navigation, ValidNavMeshSearchRange, GetHomeLocation, null));
            var State_MoveToLocation = AddState(new SMState_MoveTo(Navigation, StoppingDistance));

            State_GetHomeLocation.AddTransition(SMTransition_StateStatus.IfHasFinished(), State_MoveToLocation);

            AddDefaultTransitions();
        }

        protected override ECharacterResources GetRequiredResources()
        {
            return ECharacterResources.Legs | ECharacterResources.Torso;
        }

        protected override void PopulateSupportedGoalTypes()
        {
            SupportedGoalTypes = new System.Type[] { typeof(GOAPGoal_ReturnHome) };
        }

        Vector3 GetHomeLocation()
        {
            Vector3 HomeLocation = CommonCore.Constants.InvalidVector3Position;

            LinkedBlackboard.TryGet(CommonCore.Names.HomeLocation, out HomeLocation, CommonCore.Constants.InvalidVector3Position);

            return HomeLocation;
        }
    }
}
