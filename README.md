# Interactive Orchestra
## Project Information
Tool in development to aid orchestral conducting students in understanding how to remain on beat with their conducting.

## External Assets

### Included (Dev)
| Asset | Purpose |
| ------ | ------ |
| [VRTK](https://github.com/ExtendRealityLtd/VRTK) | Tool kit used for implementing VR functionality into a scene. Allows easy interchangeability between different VR hardware. |
| [SteamVR](https://github.com/ValveSoftware/steamvr_unity_plugin) | Gives Unity access to SteamVR. | 
| [Oculus Plugin](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) | Allows Oculus functionality within Unity. |

### Included (Design)
| Asset | Purpose |
| ------ | ------ |
| [Orchestra Practice Room 3D Model](https://3dwarehouse.sketchup.com/model/55a4e238148047373166bdf893060179/Orchestra-Room) | 3D model of orchestra practice room environment. |
| [Mannequin Saxophone 3D Model](https://3dwarehouse.sketchup.com/model/da256d509d2e99fd1e54f6541acaa659/Musician-Sax) | 3D model of mannequin saxophonist for the orchestra. |
| [Mannequin Upright Bass 3D Model](https://3dwarehouse.sketchup.com/model/a5aab341c771384c11a0817415275603/Musician-Upright-bass) | 3D model of mannequin bassist for the orchestra. |
| [Mannequin Piano 3D Model](https://3dwarehouse.sketchup.com/model/c04160534b7d1a7eda6d676ff8ea001c/Musician-Piano) | 3D model of mannequin pianist for the orchestra. |
| [Mannequin Violin 3D Model](https://3dwarehouse.sketchup.com/model/36b13c92d0d23f1433eddc4ad251356e/Musician-Violin) | 3D model of mannequin violinist for the orchestra. |
| [Ceiling Tile Texture](https://3dtextures.me/2019/01/29/ceiling-gypsum-001/) | 3D texture for ceiling in the orchestra practice room environment. |
| [O Canada Sheet Music](https://musescore.com/user/326626/scores/291086) | O Canada sheet music to display on music stand. |
| [Wall Texture (Plastic Rough)](https://3dtextures.me/2019/02/15/plaster-rough-001/) | 3D wall texture for the orchestra practice room environment. |


### Excluded (Dev)
The following assets are either necessary or recommended for the project to work as intended, and may be included in the project's final release. However, we cannot distribute them, and they must be acquired elsewhere.

| Asset | Purpose | Optional? |
| ------ | ------ | ------ |
| [AirSig for HTC VIVE & Oculus Rift](https://assetstore.unity.com/packages/tools/input-management/3d-motion-gesture-and-signature-recognition-for-oculus-rift-101504) | Gesture recognition library used for defining the shape of each gesture component. Provides a score of correctness. | :heavy_check_mark: |
| [Wwise 2018.1.3](https://www.audiokinetic.com/download/) | Interactive audio software, used for slowing and speeding up the piece according to the user. | :x: |

### Excluded (Design)

| Asset | Purpose | Optional? |
| ------ | ------ | ------ |
| [Wooden Floor Texture](https://meocloud.pt/link/71c6018b-da47-4c24-94e1-13d037b42461/Wood%20Floor_006_SD/) | 3D texture for wood flooring in the orchestra practice room environment. | :heavy_check_mark: |

## Versioning

- Unity 2018.1.4f1 
- Oculus Plugin 1.36.0
- VRTK 3.3.0
- SteamVR 1.2.3

## Getting Started

1. Make note of Oculus controller: only the grip & the trigger functionalities will be used.
2. Adjust headset for user's comfort.
3. Ensure that the guide button (above the top left of music stand) is selected.
    * Select by making contact with Oculus controller.
4. Use grip functionality to pick up baton (resting at the base of the music stand) via baton grip.
    * User can let go of the grip button after baton has been selected.
5. Press & hold trigger to activate the trail system. The trail system indicates that your gestures are being tracked.
    * Move baton tip along prep beat guide to an approximate speed of 80 beats per minute. This will activate the 4:4 legato guide for the user to follow.
    * Motion baton tip along 4:4 legato static guide in a smooth motion.

## Team

### Faculty:
Dr. Jonathan Girard | Principal Investigator - Director of Orchestral Activities, Assistant Professor of Conducting and Ensembles at the UBC School of Music

### Current EML Student Team:

- Jaehun Song - Project Coordinator
- Julie Khashimova - Designer
- Julia Zhu - Designer

### Past EML Student Team:
- Rithvik Alluri - Developer
- William Beltran - Technical Lead
- Serena Chao - Project Coordinator
- Georgette Espina - Design Lead
- Farhan Kassam - Developer
- Andrea Tang - Technical Lead
- Shavonne Yu - Designer 

## Documentation
For documentation, please visit the GitHub Wiki for this repository: 
https://wiki.ubc.ca/Documentation:Interactive_Orchestra
