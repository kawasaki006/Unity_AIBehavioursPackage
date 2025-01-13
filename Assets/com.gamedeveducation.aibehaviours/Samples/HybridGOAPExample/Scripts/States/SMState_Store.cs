using CommonCore;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class SMState_Store : SMStateBase
    {
        float StoreSpeed;
        CommonCore.ResourceContainer Storage;

        CommonCore.FastName AmountHeldKey;

        public SMState_Store(float InStoreSpeed, string InCustomName = null) :
            base(InCustomName)
        {
            StoreSpeed = InStoreSpeed;  
        }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            // try to get type
            var FocusType = CommonCore.Resources.EType.Unknown;
            InBlackboard.TryGetGeneric<CommonCore.Resources.EType>(CommonCore.Names.Resource_FocusType, out FocusType, CommonCore.Resources.EType.Unknown);

            if (FocusType == CommonCore.Resources.EType.Unknown)
                return ESMStateStatus.Failed;

            // try to get source
            GameObject StorageGO = null;
            InBlackboard.TryGet(CommonCore.Names.Resource_FocusStorage, out StorageGO, null);

            if (StorageGO == null)
                return ESMStateStatus.Failed;

            if (!StorageGO.TryGetComponent<CommonCore.ResourceContainer>(out Storage))
                return ESMStateStatus.Failed;

            // check that the container is valid
            if (Storage.ResourceType != FocusType || !Storage.CanStore)
                return ESMStateStatus.Failed;

            var ResourceName = FocusType.ToString();
            AmountHeldKey = new FastName($"Self.Inventory.{ResourceName}.Held");

            return ESMStateStatus.Running;
        }

        protected override void OnExitInternal(Blackboard<FastName> InBlackboard)
        {
            // once finished storing, focus storage and type should be reset
            InBlackboard.SetGeneric(CommonCore.Names.Resource_FocusType, CommonCore.Resources.EType.Unknown);
            InBlackboard.Set(CommonCore.Names.Resource_FocusStorage, (GameObject)null);
        }

        protected override ESMStateStatus OnTickInternal(Blackboard<FastName> InBlackboard, float InDeltaTime)
        {
            // retrieve current amount held and source capacity
            float AmountHeld = InBlackboard.GetFloat(AmountHeldKey);

            // gather per delta time
            float AmountToStore = Mathf.Max(InDeltaTime * StoreSpeed, 0f);

            // store
            Storage.StoreResource(AmountToStore);

            // update blackboard
            AmountHeld -= AmountToStore;
            InBlackboard.Set(AmountHeldKey, AmountHeld);

            // if storage is full or nothing held
            if (!Storage.CanStore || AmountHeld <= 0)
                return ESMStateStatus.Finished;

            return ESMStateStatus.Running;
        }
    }
}
