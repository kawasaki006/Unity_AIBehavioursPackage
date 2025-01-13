using CommonCore;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class SMState_Gather : SMStateBase
    {
        float GatherSpeed;
        CommonCore.ResourceSource Source;

        CommonCore.FastName AmountHeldKey;
        CommonCore.FastName CapacityKey;

        public SMState_Gather(float InGatherSpeed, string InCustomName = null) :
            base(InCustomName)
        {
            GatherSpeed = InGatherSpeed;    
        }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            // try to get type
            var FocusType = CommonCore.Resources.EType.Unknown;
            InBlackboard.TryGetGeneric<CommonCore.Resources.EType>(CommonCore.Names.Resource_FocusType, out FocusType, CommonCore.Resources.EType.Unknown);

            if (FocusType == CommonCore.Resources.EType.Unknown)
                return ESMStateStatus.Failed;

            // try to get source
            GameObject SourceGO = null;
            InBlackboard.TryGet(CommonCore.Names.Resource_FocusSource, out SourceGO, null);

            if (SourceGO == null)
                return ESMStateStatus.Failed;

            if (!SourceGO.TryGetComponent<CommonCore.ResourceSource>(out Source))
                return ESMStateStatus.Failed;

            // check that the source is valid
            if (Source.ResourceType != FocusType || !Source.CanHarvest)
                return ESMStateStatus.Failed;

            var ResourceName = FocusType.ToString();
            AmountHeldKey = new FastName($"Self.Inventory.{ResourceName}.Held");
            CapacityKey = new FastName($"Self.Inventory.{ResourceName}.Capacity");

            return ESMStateStatus.Running;
        }

        protected override void OnExitInternal(Blackboard<FastName> InBlackboard)
        {
            // once finished gathering, focus source should be reset
            InBlackboard.Set(CommonCore.Names.Resource_FocusSource, (GameObject)null);
        }

        protected override ESMStateStatus OnTickInternal(Blackboard<FastName> InBlackboard, float InDeltaTime)
        {
            // retrieve current amount held and source capacity
            float AmountHeld = InBlackboard.GetFloat(AmountHeldKey);
            float Capacity = InBlackboard.GetFloat(CapacityKey);

            // gather per delta time
            float AmountToRetrieve = Mathf.Min(InDeltaTime * GatherSpeed, Capacity - AmountHeld);

            // update blackboard amount of the source
            AmountHeld += Source.Consume(AmountToRetrieve);
            InBlackboard.Set(AmountHeldKey, AmountHeld);

            // if source is depleted
            if (!Source.CanHarvest || Mathf.Approximately(AmountHeld, Capacity))
                return ESMStateStatus.Finished;

            return ESMStateStatus.Running;
        }
    }
}
