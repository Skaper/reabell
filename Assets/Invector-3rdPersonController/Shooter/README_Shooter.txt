Thank you for support our asset!

*IMPORTANT* This asset requires Unity 2018.4.12f1 LTS or higher.

If you have any question about how it works or if you are experiencing any trouble, 
feel free to email us at: inv3ctor@gmail.com
Please do not Upload or share this asset as a package without permission.

If you downloaded this asset illegally for studies or prototype purposes, 
please reconsider purchase if you want to publish your work, you can buy on the AssetStore or the vStore
or send us a email and we can figure something out, you can even post your work on our Forum, 
we will be happy to help and advertise your game.

It has been more than 4 years since the release of v1.0 and we continue to work on this only because of your support, 
otherwise we will have to find day jobs and we would never had time to work on this, so thank you!

ASSETSTORE: https://www.assetstore.unity3d.com/en/#!/content/44227
VSTORE: https://sellfy.com/invector
FORUM: http://invector.proboards.com/
YOUTUBE: https://www.youtube.com/channel/UCSEoY03WFn7D0m1uMi6DxZQ
WEBSITE: http://www.invector.xyz/
PATREON: https://www.patreon.com/invector
ONLINE DOCUMENTATION: https://www.invector.xyz/thirdpersondocumentation

Invector Team - 2019

Shooter v2.5.0a HOTFIXES - 03/05/2020

- Fix Unity 2019.x not drawing the IK Adjustments Handlers in the SceneView
- Fix Controller moving faster when moving diagonally 
- Fix Lock-On not changing to FreeLocomotion when not using the option 'StrafeWhileLockOn'
- Fix FallDamage not calculating the damage correctly
- Fix Jump with RootMotion not working as expected
- Improved Roll rotation while Strafing
- Improved FreeMovement leanning 

* The next update will be focused on the Inventory System ;)

-----------------------------------------------------------------------------------------------------

Shooter v2.5.0 CORE UPDATE & New Features - 02/12/2019

- Project upgraded from 5.6.2 to 2018.4.12 LTS with all the warnings regarding deprecated variables are now fixed
- Big jump from 1.3.2 to 2.5.0 to match other versions of the template
- Add new Welcome Window - Make sure to import the 'ProjectSettings' after you import the package
- Change Mobile, Topdown, 2.5D, ClickToMove and the vMansion examples are now separated add-ons and can be install from the WelcomeWindow/Add-ons
- CORE UPDATE - new methods to Move and Rotate the Character it's now much more fluid
- CORE UPDATE - tpMotor, tpAnimator, tpController, and tpInput was improved and restructured, check the documentation to see the flowchart demonstrating the new structure
- Improved 2.5Input scripts are no longer needed, now you just need to overwrite the methods ControlLocomotionType and ControlRotateType to create new Controller styles
- Improved 2.5Shooter Aim to be more accurate with the cursor position
- Improved ClickToMove with better target selection and collision detection
- Improved Ladder System, now supports inclined angles, new ladder model with each step set to 0.5f of height, add ClimbSpeed & FastClimbSpeed with an option to consume stamina
- Improved components such as generic action, headtrack, ladderAction that previusly had their own updates are now shared with the tpInput update to improve performance
- Improved Debug mode for the controller, new Gizmos for StepOffset and Ground Detection 
- Improved Bow animation transitions while aiming 
- Improved IK Adjustment Window - New Editor and New way to align weapons > Check the Documentation!
- Add New Animations for the Ladder System and better match target to entry and exit
- Add 'AutoReload' option for Shooter Weapons - automatically performs a reload if there is ammo left
- Add 'DontUseReload' option for weapons that use a single amount of bullet 'xx' instead of 'xx/xx'
- Add transition to Sprint if you're Crouching
- Add 'SprintOnlyFree' option to switch to free locomotion while sprinting
- Add 'UseLeanAnimations' option to use the lean left/right animations while turning on free locomotion, disable it when using a TopDownController
- Add 'RotateWithCamera' option to each LocomotionType (Free or Strafe) to make the character Rotate with the Camera forward while standing still
- Add 'MovementSmooth' and 'AnimationSmooth' values for each LocomotionType (Free or Strafe) to have better control of the smoothness you need when moving the character or the locomotion animation speed
- Add 'SnapToGround' optional to snap the collider to the ground, recommended when using complex terrains or inclined ramps
- Add Roll options such as UseRollRootMotion, UseRollGravity, RollSpeed, RollRotationSpeed and TimeToRollAgain
- Add vAnimatorParameters to manage all the animator parameters and convert string to hash to increase performance
- Add new parameters 'IsSprinting' to the animator to know if the character is sprinting or not (useful to create directional transitions)
- Add new options for the Jump/Airborne you can now use the current Rigidbody Velocity to influence on the jump speed direction
- Add 'Jump and Rotate' to rotate while jump/airborne, 'AirSpeed' to control the speed while jump/airborne, 'AirSmooth' to control the smoothness
- Add 'Velocity Multiplier' option for the Ragdoll, it gets the current velocity of the character rigidbody and applies to the ragdoll when enabled, creating a more realistic transition
- Add 'Falling Damage' option in the Jump/Airborn tab
- Fix Reloading when having ammo in the current clip but no ammo left
- Fix bug when entering the ladder and some animations didn't reset (ex: aiming, crouching, blocking)
- Fix Ragdoll Component not being added when creating a ragdoll, now all the colliders are set to CollisionMode Speculate
- ADD-ON Swimming now has an input to swim up/down, demo scene improved with generic action climb instead of the old system to exit the swim
- Removed XInput.dll from the project
- Removed vCharacterStandlone, use vHealthController instead

-----------------------------------------------------------------------------------------------------

Shooter v1.3.2 New Features & Hotfix - 13/08/2019

- Add IKAdjust new feature to dynamically improve/create new poses for each weapon on 4 different states (standing, standingCrouch, aiming, aimingCrouch)
- Add BodySnap Attachments new feature to make it easier to transfer attachments from one character to another (check shooterMelee demo scene & documentation)
- Add FreeMovement and FreeRotation separated methods, also a new LockMovement and LockRotation tag to be used in the AnimatorTags
- Removed 'AllowMovementAt' from MeleeAttackControl, you can now use the tags LockMovement and LockRotation on specific attacks
- Removed 'actions' bool from the ThirdPersonMotor, use customAction instead
- Add AnimatorTagAdvanced, you can now check a tag using the normalized time of an animation, ex: from 0.2 to 0.7 you can use the tag 'LockRotation' on an attack
- Add Animator Controller 'Invector@MeleeCombat_Upperbody' with an example of Attack and Move at the same time, also a transition from a combo that starts on Upperbody and goes to FullBody
- Add CheckItemIsEquipped to InventoryExamples, you can now trigger events if a item is equipped or uniquipped (check shooterMelee demo scene)
- Add example script to verify if the health item can be used only if currentHealth < maxHealth (check shooterMelee demo scene)
- Add ThrowManager is now a prefab to drag and drop inside the ShooterController (easier to setup - check shooterMelee demo scene)
- Fix OnCrouch event being called when Rolling
- Fix Ladder not reseting the speed after exiting 
- Fix damage field not being display in the vObjectDamage inspector
- Fix NaN NaN NaN error with the SimpleMelee AI (also add Wander behavior)
- Fix ShooterWeapon DamageByDistance not working
- Improved MessageSender can now send messages to parent
- Improved StepOffset
- Improved Character Creation window now is already assign with default prefabs for each template
- Improvements in transition between locomotion speeds, use the Acceleration variable in the Locomotion tab > Free/Strafe Speed
- Several improvements in the GenericAction in preparation for the newest Add-on vBuilder

-----------------------------------------------------------------------------------------------------

Shooter v1.3.1 New Features & Hotfix - 03/05/2019

- Add new GenericAction Examples of use (check Basic & Shooter Main Scenes)
- Add New inputs to trigger GenericActions directly from the Trigger (HoldTimer, DoubleButton, ButtonDown, Auto)
- Add option to set a ActionState in the Animator to handle special conditions when playing a CustomAnimation of the GenericAction (check GenericAction ButtonTimer Example)
- Add more Events and Options in the TriggerGenericAction
- Add Separated Equip and Unequip custom animations to play in the ItemListData (EquipAnim can also be triggered when using an Consumable Item)
- Add Separated Equip and Unequip delay time for items in the ItemListData
- Add option display Inventory UI Buttons for items to be Used, Droped, Destroyed and DestroyAfterUse for each item in the ItemListData
- Add option to trigger an animation when using a consumable item on the ItemListData
- Add generic method to open/close Inventory 
- Add vUseItemEventTrigger to use a item from the Inventory UI and trigger an event
- Add option to InfinityAmmo, NoReload or both (with update ammo display)
- Add Event OnCrouch/OnStandUp for the character
- Add Event CheckHealthEvents for the character to trigger a Event when it takes damage lower/higher/equals than value
- Add exposed CurrentHealth and option to Fill up with MaxHealth value on Start or not (to start with lower health)
- Add Volume, SpawnStepMark, SpawnParticles methods on the Footstep to be used with Events (for example, lower the footstep volume when crouched)
- Improved Rotation method of the GenericAction when using the option "Use Trigger Rotation" 
- Improvements on the HealthController
- Improvements in the Headtrack System
- Fix LockInventoryInput method to avoid openning while is attacking, reloading or changing weapon
- Fix WeaponHolderManager timming issues with equipDelay from ItemManager and add debugMode
- Fix Reload Weapon input issue when smashing the button
- Fix LockOn sprite not disabling after reset the scene
- Fix Slow transition from Can't Aim to UpperbodyAim 
- Fix broken links to download the vCrossPlatform & MobileControls prefabs
- Fix Headtrack not ignoring the tags in the animator
- Fix all Mobile demo scenes, broken UI buttons, Mobile Inventories and link to download vCrossPlatform updated
- Fix Ladder Alignment 
- AnimatorControllers Updated with minor changes to work with the new updates of the GenericAction and Inventory Equip/Unequip

-----------------------------------------------------------------------------------------------------

Shooter v1.3.0 New Features! - 13/12/2018

- All Animations (Basic, Melee & Shooter) retarget to VBot 2.0 and improved (crooked fingers, better poses, aim and fire - project weight reduction)
- Add vAnimationEvent & vAnimationEventReceiver to Trigger Events directly from Animation States (Check AssaultRifle Reload example)
- Add vMessageReceiver and vMessageSender you can now send a message to any object and trigger Events - Check the Online Documentation
- Add vDrawHideShooterWeapons to automatically hide weapons (needs vWeaponHolderManager - also possible to be called using Events)
- Add Infinity Ammo option per weapon
- Add Reload per bullet (see Shotgun example)
- Add Reload Time Delay to actually reload the weapon after a period of time 
- Add CancelReload method that can be called using Events for example: OnReceiveDamage or OnStartAction and the animation will be interrupted
- Add Equip/Unequip Delay Time separated for equip and unequip animations for the ItemListData
- Add Continuous Sprint option for the Input 
- Add more Events for the Controller like OnJump, OnCrouch, OnStartSprinting, OnFinishSprinting
- Add ParticleCollision detection to the vObjectDamage
- Add Optional Strong Attack in the MeleeWeapon
- Add OnEnter/Exit Look Events for the vLookTarget script 
- Add Slide as new action example for the Basic demo scene
- Add CustomCameraState parameter in the vTriggerGenericAction to play custom camera state while doing the action
- Fix Arrow streching the scale when parenting on gameObjects that are also scaled
- Fix ShooterController doing punches even if equipped with a ShooterWeapon without a MeleeWeapon component
- Fix Strafe bug when using the Throw Manager
- Fix Camera Culling issue with 2018.x
- Fix GroundDistance not ignoring colliders when IsTrigger was checked
- Arm IK transition improved when aiming

-----------------------------------------------------------------------------------------------------

Shooter v1.2.3 HOTFIX - 12/09/2018

- Project optmization (less/optimized textures)
- Add vWeaponConstrain for NoInventory Collectibles with Rigibody (fix 2018.x handler bug - solution by sjmtech)
- Fix RandomAttack example in the MeleeCombat Animator Controller
- Re-add Waypoint System for the Simple Melee AI 

-----------------------------------------------------------------------------------------------------

Shooter v1.2.2 HOTFIX/AI TEMPLATE PREPARATION - 13/07/2018

- Add menu tab to Import ProjectSettings manually (Unity 2018.x) 
- Add optional bool to use or not .instance with the controller
- Add support for ItemType Melee/Archery in the Inventory_ShooterMobile prefab
- Add SmoothDamp for States to the ThirdPersonCamera
- Add OnStartAction and OnEndAction Events on vGenericAction script
- Fix ScopeView UI rotation bug 
- Fix Ragdoll not being activated on vSpikes
- Fix StepOffset bug 
- Fix Jump 'inplace' bug caused on previous update
- Improved rigidbody movement methods 
- Improved Roll and Jump verifications

-----------------------------------------------------------------------------------------------------

Shooter v1.2.1 HOTFIX/CORE UPDATE - 26/06/2018

- Add core-support to the new AI Template (new asset coming soon)
- Add Footstep Support for multiple Terrains 
- Add new verifications for the ItemManager
- Add ObjectContainer for instantiated objects to avoid polluting the hierarchy in Playmode
- Improved performance on Footstep Detection Material
- Improved Arm IK transition 
- Improved Shoot Input accuracy
- Fixed Throw bug 
- Fixed Holder UnequipDelayTime bug
- Fixed Hipfire Reload while aiming bug
- Fixed Footstep not detecting different Terrains
- Fixed Footstep generic rig type bug 
- Fixed Bow issue not aligning correctly
- Fixed root motion not working on idle states
- Fixed Footstep error "TargetException: Non-static method requires a target" in Unity 2018.1
- Convert smoke legacy particle to shuriken
- Several overall improvements in the Project

-----------------------------------------------------------------------------------------------------

Shooter v1.2.0 CORE UPDATE- 16/03/2018

- add MeleeClickToMove demo scene (Diablo combat style)
- add Jump Multiplier Spring example in the Basic Locomotion scene
- add Archery System for No-Inventory scene (Shooter only)
- add namespace on all vScripts
- add vHealthController (You can use this component to have health into generic objects without the need of a vCharacter which now inherits from the vHealthController)
- add IK offset for left hand
- add OnEnterLadder/Exit Events
- convert Legacy Particles to Shuriken 
- fix rotation bug with Generic Actions and Ladder
- fix Basic Locomotion tab not showing in Mac OS devices
- fix ScopeView rotation when crouched
- fix Ragdoll issues 
- improved Standalone Character 
- improve attack exit transitions smoothness
- update several scripts to avoid over warnings using Unity last API 
- update and fixed several prefabs and scenes
- update project to Unity 5.6.1 

-----------------------------------------------------------------------------------------------------

Shooter v1.1.5 ARCHERY UPDATE - 08/01/2018

* Happy New Year!!

- add archery support with collectable arrows
- add secondary shot with vRifle example 
- add support for charge shot and multiple projectiles (see vRifle example)
- add leaning animations for walk, run and sprint animations
- add new unarmed moveset for free, strafe and crouch 
- add new pick up item animations
- add SetLockShooterInput, SetLockMeleeInput and SetLockBasicInput to call on Events and lock individual inputs
- add ShowCursor, LockCursor and SetLockCameraInput methods to call on Events
- add new CheckGroundMethod with options to Low and High detections levels
- updated AmmoManager to automatically creates a ammoType
- updated vPlatform to work with the FreeClimb Add-on
- updated all animator controllers (*important - make sure to update your old animator based on the new)
- fix camera bugs
- fix animator looping some animations on 2017.3

-----------------------------------------------------------------------------------------------------

Shooter v1.1.4 HOTFIX - 31/10/2017

- changes in the tpInput to update the Adventure Creator & Playmaker Integration
- slopeLimit improved and add slide velocity in the inspector
- fix bugs in the 2.5D scene, player animator needs to be on update mode animatePhysics
- fix lock-on target not exiting lock-on mode with more then 1 target close
- add aimMinDistance for the TopDownShooter

-----------------------------------------------------------------------------------------------------

Shooter v1.1.3 HIPFIRE SHOOTER / HOTFIX - 05/10/2017

- add hipfire shot (ability to shot without aim) 
- add hipfire dispersion (how much the shot will disperse while shooting without aiming)
- add camera sway (random camera movement while aiming) 
- shooter weapon add precision (how much precision the weapon have, 1 means no cameraSway and 0 means maxCameraSway)
- fix onDead event not being called on vCharacterStandalone
- fix ragdoll RemovePhysicsAfterDie option
- fix ragdoll causing error when dying on spikes
- fix weapon holder bug (when pickup a new weapon, the current weapon holder show/hide)

-----------------------------------------------------------------------------------------------------

Shooter v1.1.2 TOPDOWN SHOOTER / HOTFIX - 18/09/2017

- add support for topdown shooter aim height
- add support to quickly change the CameraState using the ChangeCameraState method from tpInput
- add support to create Ragdolls for Generic Rigs 
- add Ragdoll Generic Template if you have several models with the same hierarchy name, add the bone name once and create for every model
- fix weapon handler equip delay time
- fix roll direction bug when using strafe
- fix isAiming not reseting when the weapon is destroyed
- fix isReloading not reseting when the weapon is destroyed
- fix strafeLimit not working when walkByDefault is enable
- change animator update mode back to normal 
- change ColorSpace back to Gama (default color space of Unity)
- minor improvements 

-----------------------------------------------------------------------------------------------------

Shooter v1.1.1 HOTFIX- 30/08/2017

- fix strafe movement speed 
- fix melee manager not causing any damage on generic custom hitboxes
- fix input timer for shooter weapons, it's more precise now (you may need to change the frequency value)
- fix camera jittering when using the MeleeLockOn
- fix OnCheckInvalidAim not displaying the "X" when can't aim
- fix headTrack sync issue with shooterAimIK
- fix katana 3D Model importing errors on 5.5.0
- fix speed of holding melee weapons walk animations
- fix mobile inventory not equiping weapons
- improve shoot coroutines 
- change shootFrequency values of the example weapons
- add free movement with lockOn (use the bool strafeWhileLockOn) 
- add support to shoot bodyParts on generic rigs without using a ragdoll (using vCollisionMessage)
- add API link under the tab Help

-----------------------------------------------------------------------------------------------------

Shooter v1.1.0 TOPDOWN/2.5D/THROW- 17/08/2017

- add 2.5D Shooter Demo scene
- add 2.5D Curved path system
- add camera trigger to change angle
- add Topdown movement with rotation based on mousePosition 
- add Throw system (works like add-on, plug & play)
- add TopDown & Throw System Demo Scene
- add LockMeleeInput & LockShooterInput option to use with events (see topdown example)
- remove ClickToMove from the Core, now different types of controller have their own scripts making the core more clean
- change the GenericAnimation to have a external way to call the method PlayAnimation without input
- improved aim IK behaviour smoothness
- improved camera behaviour smoothness
- improved charater rotation smoothness
- fix Camera StartUsingTransformRotation and StartSmooth options
- fix footstep particle instantiating without layer verification
- fix stamina consumption while crouching or jumping
- fix bullet life settings bugs

-----------------------------------------------------------------------------------------------------

Shooter v1.1d HOTFIX UPDATE - 19/07/2017

- add bullet life settings and options to pass through objects (see vShooterOnly_WITHInventory demo scene)
- add katana 2 hand moveset & attack animations 
- add revival option for both Player & AI
- add new inspector icons for important scripts
- creating a melee controller now adds the meleeManager and lock-on automatically
- footstep has the defaultSurface assign when add the component
- change move speed variables to a internal class 
- fix reaction/recoil/death animations playing without root motion
- fix ragdoll bug disappearing the character after the 2 hit
- fix mouse click not working in the inventory on editor
- fix/improved turnOnSpot verifications

-----------------------------------------------------------------------------------------------------

Shooter v1.1c IMPROVEMENTS UPDATE - 30/6/2017

- add inventory support for Mobile
- add Inventory DemoScene with examples on how to add/remove/equip/unequip/destroy/drop/check items
- add vItemCollectionDisplay prefab (hud text showing what you collect from the vItemCollection)
- add keycard examples to open doors
- add support to revive the Player 
- add option to remove components when the player dies
- add option to start the camera without the player rotation and without lerp
- add input to go up/down for the ladder
- fixed StepOffset Raycast interfering with Triggers making the character float a little bit 
- fixed headtrack look at while aiming bug
- new small features for the scene 2.5D, lock Z axis and remove vertical input
- the Lock-On component is now a add-on and should be attached into the Player instead of the Camera
- improved FootStep logic
- remove all SendMessage calls from the project
- several small fixes and improvements requested by add-on creators to improve compatibility
- controller is now setup to be without root motion by default, to improve the range of adding new custom animations

-----------------------------------------------------------------------------------------------------

Shooter v1.1b HOTFIX/INVENTORY - 5/6/2017

- updated documentation
- improved ladder action component
- improved jump animation transition
- add strong unarmed attack animations
- add option to auto equip melee weapons, and set the equiArea (melee, consumable, etc) 
- add SlotIdentifier on the EquipDisplay so it's easier to know what slot you're in
- fix chests in the melee demo scene
- fix occasionally 360 camera spin after enemy die with lock-on activated
- fix weapon holders bugs 

-----------------------------------------------------------------------------------------------------

Shooter v1.1a HOTFIX - 31/5/2017

- Fix missing BodyMembers in the MeleeManager
- Fix camera reposition to 0, 0, 0 in the Editor
- Add Auto Equip option for the Item & ItemCollection
- Add vRemoveCurrentItem as example to Unequip, Drop or Destroy the current item equipped
- Add vSimpleTrigger script with simple trigger verifications with Events
- Small changes in the vGenericAction 

-----------------------------------------------------------------------------------------------------

Shooter v1.1 - 22/5/2017

- New Action system implemented
- New in-game HUD add 
- New TriggerAction with more options for MatchTarget
- New LadderAction separated from the controller
- New vSkin for the Editors
- Add Events for the Controller
- Add RandomAttacks example
- Add support for Shooter & Melee together without inventory 
- Add support to drag and drop a shooter weapon on start
- Fix Twisted Model on the mobile demo scene
- Fix missing particles prefabs 
- Fix Jump stuck animation in the ShooterMelee Animator
- Removed ActionText from the HUD
- Several minor changes to improve stability

-----------------------------------------------------------------------------------------------------

Changelog v1.0a HOTFIX xx/04/2017

Fix: 
- removed exit time transition of jump on the shooterMelee animator

Changes:
- attack stamina is now deal at the attack state as requested by users
- small changes to improve compatibility with custom add-ons & integrations

-----------------------------------------------------------------------------------------------------

Shooter v1.0 - xx/03/2017
- First Release based on MeleeCombat Template v2.1