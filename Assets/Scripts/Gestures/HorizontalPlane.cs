using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlane : MonoBehaviour {

    #region Variables 
    private bool visible = false;
    private Renderer planeRenderer;

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

    public void SpawnPlane(Vector3 controllerPosition)
    {
        gameObject.transform.position = controllerPosition;
        ToggleView();  
    } 

    #endregion
}
