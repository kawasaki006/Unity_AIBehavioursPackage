using CommonCore;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class SMState_SelectResource : SMStateBase
    {
        IResourceQueryInterface ResourceInterface;

        public SMState_SelectResource(IResourceQueryInterface InResourceInterface, string InCustomName = null) :
            base(InCustomName)
        {
            ResourceInterface = InResourceInterface;
        }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            // cleanup
            InBlackboard.SetGeneric(CommonCore.Names.Resource_FocusType, CommonCore.Resources.EType.Unknown);
            InBlackboard.Set(CommonCore.Names.Resource_FocusSource, (GameObject)null);

            if (ResourceInterface == null)
                return ESMStateStatus.Failed;

            var Self = GetOwner(InBlackboard);

            // try to find a focus type base on the logic from the cached resource interface
            var FocusType = CommonCore.Resources.EType.Unknown;
            ResourceInterface.RequestResourceFocus(Self, (CommonCore.Resources.EType InFocus) =>
            {
                FocusType = InFocus;
            });

            if (FocusType == CommonCore.Resources.EType.Unknown)
                return ESMStateStatus.Failed;

            // try to find a focus source once we have a type
            var FocusSource = (GameObject)null;
            ResourceInterface.RequestResourceSource(Self, FocusType, (GameObject InSource) =>
            {
                FocusSource = InSource;
            });

            if ( FocusSource == null)
                return ESMStateStatus.Failed;

            // update blackboard
            InBlackboard.SetGeneric(CommonCore.Names.Resource_FocusType, FocusType);
            InBlackboard.Set(CommonCore.Names.Resource_FocusSource, FocusSource);

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
