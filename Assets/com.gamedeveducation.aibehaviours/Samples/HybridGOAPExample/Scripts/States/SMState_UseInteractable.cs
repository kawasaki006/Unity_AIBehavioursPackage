using CommonCore;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class SMState_UseInteractable : SMStateBase
    {
        bool bFinishedInteraction = false;

        public SMState_UseInteractable(string InCustomName = null) : base(InCustomName) { }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            bFinishedInteraction = false;

            SmartObject TargetSO = null;
            InBlackboard.TryGet(CommonCore.Names.Interaction_SmartObject, out TargetSO, null);
            if (TargetSO == null)
                return ESMStateStatus.Failed;

            BaseInteraction TargetInteraction = null;
            InBlackboard.TryGet(CommonCore.Names.Interaction_Type, out TargetInteraction, null);
            if (TargetInteraction == null)
                return ESMStateStatus.Failed;

            var Self = GetOwner(InBlackboard);

            // perform interaction
            TargetInteraction.Perform(Self, (BaseInteraction InInteraction) =>
            {
                InInteraction.UnlockInteraction(Self);
                bFinishedInteraction = true;
            });

            return ESMStateStatus.Running;
        }

        protected override void OnExitInternal(Blackboard<FastName> InBlackboard)
        {
            // when we finish using current interaction, reset blackboard info on SO and interaction
            InBlackboard.Set(CommonCore.Names.Interaction_SmartObject, (SmartObject)null);
            InBlackboard.Set(CommonCore.Names.Interaction_Type, (BaseInteraction)null);
        }

        protected override ESMStateStatus OnTickInternal(Blackboard<FastName> InBlackboard, float InDeltaTime)
        {
            // finished bool prevents ontick from having to regularly pulling the status of the performing interaction
            return bFinishedInteraction ? ESMStateStatus.Finished : ESMStateStatus.Running;
        }
    }
}
