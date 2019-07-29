using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMPredictor : MonoBehaviour {


    [SerializeField] private HorizontalPlane m_horizontalPlane;

    private OVRVelocityTracker.ConductorSample m_prevConductorSample;
    private bool m_planeHasBeenSpawned = false;
    private bool m_RegionTwoFinished = false;
    private bool m_RegionOneFinished = false;
    private bool m_RegionOneStart = false;
    private bool m_RegionTwoStart = false;


    private float[] m_PredictorWeights = new float[]{-1.18118476e-02f, -1.32763364e-02f, -2.65996807e-02f, -2.65996807e-02f,
  8.19660675e-02f, -1.26190278e-01f, -4.42242101e-02f ,-1.63708474e-03f,
  2.07700007e-03f, -7.34709437e-04f, 3.30528428e-02f ,-4.22880578e-02f,
  1.45280780e-02f,  7.09113061e-02f , 1.34520906e-01f,  1.16857063e+00f,
 -1.37810550e+00f,  1.26620967e-01f};
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
    internal void RecordConductorSample(OVRVelocityTracker.ConductorSample conductorSample)
    {
        //Debug.Log(m_prevConductorSample.velocityMagnitude);
        if(m_prevConductorSample.velocityMagnitude == 0)
        {
            m_TimeStartRegionOne = conductorSample.timeRelativeToPrep;
            m_RegionOneInitialYPosition = conductorSample.position.y;
            Debug.Log("InitialTimeRecorded");
        }
        if(!m_RegionOneFinished)
        {
            m_MeanVelocityRegionOne += conductorSample.velocityMagnitude;
            m_MeanVelocityYRegionOne += conductorSample.velocityVector.y;
            m_MedianVelocityRegionOneList.Add(conductorSample.velocityMagnitude);
            m_MedianVelocityYRegionOneList.Add(conductorSample.velocityVector.y);
            m_MaxAngleRegionOne = Mathf.Max(m_MaxAngleRegionOne,conductorSample.angleToBP1);
            if (m_prevConductorSample.position.y > conductorSample.position.y && m_prevConductorSample.position.y != 0)
            {
                m_RegionOneFinished = true;
                // Distance
                m_RegionOneDistance = conductorSample.distanceCoveredSoFar;
                // Time
                m_TimeEndRegionOne = m_prevConductorSample.timeRelativeToPrep;
                m_TimeStartRegionTwo = conductorSample.timeRelativeToPrep;
                // Median Velocity
                StartCoroutine(FindMedianGivenList(median => m_MedianVelocityRegionOne = median,m_MedianVelocityRegionOneList));
                StartCoroutine(FindMedianGivenList(median => m_MedianVelocityYRegionOne = median, m_MedianVelocityYRegionOneList));
                // Mean velocity 
                m_MeanVelocityRegionOne = m_MeanVelocityRegionOne / m_SizeOfRegionOne;
                m_MeanVelocityYRegionOne = m_MeanVelocityYRegionOne / m_SizeOfRegionOne;
                // Values for getting Region 2
                m_RegionTwoThresholdDistance = (m_prevConductorSample.position.y - m_RegionOneInitialYPosition)/2;
                m_MaxRegionOneYPosition = m_prevConductorSample.position.y;
                m_SizeOfRegionTwo++;
                m_SizeOfRegionOne--;
                Debug.Log("=====================Region One End==================");
            }
            m_totalElementsRecorded++;
            m_SizeOfRegionOne++;
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
                // Distance
                m_TotalRegionDistance = conductorSample.distanceCoveredSoFar;
                m_RegionTwoDistance = m_TotalRegionDistance - m_RegionOneDistance;
                // Time
                m_TimeEndRegionTwo = conductorSample.timeRelativeToPrep;
                // Median Velocity
                StartCoroutine(FindMedianGivenList(median => m_MedianVelocityRegionTwo = median,m_MedianVelocityRegionTwoList));
                StartCoroutine(FindMedianGivenList(median => m_MedianVelocityYRegionTwo = median, m_MedianVelocityYRegionTwoList));
                // Mean velocity 
                m_MeanVelocityRegionTwo = m_MeanVelocityRegionTwo / m_SizeOfRegionTwo;
                m_MeanVelocityYRegionTwo = m_MeanVelocityYRegionTwo / m_SizeOfRegionTwo;
                Debug.Log("==========================Region Two End========================");
            }

            m_totalElementsRecorded++;
            m_SizeOfRegionTwo++;
        }
        else
        {
            CalculateBPM();
        }
        m_prevConductorSample = conductorSample;
    }

    private void AccumulateMean(float Region, float valueToAccumulate, List<float> listOfValues)
    {
        Region = Region + valueToAccumulate;
        listOfValues.Add(valueToAccumulate);
    }

    /// <summary>
    /// Finally calculate the BPM given the data from the prep beat 
    /// </summary>
    private void CalculateBPM()
    {
        Debug.Log("BOOP DOOP");
        if(m_MedianVelocityRegionTwo != 0 && m_MedianVelocityYRegionTwo!= 0)
        {
            Debug.Log("===========================Results===========================");
            string printThis = String.Format("{0}\n" +
                "{1}\n" +
                "{2}\n" +
                "{3}\n" +
                "{4}\n" +
                "{5}\n" +
                "{6}\n" +
                "{7}\n" +
                "{8}\n" +
                "{9}\n" +
                "{10}\n" +
                "{11}\n" +
                "{12}\n" +
                "{13}\n" +
                "{14}\n" +
                "{15}\n" +
                "{16}\n" +
                "{17}\n", 
                m_MedianVelocityRegionOne,
                m_MedianVelocityRegionTwo,
                m_MeanVelocityRegionOne,
                m_MeanVelocityRegionTwo,

                m_RegionOneDistance,
                m_RegionTwoDistance,
                m_TotalRegionDistance,
                m_MaxAngleRegionOne,
                m_MaxAngleRegionTwo,
                m_MinAngleRegionTwo,

                m_MedianVelocityYRegionOne,
                m_MedianVelocityYRegionTwo,
                m_MeanVelocityYRegionOne,
                m_MeanVelocityYRegionTwo,

                m_TimeEndRegionOne,
                m_TimeEndRegionTwo,
                m_TimeStartRegionOne,
                m_TimeStartRegionTwo);
            Debug.Log(printThis);
            m_RegionOneFinished = false;
            m_RegionTwoFinished = false;
            m_RegionOneStart = false;
            m_prevConductorSample = new OVRVelocityTracker.ConductorSample();
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
}
