using CharacterCore;
using CommonCore;
using UnityEngine;

namespace HybridGOAP
{
    public abstract class GOAPGoalBase : MonoBehaviour, IGOAPGoal
    {
        protected IGOAPBrain LinkedBrain;
        protected Blackboard<FastName> LinkedBlackboard => LinkedBrain.LinkedBlackboard;
        protected IInteractionSelector InteractionInterface;
        protected IInteractionPerformer PerformerInterface;

        public int Priority { get; protected set; } = GoalPriority.DoNotRun;

        public string DebugDisplayName => GetType().Name;

        protected GameObject Self => LinkedBlackboard.GetGameObject(CommonCore.Names.Self);

        public void BindToBrain(IGOAPBrain InBrain)
        {
            LinkedBrain = InBrain;

            ServiceLocator.AsyncLocateService<IInteractionSelector>((ILocatableService InService) =>
            {
                InteractionInterface = InService as IInteractionSelector;
            }, gameObject, EServiceSearchMode.LocalOnly);
            ServiceLocator.AsyncLocateService<IInteractionPerformer>((ILocatableService InService) =>
            {
                PerformerInterface = (IInteractionPerformer)InService;
            }, gameObject, EServiceSearchMode.LocalOnly);

            OnInitialise();

            PrepareForPlanning();
        }

        public void StartGoal()
        {
            OnStartGoal();
        }

        public void ContinueGoal()
        {
            OnContinueGoal();
        }

        public void StopGoal()
        {
            OnStopGoal();
        }

        public void TickGoal(float InDeltaTime)
        {
            OnTickGoal(InDeltaTime);
        }

        public void GatherDebugData(IGameDebugger InDebugger, bool bInIsSelected)
        {
            if (bInIsSelected)
                InDebugger.AddSectionHeader($"<b>{DebugDisplayName} [{Priority}]</b>");
            else
                InDebugger.AddSectionHeader($"{DebugDisplayName} [{Priority}]");

            GatherDebugDataInternal(InDebugger, bInIsSelected);
        }

        public abstract void PrepareForPlanning();

        protected virtual void GatherDebugDataInternal(IGameDebugger InDebugger, bool bInIsSelected) { }

        protected virtual void OnInitialise() { }
        protected virtual void OnStartGoal() { }
        protected virtual void OnContinueGoal() { }
        protected virtual void OnStopGoal() { }
        protected virtual void OnTickGoal(float InDeltaTime) { }
    }
}
