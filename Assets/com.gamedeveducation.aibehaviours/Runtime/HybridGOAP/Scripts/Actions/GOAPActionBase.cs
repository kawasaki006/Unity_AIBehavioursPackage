using CharacterCore;
using CommonCore;
using UnityEngine;

namespace HybridGOAP
{
    public abstract class GOAPActionBase : MonoBehaviour, IGOAPAction
    {
        protected IGOAPBrain LinkedBrain;
        protected INavigationInterface Navigation;
        protected ILookHandler LookAtHandler;
        protected IInteractionSelector InteractionInterface;
        protected IInteractionPerformer PerformerInterface;
        protected IAnimationInterface AnimationInterface;

        protected Blackboard<FastName> LinkedBlackboard => LinkedBrain.LinkedBlackboard;

        protected System.Type[] SupportedGoalTypes = { };

        public ECharacterResources ResourcesRequired { get => GetRequiredResources(); }

        public string DebugDisplayName { get => GetType().Name; }

        protected GameObject Self => LinkedBlackboard.GetGameObject(CommonCore.Names.Self);

        public void BindToBrain(IGOAPBrain InBrain)
        {
            LinkedBrain = InBrain;

            ServiceLocator.AsyncLocateService<INavigationInterface>((ILocatableService InService) =>
            {
                Navigation = InService as INavigationInterface;
            }, gameObject, EServiceSearchMode.LocalOnly);
            ServiceLocator.AsyncLocateService<ILookHandler>((ILocatableService InService) =>
            {
                LookAtHandler = (ILookHandler)InService;
            }, gameObject, EServiceSearchMode.LocalOnly);

            ServiceLocator.AsyncLocateService<IInteractionSelector>((ILocatableService InService) =>
            {
                InteractionInterface = InService as IInteractionSelector;
            }, gameObject, EServiceSearchMode.LocalOnly);
            ServiceLocator.AsyncLocateService<IInteractionPerformer>((ILocatableService InService) =>
            {
                PerformerInterface = (IInteractionPerformer)InService;
            }, gameObject, EServiceSearchMode.LocalOnly);

            ServiceLocator.AsyncLocateService<IAnimationInterface>((ILocatableService InService) =>
            {
                AnimationInterface = (IAnimationInterface)InService;
            }, gameObject, EServiceSearchMode.LocalOnly);

            PopulateSupportedGoalTypes();
            OnInitialise();
        }

        public bool CanSatisfy(IGOAPGoal InGoal)
        {
            // cannot satisfy the goal until we have a navigation interface
            if (Navigation == null)
                return false;

            // bail out if the action is not able to run
            if (!CanActionRun())
                return false;

            foreach (var GoalType in SupportedGoalTypes)
            {
                if (InGoal.GetType() == GoalType)
                    return true;
            }

            return false;
        }

        public void GatherDebugData(IGameDebugger InDebugger, bool bInIsSelected)
        {
            if (!bInIsSelected)
                return;

            InDebugger.AddSectionHeader($"{DebugDisplayName} [{ResourcesRequired}]");

            GatherDebugDataInternal(InDebugger, bInIsSelected);
        }

        public void StartAction()
        {
            OnStartAction();
        }

        public void ContinueAction()
        {
            OnContinueAction();
        }

        public void StopAction()
        {
            OnStopAction();
        }

        public void TickAction(float InDeltaTime)
        {
            OnTickAction(InDeltaTime);
        }

        protected abstract void PopulateSupportedGoalTypes();
        protected abstract void OnInitialise();

        protected abstract ECharacterResources GetRequiredResources();
        public abstract float CalculateCost();

        protected abstract void OnStartAction();
        protected abstract void OnContinueAction();
        protected abstract void OnStopAction();
        protected abstract void OnTickAction(float InDeltaTime);

        protected virtual void GatherDebugDataInternal(IGameDebugger InDebugger, bool bInIsSelected) { }
        protected virtual bool CanActionRun() { return true; }
    }
}
