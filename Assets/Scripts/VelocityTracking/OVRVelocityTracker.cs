using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.Xml;
using UnityEngine;



/// <summary>
/// Velocity Tracker is in charge of collecting and evaluating ConductorSamples. 
/// GatherSample is used for data evaluation.TrackAndStoreVelocity is used for ConductorSample collection. 
/// </summary>
public class OVRVelocityTracker : OVRGestureHandle
{


    #region Variables

    private float ModelBarLength;
    private float DISTANCE_BETWEEN_MEASUREMENTS = 0.005f;
    private float startTime;
    private float previousYVelocity;
    private Vector3 previousBatonPosition;
    private Vector3 previousControllerPosition;
    private bool dataHasBeenRecorded;
    private bool planeHasBeenSpawned;
    private bool dataShouldBeRecorded;
    private int currentTrial;
    private Vector3 basePlaneCollisionPoint;
    private float prevCollisionTime = 0;
    private float timeSincePrevCollision;
    private Vector3 BP1;
    private Vector3 previousConductorSamplePoint; 


    private char[] gestureSize = { 'S', 'M', 'L' };
    private char currentGestureSize;
    public int[] BPMToRecord = { 80, 100, 120 };
    private int currentBPMToRecord;
    private float timeBetweenBeats;
    private float allowedTimingError;
   
    public Transform conductorBaton;
    public InHouseMetronome inHouseMetronome;
    public static List<ConductorSample> samples;
    public static Dictionary<string, List<ConductorSample>> trials;

    private List<GameObject> spheres = new List<GameObject>();
    
    
    private List<ConductorSample> finalSamples;

    [SerializeField] private HorizontalPlane horizontalPlane;
    [SerializeField] private TrialDisplayBehaviour trialDisplayBehaviour; 
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initialize game objects and other values
    /// </summary>
    private void Awake()
    {

        samples = new List<ConductorSample>();
        finalSamples = new List<ConductorSample>();
        dataHasBeenRecorded = false;
        planeHasBeenSpawned = false;
        dataShouldBeRecorded = true;
        currentGestureSize = gestureSize[0];
        currentBPMToRecord = BPMToRecord[0];
        //dataUpdater = new ControllerDataUpdater();
        currentTrial = 1;
        startTime = 0;
        BP1 = new Vector3(0, 0, 0);
        previousBatonPosition = Vector3.zero;
        previousControllerPosition = Vector3.zero;
        previousYVelocity = 0;
    }

    private void Update()
    {
        DataTypeSetter();
        if(Input.GetKeyUp("r"))
        {
            trialDisplayBehaviour.displayRecordScreen();
            SetNewSamples(finalSamples);
            finalSamples.Clear();
            dataHasBeenRecorded = true;
        }
        
    }
    #endregion

    #region Class Methods


    /// <summary>
    /// Sets up the type of gesture that will be recorded. 
    /// Small/Medium/Lard and 80/100/120 BPM
    /// </summary>
    private void DataTypeSetter()
    {
       
        if (Input.GetKeyUp("1"))
        {
            currentBPMToRecord = BPMToRecord[0];
            timeBetweenBeats = (60 / currentBPMToRecord);
            inHouseMetronome.SetNewBPM((double)currentBPMToRecord);
            currentTrial = 1;
            trialDisplayBehaviour.changeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.Clear();
        }
        if (Input.GetKeyUp("2"))
        {
            currentBPMToRecord = BPMToRecord[1];
            timeBetweenBeats = (60 / currentBPMToRecord);
            inHouseMetronome.SetNewBPM((double)currentBPMToRecord);
            currentTrial = 1;
            trialDisplayBehaviour.changeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.Clear();
        }

        if (Input.GetKeyUp("3"))
        {
            currentBPMToRecord = BPMToRecord[2];
            timeBetweenBeats = (60 / currentBPMToRecord);
            inHouseMetronome.SetNewBPM((double)currentBPMToRecord);
            currentTrial = 1;
            trialDisplayBehaviour.changeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.Clear();
        }

        if (Input.GetKeyUp("s"))
        {
            currentGestureSize = gestureSize[0];
            currentTrial = 1;
            trialDisplayBehaviour.changeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.Clear();
            DestroySpheres();
        }
        if (Input.GetKeyUp("m"))
        {
            currentGestureSize = gestureSize[1];
            currentTrial = 1;
            trialDisplayBehaviour.changeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.Clear();
            DestroySpheres();
        }
        if (Input.GetKeyUp("l"))
        {
            currentGestureSize = gestureSize[2];
            currentTrial = 1;
            trialDisplayBehaviour.changeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.Clear();
            DestroySpheres();
        }
    }
    #endregion

    #region Class Methods


    /// <summary>
    /// Destroy the spheres used to represent the current gesture
    /// </summary>
    private void DestroySpheres()
    {
        foreach(GameObject sphere in spheres)
        {
            Destroy(sphere);
        }
    }
    /// <summary>
    /// Collects conductor samples every 'DistanceBetweenMeasurements' apart. 
    /// </summary>
    /// <param name="gestureString"> Current gesture the sample was collected at </param>
    /// <param name="device"> Device corresponding to the baton </param>
    public void StoreConductorSample(string gestureString, OVRInput.Controller device)
    {
        if (!dataShouldBeRecorded) return;
        int SizeOfSamplesList = samples.Count;
        // We're starting a new list of Conductor Samples so we need to track the start time
        if (SizeOfSamplesList == 0) startTime = (Time.time*1000.0f)/1000.0f;
        float currRelativeTimeInPrepBeat = Mathf.Round((Time.time - startTime) * 1000.0f) / 1000.0f;
        float currOverallTime = Mathf.Round(Time.time * 1000.0f) / 1000.0f;

        float distanceCoveredSofar = GetDistanceCoveredSoFar(device);
        
        if (SizeOfSamplesList == 0 || distanceCoveredSofar != 0 )
        {
            Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(device);
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(device);
            float controllerAcceleration = OVRInput.GetLocalControllerAcceleration(device).magnitude;

            if(previousYVelocity < 0 && controllerVelocity.y > 0 && !planeHasBeenSpawned)
            {
                horizontalPlane.SpawnPlane(conductorBaton.position);
                prevCollisionTime = currOverallTime;
                horizontalPlane.SpawnPlane(conductorBaton.position);
                basePlaneCollisionPoint = controllerPosition; 
                if (previousBatonPosition.y > conductorBaton.position.y)
                {
                    horizontalPlane.SpawnPlane(conductorBaton.position);
                    BP1 = controllerPosition;
                }
                else
                {
                    horizontalPlane.SpawnPlane(previousBatonPosition);
                    BP1 = previousControllerPosition;
                }
                planeHasBeenSpawned = true;
            }

            float angleToBP1 = GetAngleToFirstCollisionWithBasePlane(BP1,controllerPosition);
            if (angleToBP1 == 90) angleToBP1 = -999;
            float totalDistanceCoveredSoFar = distanceCoveredSofar;
            if (BP1 == Vector3.zero) totalDistanceCoveredSoFar = 0;
            // Three conditions under which we will add a data sample 
            // 1. The controller is located above the BP1 position
            // 2. The BP1 has not been degined
            // 3. Controller y velocity is positive 
            // These combinations of conditions allow for data points to only be recorded for the prep gesture

            if (controllerPosition.y > BP1.y || BP1 == Vector3.zero|| controllerVelocity.y>0 )
            {
                ConductorSample newConductorSample = new ConductorSample(
                        controllerVelocity,                                         // Velocity vector
                        controllerPosition,                                         // Position
                        controllerVelocity.magnitude,                               // Velocity magnitude
                        currRelativeTimeInPrepBeat,                                 // Local time within prep
                        currOverallTime,                                            // Global time 
                        controllerAcceleration,                                     // Acceleration magnitude
                        angleToBP1,                                                 // Angle to BP1. TODO find angle
                        totalDistanceCoveredSoFar,                                  // Total distance covered so far
                        currentGestureSize,                                         // Gesture size currently measuring.
                        currentBPMToRecord,                                         // Current BPM being collected
                        currentTrial
                        );
                if(newConductorSample.trial == 1) InstantiateDebugSphere();
                samples.Add(newConductorSample);
                trialDisplayBehaviour.updateValuesWithConductorSample(newConductorSample);
                
            }
            else
            {
                Debug.Log("Controller y position = " + samples[samples.Count - 1].position.y);
                Debug.Log("BP1 y position = " + BP1.y);
                if (samples[samples.Count - 1].position.y > BP1.y)
                {
                    ConductorSample newConductorSample = new ConductorSample(
                        controllerVelocity,                                         // Velocity vector
                        controllerPosition,                                         // Position
                        controllerVelocity.magnitude,                               // Velocity magnitude
                        currRelativeTimeInPrepBeat,                                 // Local time within prep
                        currOverallTime,                                            // Global time 
                        controllerAcceleration,                                     // Acceleration magnitude
                        angleToBP1,                                                 // Angle to BP1. TODO find angle
                        totalDistanceCoveredSoFar,                                  // Total distance covered so far
                        currentGestureSize,                                         // Gesture size currently measuring.
                        currentBPMToRecord,                                         // Current BPM being collected
                        currentTrial
                        );
                    if(newConductorSample.trial == 1) InstantiateDebugSphere();
                    samples.Add(newConductorSample);
                    trialDisplayBehaviour.updateValuesWithConductorSample(newConductorSample);

                  
                }
                else
                {
                    Debug.Log("Number of data points: " + samples.Count);
                    dataShouldBeRecorded = false;
                }
            }
            previousYVelocity = controllerVelocity.y;
            previousBatonPosition = conductorBaton.position;
            previousControllerPosition = controllerPosition;
            return;
        }


    }

    /// <summary>
    /// Spawn a sphere in the position of the baton to signify that a ConductorSample has been recorded
    /// </summary>
    private void InstantiateDebugSphere()
    {
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = conductorBaton.position;
        debugSphere.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        spheres.Add(debugSphere);
    }

    /// <summary>
    /// Calculates time elapsed since the last recorded collision with the base plane
    /// Trigger on device must be pressed down for this function to be called from OVRGestureHandle.cs
    /// </summary>
    /// <param name="device"> Device corresponding to the baton </param>
    /// <returns>Time elapsed since previous collision</returns>
    public void GetTimeSincePrevCollisionWithBasePlane(OVRInput.Controller device)
    {
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(device);
        float currOverallTime = Mathf.Round(Time.time * 1000.0f) / 1000.0f;
        // if collision (from above) with already spawned base plane is occurring 
        if (previousYVelocity > BP1.y && controllerPosition.y < BP1.y && planeHasBeenSpawned)
        {
            // calculate time since last recorded collision 
            Debug.Log("Previous Collision occurred at: " + prevCollisionTime + " seconds");
            timeSincePrevCollision = currOverallTime - prevCollisionTime;
            prevCollisionTime = currOverallTime;
            Debug.Log("Current collision occurred at: " + prevCollisionTime + " seconds");
            Debug.Log("Time elapsed since previous collision: " + timeSincePrevCollision + " seconds"); 
            CheckUserTiming();
        } 
    }

    /// <summary>
    /// Checks whether user's gestures are in time with audio BPM, providing user feedback
    /// </summary> 
    private void CheckUserTiming()
    {
        // TODO: play around with this value
        allowedTimingError = timeBetweenBeats * 0.10f;
        // user is on time
        // TODO: create PerformanceIndicator object in scene
        if (timeBetweenBeats - allowedTimingError <= timeSincePrevCollision && timeSincePrevCollision <= timeBetweenBeats + allowedTimingError)
        {
            // PerformanceIndicator (orb) turns green
        }
        // user is too fast
        else if (timeSincePrevCollision < timeBetweenBeats - allowedTimingError)
        {
            // PerformanceIndicator (orb) turns red
        }
        // user is too slow
        else
        {
            // PerformanceIndicator (orb) turns blue
        }
    }

    private float GetAngleToFirstCollisionWithBasePlane(Vector3 BP1, Vector3 currentPosition)
    {
        if (BP1 == Vector3.zero) return -999;
        Vector3 BP1ToCurrentPosition = (currentPosition - BP1).normalized;
        Vector3 BP1ProjectedVectorTowardsCurrentPosition = (new Vector3(BP1ToCurrentPosition.x, 0,BP1ToCurrentPosition.z)).normalized;
        return (float)(Math.Acos(Vector3.Dot(BP1ProjectedVectorTowardsCurrentPosition, BP1ToCurrentPosition))*(180/(Math.PI)));
    }

    /// <summary>
    /// Finds total distance covered so far in the current trial. Only returns if there have been 'DistanceBetweenMeasurements' units between
    /// the current data point and the latest one in 'samples'
    /// </summary>
    /// <returns> Total distance covered in the current trial </returns>
    private float GetDistanceCoveredSoFar(OVRInput.Controller device)
    {

        //Possibly also return 0 if we're to the right of BP1.
        try
        {
            ConductorSample latestConductorSample = samples[samples.Count - 1];
            Vector3 lastPosition = samples[samples.Count - 1].position;
            Vector3 currPosition = OVRInput.GetLocalControllerPosition(device);
            float distanceBetweenDataPoints = GetDistanceBetweenVectors(lastPosition,currPosition);
            if (distanceBetweenDataPoints > DISTANCE_BETWEEN_MEASUREMENTS)
            {
                return distanceBetweenDataPoints + samples[samples.Count-1].distanceCoveredSoFar;
            }
            else return 0;
        }

        catch(ArgumentOutOfRangeException e)
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


    /// <summary>
    /// Called when user lets go of tracker button. Takes list of samples collected thus far and 
    /// stores it as one trial.
    /// </summary>
    internal void StoreCurrentTrial()
    {
        Debug.Assert(samples.Count != 0);
        if (samples.Count > 10)
        {
            currentTrial++;
            trialDisplayBehaviour.changeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.AddRange(samples);
           
        }
        samples.Clear();
        planeHasBeenSpawned = false;
        dataShouldBeRecorded = true;
        previousYVelocity = 0;
        previousBatonPosition = Vector3.zero;
        previousControllerPosition = Vector3.zero;
        BP1 = Vector3.zero;
    }



    /// <summary>
    /// Returns true if a is within an offset of b
    /// </summary>
    /// <param name="a"> value 1 </param>
    /// <param name="b"> value 2 </param>
    /// <param name="offset"> Room for error within the equality of value 1 and value 2.  </param>
    /// <returns> True if a and b are within an offset of each other </returns>
    public Boolean ApproximatelyEquals(float a, float b, float offset)
    {
        float aMin = a - offset;
        float aMax = a + offset;
        if (b >= aMin && b <= aMax)
            return true;
        return false;
    }

    /// <summary>
    /// Called from OVRVelocityTracker once enough samples are collected
    /// Creats a new XML file, populated by ConductorSamples. 
    /// </summary>
    /// <param name="samples"> List of newly recorded ConductorSamples </param>
    public void SetNewSamples(List<ConductorSample> samples)
    {

        //using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/XML_Data/ConductorSamples_OcculusTrial_Test_ " + currentBPMToRecord + "_" + currentGestureSize + ".xml"))
        using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/ConductorSamples_OcculusTrial_Test_ " + currentBPMToRecord + "_" + currentGestureSize + ".xml"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Samples");

            foreach (ConductorSample cs in samples)
            {
                writer.WriteStartElement("ConductorSample");

                writer.WriteElementString("ID", cs.id);
                writer.WriteElementString("GestureSize", cs.gestureSize.ToString());

                writer.WriteElementString("Velocity_X", cs.velocityVector.x.ToString());
                writer.WriteElementString("Velocity_Y", cs.velocityVector.y.ToString());
                writer.WriteElementString("Velocity_Z", cs.velocityVector.z.ToString());

                writer.WriteElementString("Position_X", cs.position.x.ToString());
                writer.WriteElementString("Position_Y", cs.position.y.ToString());
                writer.WriteElementString("Position_Z", cs.position.z.ToString());

                writer.WriteElementString("VelocityMagnitude", cs.velocityMagnitude.ToString());
                writer.WriteElementString("TimeRelativeToPrep", cs.timeRelativeToPrep.ToString());

                writer.WriteElementString("AngleToBP1", cs.angleToBP1.ToString());

                writer.WriteElementString("DistanceCoveredSoFar", cs.distanceCoveredSoFar.ToString());
                writer.WriteElementString("Acceleration", cs.acceleration.ToString());
                writer.WriteElementString("BPM", cs.BPM.ToString());
                writer.WriteElementString("Trial", cs.trial.ToString());

                writer.WriteEndElement();

            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }

    #endregion

    #region Struct
    /// <summary>
    /// Struct that contains relevant information of the controller.
    /// Collected at each frame
    /// </summary>
    [Serializable]
    public struct ConductorSample
    {
        public string id;
        public char gestureSize;
        public Vector3 velocityVector;
        public Vector3 position;
        public float velocityMagnitude;
        public float timeRelativeToPrep;
        public float angleToBP1;
        public float distanceCoveredSoFar;
        public float acceleration;
        public int BPM;
        public int trial;

        public ConductorSample(Vector3 velocityVector, Vector3 position,
            float velocityMagnitude, float timeRelativeToPrep ,float globalTime,
            float acceleration,float angleToBP1,float distanceCoveredSoFar, char size, int BPM, int trial)
        {
            this.id = "" + globalTime + size + BPM + "T" + trial;
            this.gestureSize = size;
            this.velocityVector = velocityVector;
            this.position = position;
            this.velocityMagnitude = velocityMagnitude;
            this.timeRelativeToPrep = timeRelativeToPrep;
            this.angleToBP1 = angleToBP1;
            this.BPM = BPM;
            this.acceleration = acceleration;
            this.distanceCoveredSoFar = distanceCoveredSoFar;
            this.trial = trial;

        }

    }
    #endregion


}
