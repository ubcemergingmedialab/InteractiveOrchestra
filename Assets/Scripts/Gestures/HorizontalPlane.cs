using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlane : MonoBehaviour {

    #region Variables 
    private bool visible = false;
    private bool flag;
    private Renderer planeRenderer;
    public static List<Vector3> planePositions;
    public TempoController tempoController;
    

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

    /// <summary>
    /// If right controller's velocity is negative, save the position data
    /// </summary>
    void FixedUpdate() { 
        // only track position when user is conducting
       /* if (tempoController.getGestureString() != "PREP") // from VelocityTracker TrackAndStoreVelocity()
        {
            OVRInput.FixedUpdate();
            Vector3 currentVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.GetActiveController());

            Debug.Log("Velocity: " + currentVelocity);
            // if Y component of velocity is negative,
            if (currentVelocity.y <= 0)
            {
                // save position of controller
                // TODO: write function that tracks time elapsed between current time and previous collision with plane
                Vector3 currentPos = OVRInput.GetLocalControllerPosition(OVRInput.GetActiveController());
                Debug.Log("Position: " + currentPos);
                planePositions.Add(currentPos);

                // testing planePositions list
                foreach (Vector3 pp in planePositions)
                {
                    print(pp);
                }
            }
        }*/
    }

    ///// <summary>
    ///// If right controller's position is at y position of plane, save the position data
    ///// </summary>
    //void FixedUpdate()
    //{
    //    // only track position when user is conducting
    //    if (tempoController.getGestureString() != "PREP") // from VelocityTracker TrackAndStoreVelocity()
    //    {
    //        OVRInput.FixedUpdate();
    //        Vector3 currentPos = OVRInput.GetLocalControllerPosition(OVRInput.GetActiveController());
    //        // if Y component of velocity is negative,
    //        if (currentPos.y == transform.position.y)
    //        {
    //            // save position of controller
    //            // TODO: write function that tracks time elapsed between current time and previous collision with plane 
    //            Debug.Log("Position: " + currentPos);
    //            planePositions.Add(currentPos);

    //            // testing planePositions list
    //            foreach (Vector3 pp in planePositions)
    //            {
    //                print(pp);
    //            }
    //        }
    //    }
    //}
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
        flag = false;
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
        if (other.gameObject.CompareTag("BatonSphere"))
        {
            if (flag == false)
            {
                flag = true;
            } else
            {
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

    #endregion
}
