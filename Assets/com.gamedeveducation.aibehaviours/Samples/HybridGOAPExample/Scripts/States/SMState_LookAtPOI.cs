using CommonCore;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class SMState_LookAtPOI : SMStateBase
    {
        System.Action<GameObject> SetLookAtTargetFn;

        public SMState_LookAtPOI(System.Action<GameObject> InSetLookAtTargetFn, string InCustomName = null) :
            base(InCustomName)
        {
            SetLookAtTargetFn = InSetLookAtTargetFn;
        }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            if (SetLookAtTargetFn == null)
                return ESMStateStatus.Failed;

            GameObject POI = null;
            InBlackboard.TryGet(CommonCore.Names.LookAt_GameObject, out POI, null);

            if (POI == null)
                return ESMStateStatus.Failed;

            SetLookAtTargetFn(POI);

            return ESMStateStatus.Running;
        }

        protected override void OnExitInternal(Blackboard<FastName> InBlackboard)
        {
        }

        protected override ESMStateStatus OnTickInternal(Blackboard<FastName> InBlackboard, float InDeltaTime)
        {
            return OnEnterInternal(InBlackboard);
        }
    }
}
