using CommonCore;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class SMState_SelectStorage : SMStateBase
    {
        IResourceQueryInterface ResourceInterface;

        public SMState_SelectStorage(IResourceQueryInterface InResourceInterface, string InCustomName = null) : 
            base(InCustomName)
        {
            ResourceInterface = InResourceInterface;        
        }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            // cleanup
            InBlackboard.Set(CommonCore.Names.Resource_FocusStorage, (GameObject)null);

            // no valid resource interface
            if (ResourceInterface == null)
                return ESMStateStatus.Failed;

            var Self = GetOwner(InBlackboard);

            // try to get type
            var FocusType = CommonCore.Resources.EType.Unknown;
            InBlackboard.TryGetGeneric<CommonCore.Resources.EType>(CommonCore.Names.Resource_FocusType, out FocusType, CommonCore.Resources.EType.Unknown);

            if (FocusType == CommonCore.Resources.EType.Unknown)
                return ESMStateStatus.Failed;

            // select a storage container
            GameObject StorageGO = null;
            ResourceInterface.RequestResourceStorage(Self, FocusType, (GameObject InStorage) =>
            {
                StorageGO = InStorage;
            });

            if (StorageGO == null)
                return ESMStateStatus.Failed;

            // update blackboard
            InBlackboard.Set(CommonCore.Names.Resource_FocusStorage, StorageGO);

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
