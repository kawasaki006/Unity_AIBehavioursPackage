using CharacterCore;
using CommonCore;

namespace BehaviourTree
{
    public interface IBehaviourTree : IDebuggable
    {
        INavigationInterface NavigationInterface { get; }
        ILookHandler LookAtInterface { get; }
        IInteractionSelector InteractionInterface { get; }
        IInteractionPerformer PerformerInterface { get; }
        IAnimationInterface AnimationInterface { get; }
        Blackboard<FastName> LinkedBlackboard { get; }
        IBTNode RootNode { get; }

        void Initialise(INavigationInterface InNavigationInterface, ILookHandler InLookAtInterface,
                        IInteractionSelector InInteractionInterface, IInteractionPerformer InPerformerInterface,
                        IAnimationInterface InAnimationInterface,
                        Blackboard<FastName> InBlackboard, string InRootNodeName = "Root");

        void Reset();

        IBTNode AddChildToRootNode(IBTNode InNode);

        EBTNodeResult Tick(float InDeltaTime);
    }
}
