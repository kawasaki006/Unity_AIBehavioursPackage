using CommonCore;
using StateMachine;
using UnityEngine;

namespace DemoScenes
{
    public class SMState_Store : SMStateBase
    {
        float Speed;
        CommonCore.ResourceContainer Storage;

        CommonCore.FastName AmountHeldKey;

        public SMState_Store(float InSpeed, string InDisplayName = null) :
            base(InDisplayName)
        {
            Speed = InSpeed;
        }

        protected override ESMStateStatus OnEnterInternal()
        {
            // get the focus type
            var FocusType = CommonCore.Resources.EType.Unknown;
            LinkedBlackboard.TryGetGeneric<CommonCore.Resources.EType>(CommonCore.Names.Resource_FocusType, out FocusType, CommonCore.Resources.EType.Unknown);

            if (FocusType == CommonCore.Resources.EType.Unknown)
                return ESMStateStatus.Failed;

            // get the container game object
            GameObject StorageGO = null;
            LinkedBlackboard.TryGet(CommonCore.Names.Resource_FocusStorage, out StorageGO, null);

            if (StorageGO == null)
                return ESMStateStatus.Failed;

            if (!StorageGO.TryGetComponent<CommonCore.ResourceContainer>(out Storage))
                return ESMStateStatus.Failed;

            // check that the container is valid
            if ((Storage.ResourceType != FocusType) || !Storage.CanStore)
                return ESMStateStatus.Failed;

            var ResourceName = FocusType.ToString();
            AmountHeldKey = new FastName($"Self.Inventory.{ResourceName}.Held");

            return ESMStateStatus.Running;
        }

        protected override void OnExitInternal()
        {
            LinkedBlackboard.SetGeneric(CommonCore.Names.Resource_FocusType, CommonCore.Resources.EType.Unknown);
            LinkedBlackboard.Set(CommonCore.Names.Resource_FocusStorage, (GameObject)null);
        }

        protected override ESMStateStatus OnTickInternal(float InDeltaTime)
        {
            float AmountHeld = LinkedBlackboard.GetFloat(AmountHeldKey);

            float AmountToStore = Mathf.Max(InDeltaTime * Speed, 0f);

            Storage.StoreResource(AmountToStore);

            AmountHeld -= AmountToStore;
            LinkedBlackboard.Set(AmountHeldKey, AmountHeld);

            if (!Storage.CanStore || (AmountHeld <= 0))
                return ESMStateStatus.Finished;

            return ESMStateStatus.Running;

        }
    }
}
