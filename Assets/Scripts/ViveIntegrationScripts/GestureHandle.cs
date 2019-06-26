using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  Based on the example script from AirSig BasedGestureHandle
///  Mostly handles the drawing of the trail for the gesture
/// </summary>
public class GestureHandle : MonoBehaviour {

    #region Variables
    public TempoController tempoController;
    // Reference to AirSigManager for setting operation mode and registering listener

    // Reference to the vive right hand controller for handing key pressing
    public SteamVR_TrackedObject rightHandControl;

    public ParticleSystem track;
    #endregion

    #region Class Methods


    /// <summary>
    /// Checks for the existence of a right controller, used for displaying particles. 
    /// </summary>
    protected void UpdateUIandHandleControl() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            Application.Quit();
        }

        if (-1 != (int)rightHandControl.index) {
            var device = SteamVR_Controller.Input((int)rightHandControl.index);
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)) {
                track.Clear();
                track.Play();
            } else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)) {
                track.Stop();
            }
        }
    }
    #endregion

}
