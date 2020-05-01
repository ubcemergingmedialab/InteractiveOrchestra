# Interactive Orchestra
## Project Information
Tool in development to provide a realistic and immersive orchestral conducting experience aimed towards both curious users as well as musical conducting students. 
[Onboarding Information](https://docs.google.com/presentation/d/1Ni8gYi2Y3Hp6HZS5PyQRGanwPO-ND7zmk26XFdmHJX4/edit#slide=id.p) | Click this link for more information about this project and Emerging Media lab @ UBC! 

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
| [O Canada Sheet Music](https://musescore.com/user/326626/scores/291086) | O Canada sheet music to display on music stand. |


### Excluded (Dev)
The following assets are either necessary or recommended for the project to work as intended, and may be included in the project's final release. However, we cannot distribute them, and they must be acquired elsewhere.

| Asset | Purpose | Optional? |
| ------ | ------ | ------ |
| [Wwise 2018.1.3](https://www.audiokinetic.com/download/) | Interactive audio software, used for slowing and speeding up the piece according to the user. | :x: |

### Excluded (Design)

| Asset | Purpose | Optional? |
| ------ | ------ | ------ |
| [Wooden Floor Texture](https://meocloud.pt/link/71c6018b-da47-4c24-94e1-13d037b42461/Wood%20Floor_006_SD/) | 3D texture for wood flooring in the orchestra practice room environment. | :heavy_check_mark: |

## Versioning

- Unity 2018.4.0f LTS 
- Oculus Plugin 1.38.4
- VRTK 3.3.0 
- SteamVR 1.2.3

## Getting Started

(We are currently working on a tutorial scene that will take care of the in-game instructions ie. steps 4 and 5 in order to provide a intuitive experience for the user)
1. Make note of Oculus controller: only the grip & the trigger functionalities will be used. Baton can only be picked 
    * up with right controlller.
2. Adjust headset for user's comfort.
3. Ensure that the guide button (above the top left of music stand) is selected.
    * Select by making contact with Oculus controller.
4. Use grip functionality to pick up baton (resting at the base of the music stand) via baton grip.
    * User can let go of the grip button after baton has been selected.
5. Press & hold trigger to activate the trail system. The trail system indicates that your gestures are being tracked.
    * Move baton tip along prep beat guide to an approximate speed of 80 beats per minute. This will activate the 4:4 legato guide for the user to follow.
    * Motion baton tip along 4:4 legato static guide in a smooth motion. The audio playback will change based on your 
    * conducting speed.
    
## Team

### Faculty:
- Dr. Jonathan Girard | Principal Investigator - Director of Orchestral Activities, Assistant Professor of Conducting and Ensembles at the UBC School of Music
- Dr. Patrick Pennefather | Principal Investigator - Assistant Professor at the Master of Digital Media Program and shared with UBC Theatre and Film.

### Current EML Student Team:

- Dante Cerron - Project Coordinator
- Jaehun Song - Project Coordinator
- Alice Hsiao - UX Designer
- Yousra Alfarra - UX Designer
- Sean Jeon - UX Advisor

### Past EML Student Team:
- Julie Khashimova - Designer
- Julia Zhu - Designer
- Michael Hahn - Developer
- Nikko Dumrique - Developer
- Winston Wu - Developer
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
