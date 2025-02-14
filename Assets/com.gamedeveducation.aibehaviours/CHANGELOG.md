## [5.1.0] - 14th August 2024
- Added AnimatorBridge and Animation Interface
- Added animation support to interactions
- Added Behaviour Tree node to perform animations

## [5.0.0] - 14th July 2024
- Added new interactable system
- Removed old Smart Object system

## [4.4.0] - 13th July 2024
- Expand support for setting a destination orientation when moving
- Add support for storing interfaces that implement IBlackboardStorable into the blackboard
- Fix movement being considered done during end of path reorienting
- Fix GOAP plans not sending stop notifications if they became invalid during execution

## [4.3.0] - 12th July 2024
- Refactor the POI/Look At system into the Character Core and moved behaviours for it to their base systems

## [4.2.0] - Standalone State Machines
- Add full Blackboard support to the State Machine
- Added new FSM State SMState_SMContainer which supports embedding a state machine within a state machine (no depth limit)
- Expanded SMState_Parallel to support ignoring failing states as well as options to never finish
- Added support for Standalone State Machines with example scenes

## [4.1.0] - Game Debugger Editor Window
- Added support for an Editor Window for the Game Debugger (thankyou again to SiliconOrchid for the suggestion and sharing your approach!)

## [4.0.2] - Initial release of Behaviour Trees
- Major updates to folder structure
- Added new Behaviour Tree system
- Added Behaviour Tree based versions of all GOAP actions
- Added Standalone Behaviour Tree based versions of the behaviours
- Fixed Navigation_UnityNavMesh incorrectly animating after a failed pathfind
- Consistency pass on naming for the linked blackboards
- Fixes #8 - Add a physics raycast before the navmesh sampling to allow shorter ranges to be used
- Reduce all navmesh search ranges (from 5m to 2m)
- Fix interaction interrupting if there was only one valid that we were doing
- Fix decorators defaulting to having failed causing unintended resets

## [4.0.1] - Baseline for Behaviour Tree System
- Setup all scenes for BTs

## [4.0.0] - 19th April 2024
- Update to Unity 2023.2.18f1
- Restructure the Game Debugger to make it easy to swap in additional UI interfaces
- Add tracking of if we have a destination set to the INavigationInterface
- Fixed NavMeshAgent stoppingDistance being out of sync (was always the previous one not the one for the current path)
- Add AddComponentMenu support to goals and actions

## [3.1.0] - 3rd February 2024
- Fixes #3 - Navigability/Range to SmartObject Interaction point (reported by SiliconOrchid)
	- Added IsLocationReachable to INavigationInterface
	- Initial implementation just checks if the location is on the NavMesh. Will add further checks after interactable overhaul.
	- The validation is relatively cheap and will form a basis for larger changes to the interactable system.
- Fixes #5 - Navigation Search Range meaning/function (reported by SiliconOrchid)
	- NavigationSearchRange has been renamed to ValidNavMeshSearchRange and given a tooltip
	- Added support for setting an interactable search range. This is temporarily on the GOAPBrainWrapper pending an overhaul of the interaction system.

## [3.0.3] - 27 January 2024
- Fixes #2 - MonoBehaviourSingletons will de-parent and log an error (in the editor only ) if they are not at the root of the scene. (shoutout to SiliconOrchid for finding the issue)

## [3.0.2] - 26 January 2024
- Fixes #1 - If there are no valid interactables return float.MinValue rather than the default (shoutout to SiliconOrchid for finding the issue)

## [3.0.1] - 7 January 2024
- Code cleanup pass

## [3.0.0] - 7 January 2024
- Added remaining components for the new perception system
- Removed legacy perception system

## [2.2.0] - 7 January 2024
- Work in progress implementation for Perception System (not yet in use)

## [2.1.0] - 7 January 2024
- Add SerializableType (a version of System.Type) that can be serialized and used in the inspector

## [2.0.1] - 6 January 2024
- Switch Name Manager over to Standalone Singleton and enable bootstrapping of it
- Make the Blackboard Manager a locatable service and enable registering of it
- Minor commenting additions in GOAPActionBase

## [2.0.0] - 6 January 2024
- Added Service Locator and Standalone Singleton
- Removed GOAP Navigation Interface (replaced by service lookup)
- Renamed Singleton to MonoBehaviourSingleton

## [1.0.6] - 4 January 2024
- Fix GOAPGoal_GatherResource failing on shutdown
- Run a code cleanup to remove unused usings and enforce consistency
- Add all sample scenes to the build

## [1.0.5] - 19 December 2023
- Remove TextMesh Pro dependency (needs manual install for pre 2023.2)

## [1.0.4] - 19 December 2023
- Fix unintended include of Visual Scripting

## [1.0.3] - 2 December 2023
- Add support to the navigation system for using the agent locomotion except in the start/end orienting states
- Enable the new behaviour by default
- Increase the number of agents spawned in the village to 20

## [1.0.2] - 2 December 2023
- Provide a cleaner interface for adding states to the state machine
- Provide a cleaner interface for adding state status transition and default transitions

## [1.0.1] - 23 November 2023
- Remove incorrect using statement in SMState_Store

## [1.0.0] - 23 November 2023
- Initial Version