using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlane : MonoBehaviour {

    #region Variables 
    private bool visible = false;
    private Renderer planeRenderer;
    [SerializeField] private TempoController tempoController;

    #endregion

    #region UnityFunctions
    /// <summary>
    /// Initializes position of plane and list of planePositions
    /// </summary>
    void Awake()
    { 
        planeRenderer = GetComponent<Renderer>();
        planeRenderer.enabled = visible;
    }
    #endregion

    #region ClassFunctions
    // TODO: test PlaneButton

    /// <summary>
    /// Toggle feature that displays/hides horizontal plane
    /// </summary>
    void ToggleView()
    {
        //visible = !visible;
        planeRenderer.enabled = !visible;
    }

    /// <summary>
    /// Creates horizontal plane at (x,y,z) controllerPosition during initial prep beat 
    /// </summary>
    /// <param name="controllerPosition">x,y,z position of conducting baton controller</param>
    public void SpawnPlane(Vector3 controllerPosition)
    {
        gameObject.transform.position = controllerPosition;
        ToggleView();
        tempoController.isPrepComplete = true;
        PlaneFeedback();
    } 

    public void PlaneFeedback()
    {
        StartCoroutine(Haptics(0.5f, 0.5f, 0.1f));
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
    
    #endregion
}
