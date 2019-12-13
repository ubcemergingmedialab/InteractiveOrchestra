using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class OVRGestureHandle : MonoBehaviour {

    #region Variables
    [SerializeField] private SteamVR_TrackedObject rightHandControl;
    // Reference to the vive right hand controller for handing key pressing
    // public SteamVR_TrackedObject rightHandControl;
    [SerializeField] private ParticleSystem batonTrail;
    
    public OVRVelocityTracker velocityTracker;

    private bool tracking = false;

    public static bool songOver = false;
    #endregion

    #region Class Functions

    /// <summary>
    /// Checks for the existence of a right controller, used for displaying particles. 
    /// </summary>
    void FixedUpdate() { 
        if (Input.GetKeyUp(KeyCode.Escape)) {
            Application.Quit();
        }
        if (-1 != (int)rightHandControl.index)
        {
            var device = OVRInput.Controller.RTouch;

            float triggerKeyValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            if (triggerKeyValue > 0.8f)
            {
                velocityTracker.CollectConductorSamples(device); 
                velocityTracker.SetTimeSincePrevCollisionWithBasePlane(device);
                batonTrail.Play();
                tracking = true;
            }
            else if (triggerKeyValue < 0.1f && tracking)
            {
                velocityTracker.StoreCurrentTrial();
                batonTrail.Stop();
                velocityTracker.RemovePlane();
                tracking = false;
            }
        }
    }

    public void StopParticles(float dummyParam)
    {
        batonTrail.Stop();
        songOver = true;
    }

    private void Awake()
    {
        TempoController.PieceStop += StopParticles;
    }

    #endregion

}
