using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlane : MonoBehaviour {

    #region Variables 
    private bool visible = false;
    private bool flag = false;
    private bool planeIsEnabled = false;
    private Renderer planeRenderer;
    [SerializeField] private TempoController tempoController;
    public static List<Vector3> planePositions;

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
    // TODO: test PlaneButton

    /// <summary>
    /// Toggle feature that displays/hides horizontal plane
    /// </summary>
    void ToggleView()
    {
        //visible = !visible;
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
        Debug.Log("Spawn plane was called! ");
        gameObject.transform.position = controllerPosition;
        ToggleView();
        tempoController.isPrepComplete = true;
        PlaneFeedback();
        flag = false;
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
        Debug.Log("Print 1");
        yield return new WaitForSeconds(duration);
        Debug.Log("Print 2");
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
    
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.25F);
        ChangeColorToBlueOnCollision();
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("BatonSphere") || other.gameObject.CompareTag("BatonSphere_001"))
    //    {
    //        if (flag == false)
    //        {
    //            flag = true;
    //        } else
    //        {
    //            ChangeColorToBlackOnCollision();
    //            StartCoroutine(Timer());
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("COLLIDED");
        if (other.gameObject.CompareTag("BatonSphere"))
        {
            //Debug.Log("=============");
            //Debug.Log(flag);
            if (flag == true) {
                //Debug.Log("flag is null"); 
            }
            if (flag == null){
                //Debug.Log("flag is null");
            }
            if (flag == false)
            {
                //Debug.Log("Flag set to true");
                flag = true;
            } else
            {
                //Debug.Log("+++++++++++++++");
                ChangeColorToBlackOnCollision();
                StartCoroutine(Timer());
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
