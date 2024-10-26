using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BTNodeBase : IBTNode
    {
        public GameObject Self => LinkedBlackBoard.GetGameObject(CommonCore.Names.Self);

        public IBehaviourTree OwningTree {  get; protected set; }

        public Blackboard<FastName> LinkedBlackBoard => OwningTree.LinkedBlackboard;

        public EBTNodeResult LastStatus { get; protected set; } = EBTNodeResult.Unintialized;
        protected EBTNodeTickPhase CurrentTickPhase { get; set; } = EBTNodeTickPhase.WaitingForNextTick;

        protected List<IBTService> AlwaysOnServices;
        protected List<IBTDecorator> Decorators;
        protected List<IBTService> GeneralServices;

        public bool HasFinished => (LastStatus == EBTNodeResult.Succeeded || LastStatus == EBTNodeResult.Failed);
        public abstract bool HasChidlren { get; }

        public abstract string DebugDisplayName { get; protected set; }

        protected bool bDecoratorAllowRunning = false;
        protected bool bCanSendExitNotification = false;

        public IBTNode AddDecorator(IBTDecorator InDecorator)
        {
            if (Decorators == null)
                Decorators = new();

            InDecorator.SetOwningTree(OwningTree);
            Decorators.Add(InDecorator);

            return this;
        }

        public IBTNode AddService(IBTService InService, bool bInIsAlwaysOn = false)
        {
            InService.SetOwningTree(OwningTree);

            if (bInIsAlwaysOn)
            {
                if (AlwaysOnServices == null)
                    AlwaysOnServices = new();

                AlwaysOnServices.Add(InService);
            } else
            {
                if (GeneralServices == null)
                    GeneralServices = new();

                GeneralServices.Add(InService);
            }

            return this;
        }

        public bool DoDecoratorsNowPermitRunning(float InDeltaTime)
        {
            // If the decorators already allowed running then no need to check
            if (bDecoratorAllowRunning)
                return false;

            // Update always on services
            if (!OnTick_AlwaysOnServices(InDeltaTime))
                return false;

            // Check decorators
            if (!OnTick_Decorators(InDeltaTime)) 
                return false;

            return true;
        }

        public void GatherDebugData(IGameDebugger InDebugger, bool bInIsSelected)
        {
            if (bInIsSelected)
            {
                InDebugger.PushIndent();

                if (AlwaysOnServices != null)
                {
                    foreach (var service in AlwaysOnServices)
                        service.GatherDebugData(InDebugger, bInIsSelected);
                }

                if (Decorators != null)
                {
                    foreach (var decorator in Decorators)
                        decorator.GatherDebugData(InDebugger, bInIsSelected);
                }

                if (GeneralServices != null)
                {
                    foreach (var service in GeneralServices)
                        service.GatherDebugData(InDebugger, bInIsSelected);
                }

                GatherDebugDataInternal(InDebugger, bInIsSelected);

                InDebugger.PopIndent();
            }
        }

        protected virtual void GatherDebugDataInternal(IGameDebugger InDebugger, bool bInIsSelected)
        {
            
        }

        public void Reset()
        {
            LastStatus = EBTNodeResult.ReadyToTick;
        }

        public void SetOwningTree(IBehaviourTree InOwningTree)
        {
            OwningTree = InOwningTree;
        }

        public EBTNodeResult Tick(float InDeltaTime)
        {
            // First time running, reset the node
            if (LastStatus == EBTNodeResult.Unintialized)
                Reset();

            CurrentTickPhase = EBTNodeTickPhase.AlwaysOnServices;
            if (!OnTick_AlwaysOnServices(InDeltaTime))
            {
                LastStatus = EBTNodeResult.Failed;
                return OnTickReturn(LastStatus);
            }

            CurrentTickPhase = EBTNodeTickPhase.Deocrators;
            if(!OnTick_Decorators(InDeltaTime))
            {
                LastStatus = EBTNodeResult.Failed;

                // Node has previously run and now is not permitted to
                if (bDecoratorAllowRunning && bCanSendExitNotification)
                    OnExit();

                bDecoratorAllowRunning = false;

                return OnTickReturn(LastStatus);
            }

            // If the decorators have changed to permit running then reset the node
            if (!bDecoratorAllowRunning)
            {
                Reset();

                bDecoratorAllowRunning = true;
            }

            // Have we already finished?
            if (HasFinished)
                return OnTickReturn(LastStatus);

            CurrentTickPhase = EBTNodeTickPhase.GeneralServices;
            if (!OnTick_GeneralServices(InDeltaTime))
                return OnTickReturn(EBTNodeResult.Failed);


            // Node has never been ticked?
            if (LastStatus == EBTNodeResult.ReadyToTick)
            {
                OnEnter();

                if (HasFinished)
                    return OnTickReturn(LastStatus);
            }

            CurrentTickPhase = EBTNodeTickPhase.NoodeLogic;
            if (OnTick_NodeLogic(InDeltaTime))
                return OnTickReturn(EBTNodeResult.Failed);

            if (HasChidlren)
            {
                CurrentTickPhase = EBTNodeTickPhase.Children;
                if (!OnTick_Children(InDeltaTime))
                    return OnTickReturn(LastStatus);
            }

            CurrentTickPhase = EBTNodeTickPhase.WaitingForNextTick;

            return OnTickReturn(LastStatus);
        }

        protected virtual EBTNodeResult OnTickReturn(EBTNodeResult InProvisionalResult)
        {
            EBTNodeResult FinalResult = InProvisionalResult;
            CurrentTickPhase = EBTNodeTickPhase.WaitingForNextTick;

            if (Decorators != null)
            {
                foreach (var Decorator in Decorators)
                {
                    if (Decorator.CanPostProcessTickResult)
                        FinalResult = Decorator.PostProcessTickResult(FinalResult);
                }
            }

            if (bCanSendExitNotification && HasFinished)
                OnExit();

            return FinalResult;
        }

        protected virtual bool OnTick_AlwaysOnServices(float InDeltaTime)
        {
            if (AlwaysOnServices != null)
            {
                foreach(var Service in AlwaysOnServices)
                {
                    if (!Service.Tick(InDeltaTime))
                        return false;
                }
            }

            return true;
        }

        protected virtual bool OnTick_GeneralServices(float InDeltaTime)
        {
            if (AlwaysOnServices != null)
            {
                foreach (var Service in GeneralServices)
                {
                    if (!Service.Tick(InDeltaTime))
                        return false;
                }
            }

            return true;
        }

        protected virtual bool OnTick_Decorators(float InDeltaTime)
        {
            if (AlwaysOnServices != null)
            {
                foreach (var Decorator in Decorators)
                {
                    if (!Decorator.Tick(InDeltaTime))
                        return false;
                }
            }

            return true;
        }

        protected abstract bool OnTick_NodeLogic(float InDeltaTime);
        protected abstract bool OnTick_Children(float InDeltaTime);

        protected virtual void OnEnter()
        {
            bCanSendExitNotification = true;
        }

        protected virtual void OnExit()
        {
            bCanSendExitNotification = false;
        }
    }
}
