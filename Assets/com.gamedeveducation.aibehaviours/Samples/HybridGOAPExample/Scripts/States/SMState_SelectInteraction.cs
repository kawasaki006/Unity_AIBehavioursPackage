using CommonCore;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class SMState_SelectInteraction : SMStateBase
    {
        System.Func<GameObject, System.Tuple<SmartObject, BaseInteraction>> SelectInteraction;

        public SMState_SelectInteraction(System.Func<GameObject, System.Tuple<SmartObject, BaseInteraction>> InSelectInteraction, string InCustomName = null) :
            base(InCustomName)
        {
            SelectInteraction = InSelectInteraction;    
        }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            InBlackboard.Set(CommonCore.Names.Interaction_SmartObject, (SmartObject)null);
            InBlackboard.Set(CommonCore.Names.Interaction_Type, (BaseInteraction)null);

            if (SelectInteraction == null)
                return ESMStateStatus.Failed;

            var Self = GetOwner(InBlackboard);

            // attempt to find an interactoin
            var Interaction = SelectInteraction(Self);
            if (Interaction == null || Interaction.Item1 == null || Interaction.Item2 == null)
                return ESMStateStatus.Failed;

            // check if ai can lock the interaction
            var TargetSO = Interaction.Item1;
            var TargetInteraction = Interaction.Item2;
            if (!TargetInteraction.LockInteraction(Self))
                return ESMStateStatus.Failed;

            // populate SO and interaction
            InBlackboard.Set(CommonCore.Names.Interaction_SmartObject, TargetSO);
            InBlackboard.Set(CommonCore.Names.Interaction_Type, TargetInteraction);

            return ESMStateStatus.Finished;
        }

        protected override void OnExitInternal(Blackboard<FastName> InBlackboard)
        {
        }

        protected override ESMStateStatus OnTickInternal(Blackboard<FastName> InBlackboard, float InDeltaTime)
        {
            return CurrentStatus;
        }
    }
}
