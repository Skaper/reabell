Thank you for support our asset!

*IMPORTANT* This asset requires Unity 2018.4.12f1 LTS or higher.

If you have any question about how it works or if you are experiencing any trouble, 
feel free to email us at: inv3ctor@gmail.com
Please do not Upload or share this asset as a package without permission.

If you downloaded this asset illegally for studies or prototype purposes, 
please reconsider purchase if you want to publish your work, you can buy on the AssetStore or the vStore
or send us a email and we can figure something out, you can even post your work on our Forum, 
we will be happy to help and advertise your game.

We have been working on these templates for more than 4 years now and we continue to work on this only because of your support, 
otherwise we will have to find day jobs and we would never had time to work on this, so thank you!

ASSETSTORE: https://www.assetstore.unity3d.com/en/#!/content/44227
VSTORE: https://sellfy.com/invector
FORUM: http://invector.proboards.com/
YOUTUBE: https://www.youtube.com/channel/UCSEoY03WFn7D0m1uMi6DxZQ
WEBSITE: http://www.invector.xyz/
PATREON: https://www.patreon.com/invector
DOCUMENTATION: https://www.invector.xyz/aidocumentation

Invector Team - 2019

FSM AI v1.1.0 CORE UPDATE & IMPROVEMENTS 03/12/2019

- Update shared files to Third Person Templates (Basic, Melee & Shooter) v2.5.0
- New Welcome Window
- New methods to FindTarget
- Add new Decision HasWaypoint
- Improvements EventWithDelay 
- Improvements on AI Aiming behavior
- Improvements in the LookTo behavior
- Improvements in some actions, lookTo and findTarget
- Fix error when openning the FSM Window without any FSM selected
- Fix error when trying to create new Action & Decisions via FSM Menu

-----------------------------------------------------------------------------------------------------

FSM AI v1.0.2 OVERALL IMPROVEMENTS 06/09/2019

- Add Melee Companion FSM example (check companion demo scene)
- Add PlayAnimation Action
- Add GoToDamageSender Action
- Add LookToDamageSender Action
- Add SetDamageSenderAsTarget Action
- Add RotateToTarget Action
- Add CheckNoiseDistance Decision
- Add snapping to the grid for the states in the FSM window
- Add description and category for each Action and Decitions
- Add support to add target without a vHealthController, targets now just need a Collider
- Add 'isImmortal' option in the HealthController
- Add 'LockRotation' AnimatorTag support
- Add option to quickly create Custom Actions & Decisions directly from the FSM Window
- Add Mac OS support to navigate through the FSM Window using CMD+RightClick or Alt+RightClick
- CustomStates are no longer supported, use Actions and Decision instead (ex: use the action ChaseTarget instead of the custom node Chase)
- Improvements in the FSM Window (all actions and decisions now have descriptions)
- Improvements in the Headtrack
- Improvements in the Jump 
- Updated Documentation
- Fix Debug Visual Detection not working on 1.0.1 due to the new IK Adjustments feature

-----------------------------------------------------------------------------------------------------

FSM AI v1.0.1 NEW IK ADJUST FEATURE - HOTFIXS 14/08/2019

- Add IKAdjust new feature to dynamically improve/create new poses for each weapon on 4 different states (standing, standingCrouch, aiming, aimingCrouch)
- Add BodySnap Attachments new feature to make it easier to transfer attachments from one character to another (check shooterMelee demo scene & documentation)
- Removed 'AllowMovementAt' from MeleeAttackControl, you can now use the tags LockMovement and LockRotation on specific attacks
- Add AnimatorTagAdvanced, you can now check a tag using the normalized time of an animation, ex: from 0.2 to 0.7 you can use the tag 'LockRotation' on an attack
- Fix damage field not being display in the vObjectDamage inspector
- Fix AI Headtrack issues
- Fix ShooterWeapon DamageByDistance not working
- Fix FSM Window Editor not drawing the decisions on 2019.x
- Improved MessageSender can now send messages to parent

-----------------------------------------------------------------------------------------------------

FSM AI v1.0 STABLE RELEASE 30/04/2019

- Add Companion AI behavior
- Add Cover behavior 
- Add Multiple Throw behavior (Fire Grenade & Smoke Grenade example)
- Add Shooter secundary shot behavior (grenade launcher example)
- Add shorcut to center the FSM States back to the center-screen (Press 'F') or use the button in the top-right corner 
- Add AI with Bow in the AI_Examples
- New Actions added (GetCover, ShooterAttack, GoToFriend, )
- New Decisions added (HasTarget, TargetIsAttacking, TargetIsCrouched, RandomDecision, CheckTargetDistance, CheckDamage)
- New demo scene showing Companion, Cover & Throw behavior
- Add Crouch for the FPS Example Controller
- Debug tab improved with more options
- Overhaul improvements

-----------------------------------------------------------------------------------------------------

FSM AI v0.3 BETA - 28/12/2018
- Updated core files (Basic, Melee & Shooter)
- Update All Animations & Animator Controller
- Add vAnimatorTag & vAnimatorEvents (check online documentation)

-----------------------------------------------------------------------------------------------------

FSM AI v0.2 BETA - 13/09/2018
- New node editor skin 
- Add option to Mute a Transition 
- Bug Fixes on demo scenes

-----------------------------------------------------------------------------------------------------

FSM AI v0.1 BETA - 13/09/2018
- First Release