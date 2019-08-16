using System.Collections.Generic;
using UnityEngine;


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
        // We noticed there was a delay in the piece playing when we called this in TempoController so we made the change to start the piece here
        // tempoController.playPiece();
        if (showGuide)
            {
                prep.SetActive(false);
                gestureGuide.SetActive(true);
            }
        
    }
    #endregion


    #region Class functions

    /// <summary>
    /// Triggered when piece is stopped. Sets the target back to PREP.
    /// </summary>
    public void Reset()
    {

        if (showGuide)
        {
            prep.SetActive(true);
            gestureGuide.SetActive(false);
        }
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