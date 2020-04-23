using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMPredictor : MonoBehaviour {

    private OVRVelocityTracker.ConductorSample m_prevConductorSample;
    private bool m_planeHasBeenSpawned = false;
    private bool m_RegionTwoFinished = false;
    private bool m_RegionOneFinished = false;
    private bool m_RegionOneStart = false;
    private bool m_RegionTwoStart = false;
    private bool m_BPMHasBeenPredicted = false;


    /// <summary>
    /// These weights are obtained from our Ridge Regression algorithm in our python documents
    /// </summary>
    private float[] m_PredictorWeights = new float[]{-4.57478767e-02f,  1.74408086e-02f, -4.65637032e-02f, -4.65637032e-02f,
  7.57208029e-02f, -8.39595146e-02f, -8.23871177e-03f, -1.40348235e-03f,
  8.32269185e-04f,  9.32913799e-05f,  5.57131179e-02f, -3.68119487e-02f,
  2.15619653e-02f,  7.04169271e-02f,  5.49040206e-02f,  1.18360582e+00f,
  1.60565817e-01f};
    private float m_DISTANCE_BETWEEN_MEASUREMENTS = 0.005f;

    private float m_startTime = 0;
    private float m_totalElementsRecorded = 0;
    private float m_SizeOfRegionOne = 0;
    private float m_SizeOfRegionTwo = 0;

    private float m_MedianVelocityRegionOne = 0;
    private float m_MedianVelocityRegionTwo = 0;
    private List<float> m_MedianVelocityRegionOneList = new List<float>();
    private List<float> m_MedianVelocityRegionTwoList = new List<float>();
    private float m_MeanVelocityRegionOne = 0;
    private float m_MeanVelocityRegionTwo = 0;

    private float m_RegionOneDistance = 0;
    private float m_RegionTwoDistance = 0;
    private float m_TotalRegionDistance = 0;
    private float m_MaxAngleRegionOne = 0;
    private float m_MaxAngleRegionTwo = 0;
    private float m_MinAngleRegionTwo = 999;

    private float m_MedianVelocityYRegionOne = 0;
    private float m_MedianVelocityYRegionTwo = 0;
    private float m_MeanVelocityYRegionOne = 0;
    private float m_MeanVelocityYRegionTwo = 0;
    private List<float> m_MedianVelocityYRegionOneList = new List<float>();
    private List<float> m_MedianVelocityYRegionTwoList = new List<float>();

    private float m_TimeEndRegionOne = 0;
    private float m_TimeEndRegionTwo = 0;
    private float m_TimeStartRegionOne = 0;
    private float m_TimeStartRegionTwo = 0;

    private float m_RegionTwoThresholdDistance = 0;
    private float m_RegionOneInitialYPosition = 0;
    private float m_MaxRegionOneYPosition = 0;

    private Vector3 m_previousPosition = new Vector3(0,0,0);
    public Transform m_conductorBaton;
    private float m_prevCollisionTime = 0;
    private Vector3 m_basePlaneCollisionPoint;
    private Vector3 m_previousBatonPosition;
    private Vector3 m_previousControllerPosition;
    private Vector3 m_BP1;

    /// <summary>
    /// Record conductor sample components in prep gesture
    /// </summary>
    /// <param name="device"> Occulus controller </param>
    internal void RecordConductorSample(OVRVelocityTracker.ConductorSample conductorSample, TempoController tp)
    {
        if (!m_BPMHasBeenPredicted)
        {
            if (m_prevConductorSample.velocityMagnitude == 0)
            {
                m_TimeStartRegionOne = conductorSample.timeRelativeToPrep;
                m_RegionOneInitialYPosition = conductorSample.position.y;
                m_BPMHasBeenPredicted = false;
            }
            if (!m_RegionOneFinished)
            {
                m_MeanVelocityRegionOne += conductorSample.velocityMagnitude;
                m_MeanVelocityYRegionOne += conductorSample.velocityVector.y;
                m_MedianVelocityRegionOneList.Add(conductorSample.velocityMagnitude);
                m_MedianVelocityYRegionOneList.Add(conductorSample.velocityVector.y);
                m_MaxAngleRegionOne = Mathf.Max(m_MaxAngleRegionOne, conductorSample.angleToBP1);
                if (m_prevConductorSample.position.y > conductorSample.position.y && m_prevConductorSample.position.y != 0)
                {
                    m_RegionOneFinished = true;
                    // -- Distance
                    m_RegionOneDistance = conductorSample.distanceCoveredSoFar;
                    // -- Time
                    m_TimeEndRegionOne = m_prevConductorSample.timeRelativeToPrep - m_TimeStartRegionOne;
                    m_TimeStartRegionTwo = conductorSample.timeRelativeToPrep - m_TimeStartRegionOne;
                    // -- Median Velocity
                    StartCoroutine(FindMedianGivenList(median => m_MedianVelocityRegionOne = median, m_MedianVelocityRegionOneList));
                    StartCoroutine(FindMedianGivenList(median => m_MedianVelocityYRegionOne = median, m_MedianVelocityYRegionOneList));
                    // -- Mean velocity 
                    m_MeanVelocityRegionOne = m_MeanVelocityRegionOne / m_SizeOfRegionOne;
                    m_MeanVelocityYRegionOne = m_MeanVelocityYRegionOne / m_SizeOfRegionOne;
                    // -- Values for getting Region 2
                    m_RegionTwoThresholdDistance = (m_prevConductorSample.position.y - m_RegionOneInitialYPosition) / 2;
                    m_MaxRegionOneYPosition = m_prevConductorSample.position.y;
                    m_SizeOfRegionTwo++;
                    m_SizeOfRegionOne--;
                    
                }
                m_totalElementsRecorded++;
                m_SizeOfRegionOne++;
                m_prevConductorSample = conductorSample;
            }
            else if (m_RegionOneFinished && !m_RegionTwoFinished)
            {

                m_MeanVelocityRegionTwo += conductorSample.velocityMagnitude;
                m_MeanVelocityYRegionTwo += conductorSample.velocityVector.y;
                m_MaxAngleRegionTwo = Mathf.Max(m_MaxAngleRegionTwo, conductorSample.angleToBP1);
                m_MinAngleRegionTwo = Mathf.Min(m_MinAngleRegionTwo, conductorSample.angleToBP1);
                m_MedianVelocityRegionTwoList.Add(conductorSample.velocityMagnitude);
                m_MedianVelocityYRegionTwoList.Add(conductorSample.velocityVector.y);
                if (Math.Abs(m_MaxRegionOneYPosition - conductorSample.position.y) > m_RegionTwoThresholdDistance)
                {
                    m_RegionTwoFinished = true;
                    // --  Distance
                    m_TotalRegionDistance = conductorSample.distanceCoveredSoFar;
                    m_RegionTwoDistance = m_TotalRegionDistance - m_RegionOneDistance;
                    // -- Time
                    m_TimeEndRegionTwo = conductorSample.timeRelativeToPrep - m_TimeStartRegionOne;
                    // -- Median Velocity
                    StartCoroutine(FindMedianGivenList(median => m_MedianVelocityRegionTwo = median, m_MedianVelocityRegionTwoList));
                    StartCoroutine(FindMedianGivenList(median => m_MedianVelocityYRegionTwo = median, m_MedianVelocityYRegionTwoList));
                    // -- Mean velocity 
                    m_MeanVelocityRegionTwo = m_MeanVelocityRegionTwo / m_SizeOfRegionTwo;
                    m_MeanVelocityYRegionTwo = m_MeanVelocityYRegionTwo / m_SizeOfRegionTwo;
                    
                }

                m_totalElementsRecorded++;
                m_SizeOfRegionTwo++;
                m_prevConductorSample = conductorSample;
            }
            else
            {
                //CalculateBPM(tp);
            }
        }
    }

    /// <summary>
    /// Finally calculate the BPM given the data from the prep beat 
    /// </summary>
    private void CalculateBPM(TempoController tp)
    {
        if(m_MedianVelocityRegionTwo != 0 && m_MedianVelocityYRegionTwo!= 0 && !m_BPMHasBeenPredicted)
        {
            float timeBetweenCollisions = m_TimeEndRegionTwo;
            m_BPMHasBeenPredicted = true;

            int BPM = (int)(60/timeBetweenCollisions);
            //Debug.Log("TimeEndRegionTwo: " + m_TimeEndRegionTwo);
            //Debug.Log("TBC is: " + timeBetweenCollisions);
            //Debug.Log("BPM is: " + BPM);
            if (BPM > 140)
            {
                BPM = 140;
            }
            else if (BPM < 60)
            {
                BPM = 60;
            }
            tp.SetNewBPM(BPM);
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


    /// <summary>
    /// Find median of list of velocities
    /// </summary>
    /// <param name="regionValues"> List of velocities </param>
    /// <returns> median velocity </returns>
    private IEnumerator FindMedianGivenList(System.Action<float> median, List<float> regionValues)
    {
        int halfOfListIndex = regionValues.Count / 2;
        for (int i = 0; i < halfOfListIndex; i++)
        {
            int minIndex = i;
            float minValue = regionValues[i];
            for (int j = i + 1; i<regionValues.Count; i++)
            {
                if (regionValues[j] < minValue)
                {
                    minIndex = j;
                    minValue = regionValues[j];
                    float holder = regionValues[minIndex];
                    regionValues[minIndex] = regionValues[i];
                    regionValues[i] = holder;
                }    
            }
        }
        yield return null;
        median(regionValues[halfOfListIndex]);
    }

    public void ResetBPMPredictor()
    {
        m_BPMHasBeenPredicted = false;
        m_startTime = 0;
        m_totalElementsRecorded = 0;
        m_SizeOfRegionOne = 0;
        m_SizeOfRegionTwo = 0;

        m_MedianVelocityRegionOne = 0;
        m_MedianVelocityRegionTwo = 0;

        m_MeanVelocityRegionOne = 0;
        m_MeanVelocityRegionTwo = 0;

        m_RegionOneDistance = 0;
        m_RegionTwoDistance = 0;
        m_TotalRegionDistance = 0;
        m_MaxAngleRegionOne = 0;
        m_MaxAngleRegionTwo = 0;
        m_MinAngleRegionTwo = 999;

        m_MedianVelocityYRegionOne = 0;
        m_MedianVelocityYRegionTwo = 0;
        m_MeanVelocityYRegionOne = 0;
        m_MeanVelocityYRegionTwo = 0;

        m_TimeEndRegionOne = 0;
        m_TimeEndRegionTwo = 0;
        m_TimeStartRegionOne = 0;
        m_TimeStartRegionTwo = 0;

        m_RegionTwoThresholdDistance = 0;
        m_RegionOneInitialYPosition = 0;
        m_MaxRegionOneYPosition = 0;

        m_SizeOfRegionOne = 0;
        m_SizeOfRegionTwo = 0;
        m_totalElementsRecorded = 0;

        m_RegionOneFinished = false;
        m_RegionTwoFinished = false;
        m_RegionOneStart = false;

        m_MedianVelocityRegionOneList.Clear();
        m_MedianVelocityRegionTwoList.Clear();

        m_prevConductorSample = new OVRVelocityTracker.ConductorSample();
    }
}
