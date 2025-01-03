using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class SMState_Parallel : SMStateBase
    {
        List<ISMState> ChildStates = null;

        public SMState_Parallel(List<ISMState> InChildStates, string InCustomName = null) :
            base(InCustomName)
        {
            ChildStates = InChildStates;
        }

        // --------- Enter Logic for Parallel State --------- //
        // 1. enter every child state
        // 2. if a child fails to enter, fail out immediately
        // 3. if there is a single child that is running, return running
        // 4. only if all child states are finished, return finished
        // -------------------------------------------------- //
        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            if (ChildStates == null || ChildStates.Count == 0)
                return ESMStateStatus.Failed;

            bool bAllFinished = true;
            foreach (var Child in ChildStates)
            {
                // enter a child
                var Result = Child.OnEnter(InBlackboard);

                // if fail to enter a child, fail out immediately
                if (Result == ESMStateStatus.Failed)
                    return ESMStateStatus.Failed;

                if (Result == ESMStateStatus.Running)
                    bAllFinished = false;
            }

            return bAllFinished ? ESMStateStatus.Finished : ESMStateStatus.Running;
        }

        protected override void OnExitInternal(Blackboard<FastName> InBlackboard)
        {
            if (ChildStates == null || ChildStates.Count == 0)
                return;

            foreach (var Child in ChildStates)
                Child.OnExit(InBlackboard);
        }

        // similar to OnEnter logic
        protected override ESMStateStatus OnTickInternal(Blackboard<FastName> InBlackboard, float InDeltaTime)
        {
            if (ChildStates == null || ChildStates.Count == 0)
                return ESMStateStatus.Failed;

            bool bAllFinished = true;
            foreach (var Child in ChildStates)
            {
                var Result = Child.OnTick(InBlackboard, InDeltaTime);

                if (Result == ESMStateStatus.Failed)
                    return ESMStateStatus.Failed;

                if (Result != ESMStateStatus.Finished)
                    bAllFinished = false;
            }

            return bAllFinished ? ESMStateStatus.Finished : ESMStateStatus.Running;
        }

        protected override void GatherDebugDataInternal(IGameDebugger InDebugger, bool bIsSelected)
        {
            base.GatherDebugDataInternal(InDebugger, bIsSelected);

            InDebugger.PushIndent();

            foreach (var Child in ChildStates)
            {
                Child.GatherDebugData(InDebugger, bIsSelected);
            }

            InDebugger.PopIndent();
        }
    }
}
