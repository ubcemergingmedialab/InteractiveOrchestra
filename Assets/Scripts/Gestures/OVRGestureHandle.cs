using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class OVRGestureHandle : MonoBehaviour {

    #region Variables
    
    public TempoController tempoController;

    // Reference to the vive right hand controller for handing key pressing
    // public SteamVR_TrackedObject rightHandControl;
    public ParticleSystem track;

    public SteamVR_TrackedObject rightHandControl;

    public OVRVelocityTracker velocityTracker;

    private bool tracking = false;
    #endregion

    #region Class Functions


    /// <summary>
    /// Checks for the existence of a right controller, used for displaying particles. 
    /// </summary>
    protected void UpdateUIandHandleControl(string gestureString) {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            Application.Quit();
        }
        if (-1 != (int)rightHandControl.index)
        {
            //Debug.Log("Tracking Velocity");
            var device = OVRInput.GetActiveController();
            

            float triggerKeyValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            if (triggerKeyValue > 0.8f)
            {
                

                velocityTracker.StoreConductorSample(gestureString, device); 

                velocityTracker.GetTimeSincePrevCollisionWithBasePlane(device);
                track.Play();
                tracking = true;
            }
            else if (triggerKeyValue < 0.1f && tracking)
            {
                // TODO: Method that stops recording and changes the trial. 
                //Debug.Log("let go");
                velocityTracker.StoreCurrentTrial();
                track.Stop();
                tracking = false;
            }
        }
    }
    #endregion
}
