# The (Great) Fusion #

## Contents ##

This repository is meant to be the place where we (finally) bring all our projects, scripts, assets,
and so, together.  
So far, they all have been in separate places, maintained by separate people. Now is the time to
connect them all to each other.

## The Plan ##

While we are not entirely sure how everything goes, the current plan is the following:

1. Import all projects and everything belonging to them into a single Unity project
2. Eliminate possible duplicates, and/or share code and assets
3. Develop systems to connect things, especially considering cross-scene references
4. Build the actual game by creating a structure of connected rooms, etc., that contain all our
  stuff
5. Test, test, test and then test even more

## Guidelines & Things to consider ##

### Steps/Things to follow when importing projects ###

#### 1. Change the layers ####

- The Unity Layers in the old project have to be changed to exactly match those in the `Fusion`
  project
- This should be done _before_ importing the old project into `Fusion` - otherwise there will be
  warnings and potential other problems
- This is because the layers are "hardcoded", to IDs, within Unity, and cannot be imported correctly
  by default
  - This is not true, however, for Tags - these work correctly (as far as we know)

#### 2. Deal with rigs ####

- Reminder: "Rig", in this case, refers to what you are moving around with the camera in your
  project
  - This might be just a camera, or a more complex game object hierarchy with other things like
    a collider, a player model, etc.
- We can very likely not reuse the rigs that are in individual scenes, like they are, in the
  `Fusion` project - we will only have a single one in the "master scene"
- Thus, we have decided: Rigs in your own project are to be deleted
  - They can be imported into the `Fusion` project fist
  - Then it can be checked if they still contain scripts or Unity Game Objects which are still
    needed
  - Those can then be moved into the single "Player" rig in the "master scene", and _then_ they can
    be eventually deleted
- Just deactivating the rig in Unity in the `Fusion` project is problematic, because Leap Motion has
  multiple scripts that search the scene hierarchy for specific scripts, and then might find the
  wrong, deactivated script in one of the old rigs instead of the one in the "master scene"
- Leap Motion `Attachment Hand`s, and their child objects, do not need to be part of the "Player"
  rig in the "master scene"
  - Thus, they can stay in the projects and scenes they are coming from
  - The `AttachmentHand` scripts will automatically connect to the Leap Motion `Interaction Manager`
    within the "master scene", there is nothing left to do

#### 3. Change the folder structure ####

- We have agreed on a certain kind of folder structure, all projects have to have this structure
- The folder structure of the project to be imported has to be changed, to avoid confusion and
  chaos, _before_ importing the project
- The folder structure is the following:
  - Root folder for _everything_ in the project: `_VirtualSelf`
  - All Unity Asset Store assets stay in the root assets folder, where they are put by the asset
    store - never ever move them around anywhere
  - Within `_VirtualSelf`:
    - `_Scenes`, containing all the scenes of the project
    - `Animations`, `Materials`, `Models`, `Prefabs`, `Scripts`, `Shaders`, `Textures`
      - Each containing their respective type of assets
      - Unity "Physics Materials" go into `Materials\Physics`
    - For all these folders: Create a subfolder for your project, name it the same as your project,
      then put your assets in there
      - Example: `Materials\Ballmaze\...`, `Scripts\Ballmaze\...`, etc.
    - Obsolete assets, that should still be included, can be placed in folders called `Obsolete`
- After the folder structure has been changed, make sure that everything still works fine, and make
  sure that the new structure really matches the one in this repository - then this step is done

#### 4. Asset Store assets ####

- When exporting your project, do _not_ include Unity Asset Store assets
- We can add these to this repository separately, and then also make sure that we get the newest
  version
- If this is done differently, there's a danger that multiple versions of an Asset Store asset get
  into the `Fusion` project at the same time, or even that an older version overrides a more recent
  one
- Make sure to document, for every one of these assets, what files of it your project exactly needs
  - Often, only parts are needed, and potentially significant amounts of space can be saved by not
    just "blindly" importing everything

#### 5. Dealing with lights ####

- Lights that are in the hierarchies of your scenes will work in the `Fusion` project correctly,
  automatically, as long as they are already included at "edit time"
- However, if you _spawn_ lights, at _runtime_:
  - The `BehindPortal` layer of these lights has to be deactivated by your code!
  - Otherwise, the lights will also light the scene loaded in the portal

#### 6. Exporting & Importing ####

- To actually export your project, let Unity export it as a `.unitypackage` file
- This will make sure that all references within the project will be correctly treated and kept
- Then, just import the unitypackage file into the Unity project of this repository

### Namespaces ###

- We have a "group-global" namespace that _all_ code should be part of: `VirtualSelf`
- _No_ code should ever be in the global (empty) namespace!
  - This is a general guideline for writing C# code (it's basically the same for C++, as well), not
    just specific to this project
- Additionally, be wary of naming any class the same as a Unity-internal one (regardless of
  namespaces)
  - This can potentially break things
  - While namespaces prevent normal code name clashes, things like Unity's
    `UnityEngine.Object.FindObjectOfType` might return wrong results, in the worst case
  - If there is a class name clash, Unity _should_ print a warning in the console
    - However, I haven't tested this extensively, so maybe under certain circumstances, it won't
  - Example: A class named `Component` would clash with `UnityEngine.Component`
- Also note: Namespaces are _not_ like Java packages!
  - They are mostly used to avoid naming conflicts, and very loosely structure code together (e.g.
    under your organization)
- We did not decide on any other namespace rules or guidelines yet, so do what you want

___
