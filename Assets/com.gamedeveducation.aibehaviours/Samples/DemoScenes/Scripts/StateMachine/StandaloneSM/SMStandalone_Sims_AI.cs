using CharacterCore;
using StateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace DemoScenes
{
    [AddComponentMenu("AI/Standalone/State Machine: Sims AI")]
    public class SMStandalone_Sims_AI : SMStandaloneBase
    {
        [Header("Wander")]
        [SerializeField] float MinWanderRange = 10.0f;
        [SerializeField] float MaxWanderRange = 50.0f;
        [SerializeField] float MinWanderWaitTime = 4.0f;
        [SerializeField] float MaxWanderWaitTime = 8.0f;

        [Header("Idle")]
        [SerializeField] float MinIdleWaitTime = 1.0f;
        [SerializeField] float MaxIdleWaitTime = 10.0f;

        [Header("Navigation")]
        [Tooltip("Controls how far from our goal location that we will search for a valid location on the NavMesh.")]
        [SerializeField] float ValidNavMeshSearchRange = 2.0f;

        [Tooltip("Controls how close the AI needs to get to the destination to consider it reached.")]
        [SerializeField] float StoppingDistance = 0.1f;

        protected override void ConfigureFSM()
        {
            var SMCoreLogic = new SMState_SMContainer("Core Logic");

            // Interact State
            var SMInteract = SMCoreLogic.AddState(new SMState_SMContainer("Interact")) as SMState_SMContainer;
            {
                var State_SelectInteractable = SMInteract.AddState(new SMState_SelectInteraction(InteractionInterface, PerformerInterface));
                var State_GetInteractableLocation = SMInteract.AddState(new SMState_CalculateMoveLocation(Navigation, ValidNavMeshSearchRange, GetInteractableLocation, GetInteractionDirection));
                var State_MoveToInteractable = SMInteract.AddState(new SMState_MoveTo(Navigation, StoppingDistance));
                var State_UseInteractable = SMInteract.AddState(new SMState_UseInteractable(PerformerInterface));

                State_SelectInteractable.AddTransition(SMTransition_StateStatus.IfHasFinished(), State_GetInteractableLocation);
                State_GetInteractableLocation.AddTransition(SMTransition_StateStatus.IfHasFinished(), State_MoveToInteractable);
                State_MoveToInteractable.AddTransition(SMTransition_StateStatus.IfHasFinished(), State_UseInteractable);

                SMInteract.FinishConstruction();
            }

            // Wander State
            var SMWander = SMCoreLogic.AddState(new SMState_SMContainer("Wander")) as SMState_SMContainer;
            {
                var State_PickLocation = SMWander.AddState(new SMState_CalculateMoveLocation(Navigation, ValidNavMeshSearchRange, GetWanderLocation, null));
                var State_MoveToLocation = SMWander.AddState(new SMState_MoveTo(Navigation, StoppingDistance));
                var State_Wait = SMWander.AddState(new SMState_WaitForTime(MinWanderWaitTime, MaxWanderWaitTime));

                State_PickLocation.AddTransition(SMTransition_StateStatus.IfHasFinished(), State_MoveToLocation);
                State_MoveToLocation.AddTransition(new SMTransition_FinishedMove(Navigation), State_Wait);

                SMWander.FinishConstruction();
            }

            // Idle State
            var SMIdle = SMCoreLogic.AddState(new SMState_WaitForTime(MinIdleWaitTime, MaxIdleWaitTime, "Idle"));

            // Setup core logic transitions
            {
                SMInteract.AddTransition(new SMTransition_StateStatus(ESMStateStatus.Finished), SMWander);
                SMInteract.AddTransition(new SMTransition_StateStatus(ESMStateStatus.Failed), SMWander);

                SMWander.AddTransition(new SMTransition_StateStatus(ESMStateStatus.Finished), SMIdle);
                SMWander.AddTransition(new SMTransition_StateStatus(ESMStateStatus.Failed), SMIdle);

                SMIdle.AddTransition(new SMTransition_StateStatus(ESMStateStatus.Finished), SMInteract);
                SMIdle.AddTransition(new SMTransition_StateStatus(ESMStateStatus.Failed), SMInteract);

                SMCoreLogic.FinishConstruction();
            }

            // Look At State
            var SMLookAt = new SMState_SMContainer("Look At");
            {
                var ChildStates = new List<ISMState>();

                ChildStates.Add(new SMState_SelectPOI(LookAtHandler));
                ChildStates.Add(new SMState_LookAtPOI(LookAtHandler));

                SMLookAt.AddState(new SMState_Parallel(ChildStates));

                SMLookAt.FinishConstruction();
            }

            var ParallelStates = new List<ISMState>();
            ParallelStates.Add(SMCoreLogic);
            ParallelStates.Add(SMLookAt);
            AddState(new SMState_Parallel(ParallelStates, true, false));
            AddDefaultTransitions();
        }

        protected override void ConfigureBlackboard()
        {
        }

        protected override void ConfigureBrain()
        {
        }

        Vector3 GetInteractableLocation()
        {
            // attempt to get the interact point
            IInteractionPoint TargetPoint;
            LinkedBlackboard.TryGetStorable<IInteractionPoint>(CommonCore.Names.Interaction_Point, out TargetPoint, null);
            if (TargetPoint != null)
                return TargetPoint.PointPosition;

            return CommonCore.Constants.InvalidVector3Position;
        }

        Vector3 GetInteractionDirection()
        {
            // attempt to get the interact point
            IInteractionPoint TargetPoint;
            LinkedBlackboard.TryGetStorable<IInteractionPoint>(CommonCore.Names.Interaction_Point, out TargetPoint, null);
            if (TargetPoint != null)
                return TargetPoint.PointTransform.forward;

            return CommonCore.Constants.InvalidVector3Position;
        }

        Vector3 GetWanderLocation()
        {
            Vector3 CurrentLocation = LinkedBlackboard.GetVector3(CommonCore.Names.CurrentLocation);

            // pick random direction and distance
            float Angle = Random.Range(0f, Mathf.PI * 2f);
            float Distance = Random.Range(MinWanderRange, MaxWanderRange);

            // generate a position
            Vector3 WanderTarget = CurrentLocation;
            WanderTarget.x += Distance * Mathf.Sin(Angle);
            WanderTarget.z += Distance * Mathf.Cos(Angle);

            return WanderTarget;
        }

        protected override void OnStateMachineReset()
        {
            base.OnStateMachineReset();

            IInteraction CurrentInteraction;
            LinkedBlackboard.TryGetStorable<IInteraction>(CommonCore.Names.Interaction_Type, out CurrentInteraction, null);
            if (CurrentInteraction != null)
                CurrentInteraction.AbandonInteraction(PerformerInterface);

            LinkedBlackboard.Set(CommonCore.Names.Interaction_Interactable, (IInteractable)null);
            LinkedBlackboard.Set(CommonCore.Names.Interaction_Type, (IInteraction)null);
            LinkedBlackboard.Set(CommonCore.Names.Interaction_Point, (IInteractionPoint)null);
        }
    }
}
