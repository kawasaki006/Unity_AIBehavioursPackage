using HybridGOAP;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAPExample
{
    public class GOAPAction_Idle : GOAPAction_FSM
    {
        [SerializeField] float MinWaitTime = 1f;
        [SerializeField] float MaxWaitTime = 10f;

        public override float CalculateCost()
        {
            return 0f;
        }

        protected override void ConfigureFSM()
        {
            LinkedStateMachine.AddState(new SMState_WaitForTime(MinWaitTime, MaxWaitTime));

            LinkedStateMachine.AddDefaultTransition(InternalState_Finished, InternalState_Failed);
        }

        protected override ECharacterResources GetRequiredResources()
        {
            return ECharacterResources.Legs | ECharacterResources.Torso;
        }

        protected override void PopulateSupportedGoalTypes()
        {
            SupportedGoalTypes = new System.Type[] { typeof(GOAPGoal_Idle) };
        }
    }
}
