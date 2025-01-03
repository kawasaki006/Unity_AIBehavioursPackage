using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class SMState_WaitForTime : SMStateBase
    {
        float MinTime;
        float MaxTime;
        float TimeRemaining;

        public SMState_WaitForTime(float InMinTme, float InMaxTme, string InCustomName = null) :
            base(InCustomName)
        {
            MinTime = InMinTme;
            MaxTime = InMaxTme;
        }

        protected override ESMStateStatus OnEnterInternal(Blackboard<FastName> InBlackboard)
        {
            TimeRemaining = Random.Range(MinTime, MaxTime);

            return TimeRemaining > 0 ? ESMStateStatus.Running : ESMStateStatus.Finished;
        }

        protected override void OnExitInternal(Blackboard<FastName> InBlackboard)
        {
        }

        protected override ESMStateStatus OnTickInternal(Blackboard<FastName> InBlackboard, float InDeltaTime)
        {
            if (TimeRemaining > 0)
            {
                TimeRemaining -= InDeltaTime;
            }

            return TimeRemaining > 0 ? ESMStateStatus.Running : ESMStateStatus.Finished;
        }

        protected override void GatherDebugDataInternal(IGameDebugger InDebugger, bool bIsSelected)
        {
            base.GatherDebugDataInternal(InDebugger, bIsSelected);

            InDebugger.PushIndent();
            InDebugger.AddTextLine($"Time Remaining: {TimeRemaining} s");
            InDebugger.PopIndent();
        }
    }
}
