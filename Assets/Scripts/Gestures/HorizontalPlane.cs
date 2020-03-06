using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlane : MonoBehaviour {

    #region Variables
    [SerializeField] private ParticleSystem rippleTemplate;
    public Vector3 batonTipPosition;
    private ParticleSystem rippleInPlay;
    private bool visible = false;
    private bool flag = false;
    private bool planeIsEnabled = false;
    private Renderer planeRenderer;
    [SerializeField] private TempoController tempoController;
    public static List<Vector3> planePositions;
    public OVRVelocityTracker velocityTracker;
    #endregion

    #region UnityFunctions
    /// <summary>
    /// Initializes position of plane and list of planePositions
    /// </summary>
    void Awake()
    {
        planeRenderer = GetComponent<Renderer>();
        planeRenderer.enabled = visible && planeIsEnabled;
    }

    #endregion

    #region ClassFunctions

    /// <summary>
    /// Toggle feature that displays/hides horizontal plane
    /// </summary>
    void ToggleView()
    {
        Debug.Log(planeIsEnabled);
        Debug.Log(!visible);
        planeRenderer.enabled = !visible && planeIsEnabled;
    }

    /// <summary>
    /// Creates horizontal plane at (x,y,z) controllerPosition during initial prep beat
    /// </summary>
    /// <param name="controllerPosition">x,y,z position of conducting baton controller</param>
    public void SpawnPlane(Vector3 controllerPosition)
    {
        gameObject.transform.position = velocityTracker.GetBatonObject().transform.position;
        ToggleView();
        tempoController.IsPrepComplete = true;
        PlaneFeedback(controllerPosition,true);
        flag = false;
    }

    /// <summary>
    /// Calls the haptic feedback and ripple feedback
    /// </summary>
    /// <param name="positionOfController"> Position of baton in world space</param>
    /// <param name="isInitialRipple"> Determines whether we're creating ripple or moving it </param>
    public void PlaneFeedback(Vector3 positionOfController, bool isInitialRipple)
    {
        ActivateRipple(velocityTracker.GetBatonObject().transform.position, isInitialRipple);
        StartCoroutine(Haptics(0.5f, 0.5f, 0.1f));
    }

    /// <summary>
    /// Spawns or moves ripple to position var
    /// </summary>
    /// <param name="position"> Position to place ripple</param>
    /// <param name="isInitialRipple"> True instantiates a new ripple, false moves ripple around </param>
    private void ActivateRipple(Vector3 position,bool isInitialRipple)
    {
        if (isInitialRipple)
        {
            Vector3 factor = new Vector3(0f, 0.2f, 0f);
            rippleInPlay = Instantiate(rippleTemplate, position + factor, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
        else
        {
            rippleInPlay.gameObject.SetActive(true);
            rippleInPlay.transform.position = position;
            rippleInPlay.time = 0;
            rippleInPlay.Play();
        }
    }

    /// <summary>
    /// Provides haptic feedback (a short vibration) to the user upon plane collision
    /// </summary>
    IEnumerator Haptics(float frequency, float amplitude, float duration)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.25F);
        ChangeColorToBlueOnCollision();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("COLLIDED");
        if (other.gameObject.CompareTag("BatonSphere"))
        {
            if (flag == false)
            {
                flag = true;
            } 
        }
    }


    public void ChangeColorToBlackOnCollision()
    {
        planeRenderer.material.color = Color.black;
    }

    public void ChangeColorToBlueOnCollision()
    {
        Color altColor = new Color32(92, 214, 255, 255);
        planeRenderer.material.color = altColor;
    }

    public void ToggleEnablePlane()
    {
        planeIsEnabled = !planeIsEnabled;
    }
    #endregion
}