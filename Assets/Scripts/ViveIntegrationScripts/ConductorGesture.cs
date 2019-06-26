using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Sets up the gesture profile to be used for the gesture tracking and to calculate the accuracy score.
/// This script was based on the demo scene DeveloperDefined in the AirSig folder.
/// </summary>
public class ConductorGesture : GestureHandle {

    #region Variables
    public GameObject prep;
    public GameObject gestureGuide;

    private string gestureString;
    private string[] gestures = { "44L1", "44L2", "44L3", "44L4" };
    // Define callback for listening Developer-defined Gesture match event
#endregion

    #region Unity Methods




    /// <summary>
    /// At every frame, update the displayed gesture and BPM. 
    /// If PREP is the registered gesture, change the display gesture from PREP gesture to Static gestures. 
    /// </summary>
    void FixedUpdate()
    {
        UpdateUIandHandleControl();
        // We noticed there was a delay in the piece playing when we called this in TempoController so we made the change to start the piece here
        if (gestureString == "PREP")
        {
            // Need to reset gestureString to "" so that the playPiece does not need to keep being recalled and did not want to keep setting this variable in HandleOnDeveloperDefinedMatch
            gestureString = "";
            prep.SetActive(false);
            gestureGuide.SetActive(true);
        }
    }
#endregion

    #region Class Methods


    /// <summary>
    /// Triggered when piece is stopped. Sets the target back to PREP.
    /// </summary>
    public void Reset()
    {
        prep.SetActive(true);
        gestureGuide.SetActive(false);
        tempoController.setGestureCaptured(false);
    }

    /// <summary>
    /// Callback method that will handle event, where gesture is captured 
    /// </summary>
    /// <param name="gestureId"> Serial ID of gesture </param>
    /// <param name="gesture"> Gesture name (44L1, 44L2 etc...) </param>
    /// <param name="score"> Correctness score </param>
    void HandleOnDeveloperDefinedMatch(long gestureId, string gesture, float score)
    {
        if (gesture != "PREP")
        {
            tempoController.setGestureString(gesture);
            tempoController.setGestureScore(score);
            tempoController.setGestureCaptured(true);
        }
        else
        {
            gestureString = gesture;
        }
    }
#endregion

}
