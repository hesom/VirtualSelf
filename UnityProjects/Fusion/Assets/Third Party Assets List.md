# Third Party Assets List #

## Preface ##

This is a list of all third-party assets that are currently part of the project template (and thus,
of our game). The assets might be downloaded from the Unity Asset Store, or downloaded and included
externally, etc.  
Each asset in the list should include the following information:

- The name of the asset
- The version of the asset
- Whether the asset depends on some other asset, and if so, which one(s)
- Where the asset has been imported from (Asset Store, a file from some website, etc.)
- What exactly from the asset has been imported
  - Everything?
  - Just some parts?

It should be possible to re-do the third-party assets setup, as it is in this template project,
from a fresh, empty Unity project just by reading this file.

## Assets List ##

### Leap Motion: `Core Assets` ###

- **Current Version:** v4.4.0
- **Depends on:** \[Nothing\]
- **Source:** [Leap Motion Website](https://developer.leapmotion.com/unity/)
- **Imported from:** `.unitypackage` file
- **Imported files:** \[Everything\]

### Leap Motion: `Graphic Renderer` ###

- **Current Version:** v0.1.3
- **Depends on:**
  - `Leap Motion: Core Assets`
- **Source:** [Leap Motion Website](https://developer.leapmotion.com/unity/)
- **Imported from:** `.unitypackage` file
- **Imported files:** \[Everything\]

### Leap Motion: `Interaction Engine` ###

- **Current Version:** v1.2.0
- **Depends on:**
  - `Leap Motion: Core Assets`
- **Source:** [Leap Motion Website](https://developer.leapmotion.com/unity/)
- **Imported from:** `.unitypackage` file
- **Imported files:** \[Everything\]

### Leap Motion: `Hands Module` ###

- **Current Version:** v2.1.4
- **Depends on:**
  - `Leap Motion: Core Assets`
- **Source:** [Leap Motion Website](https://developer.leapmotion.com/unity/)
- **Imported from:** `.unitypackage` file
- **Imported files:** \[Everything\]

### Rotorz: `Reorderable List Editor Field for Unity` ###

- **Current Version:** `master` branch as of 2018-08-24
- **Depends on:** \[Nothing\]
- **Source:** [BitBucket](https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity)
- **Imported from:**
  - Created folder structure `Rotorz/Unity3D Reorderable List`
  - Copied over contents of `master` branch into that folder
- **Imported files:**
  - Everything within the `master` branch
  - Deleted `/.gitignore`
    - Not needed in our repository
  - Deleted `/.editorconfig`
    - Not needed in our repository
  - Deleted `/Editor.ReorderableList.csproj`
    - Not needed in our repository
  - Deleted `/Editor.ReorderableList.sln`
    - Not needed in our repository
  - Deleted all files (anywhere within the folder structure) ending in `*.js`
    - JavaScript is no longer supported by Unity (and we wouldn't use it anyway)

### UVRPN: `VRPN wrapper for Unity3D` ###

- **Current Version:** `master` branch as of 2018-08-24
- **Depends on:** \[Nothing\]
- **Source:** [GitHub](https://github.com/hendrik-schulte/UVRPN)
- **Imported from:** `.unitypackage` file (`UVRPN_V_1.5.2_Source`)
- **Imported files:**
  - `Plugins\x86_64\unityVrpn.dll`
    - Nothing else was imported
      - All the rest is useless baggage, or for another operating system
  - Everything else (`Scenes`, `Scripts`, `SharpConfig`)
