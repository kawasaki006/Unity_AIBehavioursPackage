using CharacterCore;
using CommonCore;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BTBrainBase : MonoBehaviour, IBTBrain, IInteractionPerformer
    {
        [SerializeField] float ResourceCapacity = 50.0f;

        public Blackboard<FastName> LinkedBlackboard { get; protected set; }
        public INavigationInterface Navigation { get; protected set; }
        public ILookHandler LookAtHandler { get; protected set; }
        public IInteractionSelector InteractionInterface { get; protected set; }
        public IInteractionPerformer PerformerInterface { get; protected set; }
        public IAnimationInterface AnimationInterface { get; protected set; }

        public IBehaviourTree LinkedBehaviourTree { get; protected set; } = new BTInstance();

        public string DebugDisplayName => gameObject.name;

        public GameObject Self => gameObject;

        public string DisplayName => gameObject.name;

        public Vector3 PerformerLocation => transform.position;

        void Start()
        {
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

            LinkedBlackboard = BlackboardManager.GetIndividualBlackboard(this);

            ServiceLocator.RegisterService(LinkedBlackboard, gameObject);
            ServiceLocator.RegisterService<IInteractionPerformer>(this, gameObject);

            LinkedBlackboard.Set(CommonCore.Names.Self, gameObject);

            LinkedBlackboard.Set(CommonCore.Names.CurrentLocation, transform.position);
            LinkedBlackboard.Set(CommonCore.Names.HomeLocation, transform.position);

            LinkedBlackboard.Set(CommonCore.Names.MoveToLocation, CommonCore.Constants.InvalidVector3Position);
            LinkedBlackboard.Set(CommonCore.Names.MoveToEndOrientation, CommonCore.Constants.InvalidVector3Position);

            LinkedBlackboard.Set(CommonCore.Names.Target_GameObject, (GameObject)null);
            LinkedBlackboard.Set(CommonCore.Names.Target_Position, CommonCore.Constants.InvalidVector3Position);

            LinkedBlackboard.Set(CommonCore.Names.Awareness_PreviousBestTarget, (GameObject)null);
            LinkedBlackboard.Set(CommonCore.Names.Awareness_BestTarget, (GameObject)null);

            LinkedBlackboard.Set(CommonCore.Names.LookAt_GameObject, (GameObject)null);
            LinkedBlackboard.Set(CommonCore.Names.LookAt_Position, CommonCore.Constants.InvalidVector3Position);

            LinkedBlackboard.Set(CommonCore.Names.Interaction_Interactable, (IInteractable)null);
            LinkedBlackboard.Set(CommonCore.Names.Interaction_Type, (IInteraction)null);
            LinkedBlackboard.Set(CommonCore.Names.Interaction_Point, (IInteractionPoint)null);

            LinkedBlackboard.SetGeneric(CommonCore.Names.Resource_FocusType, CommonCore.Resources.EType.Unknown);
            LinkedBlackboard.Set(CommonCore.Names.Resource_FocusSource, (GameObject)null);
            LinkedBlackboard.Set(CommonCore.Names.Resource_FocusStorage, (GameObject)null);

            // populate the inventory
            var ResourceNames = System.Enum.GetNames(typeof(CommonCore.Resources.EType));
            foreach (var ResourceName in ResourceNames)
            {
                if (ResourceName == CommonCore.Resources.EType.Unknown.ToString())
                    continue;

                LinkedBlackboard.Set(new FastName($"Self.Inventory.{ResourceName}.Held"), 0f);
                LinkedBlackboard.Set(new FastName($"Self.Inventory.{ResourceName}.Capacity"), ResourceCapacity);
            }

            ConfigureBlackboard();

            LinkedBehaviourTree.Initialise(Navigation, LookAtHandler, InteractionInterface, PerformerInterface, AnimationInterface, LinkedBlackboard);

            ConfigureBehaviourTree();

            ResetBehaviourTree();

            ConfigureBrain();

            ServiceLocator.AsyncLocateService<IGameDebugger>((ILocatableService InDebugger) =>
            {
                (InDebugger as IGameDebugger).RegisterSource(this);
            });
        }

        void OnDestroy()
        {
            ServiceLocator.AsyncLocateService<IGameDebugger>((ILocatableService InDebugger) =>
            {
                (InDebugger as IGameDebugger).UnregisterSource(this);
            });
        }

        void Update()
        {
            LinkedBlackboard.Set(CommonCore.Names.CurrentLocation, transform.position);

            TickBrain(Time.deltaTime);
        }

        public void GetDebuggableObjectContent(IGameDebugger InDebugger, bool bInIsSelected)
        {
            if (!bInIsSelected)
                return;

            LinkedBehaviourTree.GatherDebugData(InDebugger, bInIsSelected);
            LinkedBlackboard.GatherDebugData(InDebugger, bInIsSelected);
        }

        protected void TickBrain(float InDeltaTime)
        {
            OnPreTickBrain(InDeltaTime);

            var Result = LinkedBehaviourTree.Tick(InDeltaTime);

            if (Result == EBTNodeResult.Succeeded)
            {
                ResetBehaviourTree();
                OnBehaviourTreeCompleted_Finished();
            }
            else if (Result == EBTNodeResult.Failed)
            {
                ResetBehaviourTree();
                OnBehaviourTreeCompleted_Failed();
            }

            OnPostTickBrain(InDeltaTime);
        }

        protected void ResetBehaviourTree()
        {
            LinkedBehaviourTree.Reset();
            OnBehaviourTreeReset();
        }

        protected IBTNode AddChildToRootNode(IBTNode InNode)
        {
            return LinkedBehaviourTree.AddChildToRootNode(InNode);
        }

        protected abstract void ConfigureBlackboard();
        protected abstract void ConfigureBehaviourTree();
        protected abstract void ConfigureBrain();

        protected virtual void OnBehaviourTreeCompleted_Finished() { }
        protected virtual void OnBehaviourTreeCompleted_Failed() { }
        protected virtual void OnBehaviourTreeReset() { }

        protected virtual void OnPreTickBrain(float InDeltaTime) { }
        protected virtual void OnPostTickBrain(float InDeltaTime) { }
    }
}
