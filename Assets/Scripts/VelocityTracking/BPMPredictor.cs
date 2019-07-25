using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMPredictor : MonoBehaviour {


    [SerializeField] private HorizontalPlane horizontalPlane;

    private bool planeHasBeenSpawned = false;
    private bool RegionTwoFinished = false;
    private bool RegionOneFinished = false;
    private bool RegionOneStart = false;
    private bool RegionTwoStart = false;
    private float[] PredictorWeights = new float[]{-3.25511626e-02f , 1.15400230e-03f ,- 8.04258180e-02f , 1.46279823e-01f
 ,- 1.84145346e-01f, - 3.78655231e-02f ,- 3.45894887e-04f , 3.38333140e-04f
 ,- 7.83842484e-05f , 4.48474817e-02f ,- 3.15975199e-02f , 1.34321221e-02f
 , 4.50300939e-02f, - 4.23116139e-03f , 1.24455554e+00f, - 1.31935036e+00f,
  9.98110984e-02f };
    private float DISTANCE_BETWEEN_MEASUREMENTS = 0.005f;

    private float startTime = 0;
    private float totalElementsRecorded = 0;
    private float SizeOfRegionOne = 0;
    private float SizeOfRegionTwo = 0;

    private float MedianVelocityRegionOne = 0;
    private float MedianVelocityRegionTwo = 0;
    private float MeanVelocityRegionOne = 0;
    private float MeanVelocityRegionTwo = 0;

    private float RegionOneDistance = 0;
    private float RegionTwoDistance = 0;
    private float TotalRegionDistance = 0;
    private float MaxAngleRegionOne = 0;
    private float MaxAngleRegiontwo = 0;
    private float MinAngleRegionTwo = 0;

    private float MedianVelocityYRegionOne = 0;
    private float MedianVelocityYRegionTwo = 0;
    private float MeanVelocityYRegionOne = 0;
    private float MeanVelocityYRegionTwo = 0;

    private float TimeEndRegionOne = 0;
    private float TimeEndRegionTwo = 0;
    private float TimeStartRegionOne = 0;
    private float TimeStartRegionTwo = 0;
    private float previousYVelocity;

    private Vector3 previousPosition = new Vector3(0,0,0);
    public Transform conductorBaton;
    private float prevCollisionTime = 0;
    private Vector3 basePlaneCollisionPoint;
    private Vector3 previousBatonPosition;

    private Vector3 previousControllerPosition;
    private Vector3 BP1;

    /// <summary>
    /// Record conductor sample components in prep gesture
    /// </summary>
    /// <param name="device"> Occulus controller </param>
    internal void RecordConductorSample(OVRInput.Controller device)
    {
        if (RegionTwoFinished)
        {
            CalculateBPM();
        }
        if (RegionOneFinished)
        {
            StartCoroutine(SetRegionOneValues());
        }

        if (!RegionTwoFinished)
        {
        }


    }

    /// <summary>
    /// Get all values relating to region One 
    /// </summary>
    private IEnumerator SetRegionOneValues()
    {

        yield return null;
    }

    /// <summary>
    /// Finally calculate the BPM given the data from the prep beat 
    /// </summary>
    private void CalculateBPM()
    {
        
    }

    /// <summary>
    /// Finds total distance covered so far in the current trial. Only returns if there have been 'DistanceBetweenMeasurements' units between
    /// the current data point and the latest one in 'samples'
    /// </summary>
    /// <returns> Total distance covered in the current trial </returns>
    private float GetDistanceCoveredSoFar(OVRInput.Controller device)
    {
       
        try
        {
            Vector3 currPosition = OVRInput.GetLocalControllerPosition(device);
            float distanceBetweenDataPoints = GetDistanceBetweenVectors(previousPosition, currPosition);
            if (distanceBetweenDataPoints > DISTANCE_BETWEEN_MEASUREMENTS)
            {
                if (RegionOneStart && !RegionOneFinished)
                {
                    return distanceBetweenDataPoints + RegionOneDistance;
                }
                if (RegionTwoStart && !RegionTwoFinished)
                {
                    return distanceBetweenDataPoints + RegionTwoDistance;
                }
                else
                {
                    return 0;
                }
            }
            else return 0;
        }

        catch (ArgumentOutOfRangeException e)
        {
            Debug.Log("There's nothing in the samples list");
            return 0;
        }

    }

    /// <summary>
    /// Return distance between two positions
    /// </summary>
    /// <param name="lastPosition"> Latest item on samples </param>
    /// <param name="currPosition"> Current Position </param>
    /// <returns> Magnitude of vector between two given positions </returns>
    private float GetDistanceBetweenVectors(Vector3 lastPosition, Vector3 currPosition)
    {
        return (lastPosition - currPosition).magnitude;
    }
}
