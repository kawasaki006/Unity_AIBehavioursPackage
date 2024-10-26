using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum EBTNodeResult
    {
        Unintialized,

        ReadyToTick,
        InProgress,

        Succeeded,
        Failed
    }

    public enum EBTNodeTickPhase
    {
        WaitingForNextTick,

        AlwaysOnServices,
        Deocrators,
        GeneralServices,
        NoodeLogic,
        Children
    }
    public interface IBTNode : IDebuggable
    {
        GameObject Self { get; }
        IBehaviourTree OwningTree { get; }
        Blackboard<FastName> LinkedBlackBoard { get; }

        EBTNodeResult LastStatus { get; }
        bool HasChidlren { get; }


        bool HasFinished {  get; }

        void SetOwningTree(IBehaviourTree InOwningTree);

        bool DoDecoratorsNowPermitRunning(float InDeltaTime);

        void Reset();

        EBTNodeResult Tick(float InDeltaTime);

        IBTNode AddService(IBTService InService, bool bInIsAlwaysOn = false);
        IBTNode AddDecorator(IBTDecorator InDecorator);
    }

}
