using System.Collections.Generic;
using UnityEngine;
using VRTK.Controllables.PhysicsBased;

/// <summary>
/// Sets up the gesture profile to be used for the gesture tracking and to calculate the accuracy score.
/// This script was based on the demo scene DeveloperDefined in the AirSig folder.
/// </summary>
public class OVRConductorGesture : OVRGestureHandle
{
    #region Variables
    public GameObject prep;
    public GameObject gestureGuide;
    string gestureString;
    bool showGuide;
    string[] gestures = { "44L1", "44L2", "44L3", "44L4" };
    //[SerializeField] private VRTK_PhysicsSlider vRTK_PhysicsSlider;
    // Callback for receiving signature/gesture progression or identification results
    #endregion

    #region Unity functions
    /// <summary>
    /// Initialize AirSig functionality. 
    /// </summary>
    void Awake() {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        // Configure AirSig by specifying target 

        showGuide = true;
    }


    /// <summary>
    /// At every frame, update the displayed gesture and BPM. 
    /// If PREP is the registered gesture, change the display gesture from PREP gesture to Static gestures. 
    /// </summary>
    void FixedUpdate()
    {
        UpdateUIandHandleControl(gestureString);   
    }
    #endregion


    #region Class functions

    /// <summary>
    /// Triggered when piece is stopped. Sets the target back to PREP.
    /// </summary>
    public void Reset()
    {
        tempoController.setGestureCaptured(false);
    }

    /// <summary>
    /// Toggles the static guide on the screen on or off. 
    /// </summary>
    public void isGuideDisplayed()
    {
        showGuide = !showGuide;
    }
    #endregion
}