using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// Velocity Tracker is in charge of collecting and evaluating ConductorSamples. 
/// GatherSample is used for data evaluation.TrackAndStoreVelocity is used for ConductorSample collection. 
/// </summary>
public class OVRVelocityTracker : MonoBehaviour
{
    #region Variables
    private int currentTrial;
    public int currentBPMToRecord;
    public int[] BPMToRecord = { 80, 100, 120 };
    public int[] NewBPMToRecord = { 60, 100, 140 };

    private float DISTANCE_BETWEEN_MEASUREMENTS = 0.005f;
    private float startTime;
    private float previousYVelocity;
    private float prevCollisionTime;
    private float timeSincePrevCollision;

    private bool dataHasBeenRecorded;
    private bool dataShouldBeRecorded;
    private bool isBeneathPlane = false;
    private bool dataTypeHasBeenChanged = false;
    public bool RestrictRecordingData { get; private set; }
    public bool planeHasBeenSpawned;

    private Vector3 previousBatonPosition;
    private Vector3 previousControllerPosition;
    private Vector3 basePlaneCollisionPoint;
    private Vector3 BP1;

    public delegate void VelocityTracker();
    public static event VelocityTracker MusicStart;

    private char[] gestureSize = { 'S', 'M', 'L' };
    private char currentGestureSize;

    public static Dictionary<string, List<ConductorSample>> trials;

    private List<GameObject> spheres = new List<GameObject>();
    private List<ConductorSample> finalSamples;
    private List<ConductorSample> samples;

    [SerializeField] private BPMPredictor BPMPred;
    [SerializeField] private TempoController tempoController;
    [SerializeField] private HorizontalPlane horizontalPlane;
    [SerializeField] private TrialDisplayBehaviour trialDisplayBehaviour;
    [SerializeField] private PerformanceIndicator performanceIndicator;
    [SerializeField] private Transform conductorBaton;
    [SerializeField] private InHouseMetronome inHouseMetronome;
    [SerializeField] private GameObject batonObject;
    [SerializeField] private float yVelocityThreshold;

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
        currentBPMToRecord = BPMToRecord[1];        
        currentTrial = 1;
        startTime = 0;
        prevCollisionTime = 0;
        BP1 = new Vector3(0, 0, 0);
        previousBatonPosition = Vector3.zero;
        previousControllerPosition = Vector3.zero;
        previousYVelocity = 0;
        if (batonObject == null)
        {
            batonObject = GameObject.Find("Baton_Tip");
        }
    }

    private void Update()
    {
        DataTypeSetter();
        if(Input.GetKeyUp("r"))
        {
            trialDisplayBehaviour.DisplayRecordScreen();
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
            Debug.Log(BPMToRecord[0]);
            dataTypeHasBeenChanged = true;
        }
        if (Input.GetKeyUp("2"))
        {
            currentBPMToRecord = BPMToRecord[1];
            dataTypeHasBeenChanged = true;
        }

        if (Input.GetKeyUp("3"))
        {
            currentBPMToRecord = BPMToRecord[2];
            dataTypeHasBeenChanged = true;
        }

        if (Input.GetKeyUp("s"))
        {
            currentGestureSize = gestureSize[0];
            dataTypeHasBeenChanged = true;
        }
        if (Input.GetKeyUp("m"))
        {
            currentGestureSize = gestureSize[1];
            dataTypeHasBeenChanged = true;
        }
        if (Input.GetKeyUp("l"))
        {
            currentGestureSize = gestureSize[2];
            dataTypeHasBeenChanged = true;

        }
        if (dataTypeHasBeenChanged)
        {
            currentTrial = 1;
            inHouseMetronome.SetNewBPM((double)currentBPMToRecord);
            trialDisplayBehaviour.ChangeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.Clear();
            DestroySpheres();
            dataTypeHasBeenChanged = false;
        }
    }

    /// <summary>
    /// Sets the local baton object to a different baton instance in scene.
    /// </summary>
    public void SetBatonObject(GameObject newBaton) 
    {
        this.batonObject = newBaton;
    }

    /// <summary>
    /// Returns the current local baton object. 
    /// </summary>
    public GameObject GetBatonObject()
    {
        return batonObject;
    }

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
    /// Spawn the horizontal plane based on the first negative to positive convex gesture by the prep beat. 
    /// </summary>
    public void SpawnPlaneIfNotSpawned()
    {
        float currOverallTime = Mathf.Round(Time.time * 1000.0f) / 1000.0f;
        Vector3 controllerPosition = batonObject.transform.position;
        Vector3 controllerVelocity = (controllerPosition - previousBatonPosition) / Time.deltaTime;
        float thresholdCheck = Math.Abs(previousYVelocity - controllerVelocity.y);

        // =========================
        // -- Checks for the precise instance where the current controller y velocity is positive
        // -- and the previous controller y velocity is negative.
        // -- This is the first slope of the prep beat, so we spawn the plane here.
        // =========================
        if (previousYVelocity < 0 && controllerVelocity.y > 0 && !planeHasBeenSpawned && thresholdCheck > yVelocityThreshold)
        {
            prevCollisionTime = currOverallTime;
            basePlaneCollisionPoint = controllerPosition;

            // ========================
            // -- This is to account for controller weirdness. Sometimes although
            // -- the velocity of the current controller is positive, it doesn't mean that it's at a higher position
            // -- then the previous velocity. So we pick the smallest one. 
            // ========================
            if (previousBatonPosition.y > batonObject.transform.position.y)
            {
                horizontalPlane.SpawnPlane(batonObject.transform.position);
                BP1 = controllerPosition;
            }
            else
            {
                horizontalPlane.SpawnPlane(previousBatonPosition);
                BP1 = previousControllerPosition;
            }
            planeHasBeenSpawned = true;
        }
        previousYVelocity = controllerVelocity.y;
        previousBatonPosition = batonObject.transform.position;
        previousControllerPosition = controllerPosition;
    }

    /// <summary>
    /// Collects conductor samples every 'DistanceBetweenMeasurements' apart. 
    /// </summary>
    /// <param name="device"> Device corresponding to the baton </param>
    public void CollectConductorSamples(OVRInput.Controller device)
    {
        int SizeOfSamplesList = samples.Count;
        // We're starting a new list of Conductor Samples so we need to track the start time
        if (SizeOfSamplesList == 0) startTime = (Time.time*1000.0f)/1000.0f;

        float currRelativeTimeInPrepBeat = Mathf.Round((Time.time - startTime) * 1000.0f) / 1000.0f;
        float currOverallTime = Mathf.Round(Time.time * 1000.0f) / 1000.0f;
        float distanceCoveredSofar = GetDistanceCoveredSoFar(device);
        
        if (SizeOfSamplesList == 0 || distanceCoveredSofar != 0 )
        {
            Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(device);
            Vector3 controllerPosition = batonObject.transform.position;
            float thresholdCheck = Math.Abs(previousYVelocity - controllerVelocity.y);

            float controllerAcceleration = OVRInput.GetLocalControllerAcceleration(device).magnitude;
            float angleToBP1 = GetAngleToFirstCollisionWithBasePlane(BP1,controllerPosition);
            float totalDistanceCoveredSoFar = distanceCoveredSofar;
            if (BP1 == Vector3.zero) totalDistanceCoveredSoFar = 0;

            // =========================
            // -- Three conditions under which we will add a data sample 
            // -- 1. The controller is located above the BP1 position
            // -- 2. The BP1 has  been defined
            // -- 3. Controller y velocity is positive 
            // -- These combinations of conditions allow for data points to only be recorded for the prep gesture
            // =========================
            if (!RestrictRecordingData)
            {
                if (controllerPosition.y > BP1.y || BP1 == Vector3.zero || controllerVelocity.y < 0)
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
                    if (planeHasBeenSpawned) BPMPred.RecordConductorSample(newConductorSample, tempoController);

                    // =========================
                    // -- Uncomment to spawn debug Spheres on first prep beat
                    // if (newConductorSample.trial == 1) InstantiateDebugSphere();
                    // =========================

                    samples.Add(newConductorSample);
                    trialDisplayBehaviour.UpdateValuesWithConductorSample(newConductorSample);
                }
                else
                {
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
                        //if (planeHasBeenSpawned) BPMPred.RecordConductorSample(newConductorSample, tempoController);

                        // =========================
                        // -- Uncomment to spawn debug Spheres on first prep beat
                        // if(newConductorSample.trial == 1) InstantiateDebugSphere();
                        // =========================

                        samples.Add(newConductorSample);
                        trialDisplayBehaviour.UpdateValuesWithConductorSample(newConductorSample);
                    }
                    else
                    {
                        dataShouldBeRecorded = false;
                    }
                }
            }
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
    /// Trigger on device must be pressed down for this function to be called (at every frame) from OVRGestureHandle.cs
    /// </summary>
    public void SetTimeSincePrevCollisionWithBasePlane()
    {
        Vector3 controllerPosition = batonObject.transform.position;
        float currOverallTime = Mathf.Round(Time.time * 1000.0f) / 1000.0f; 
        // if the controller has gone below the plane
        if (!isBeneathPlane && controllerPosition.y <= BP1.y && BP1 != Vector3.zero) 
        {
            // provide haptic feedback
            horizontalPlane.PlaneFeedback(batonObject.transform.position,false);
            // calculate time since last recorded collision  
            timeSincePrevCollision = currOverallTime - prevCollisionTime;
            prevCollisionTime = currOverallTime;
            performanceIndicator.SetUserBPM(timeSincePrevCollision);
            // -- start playing audio if not already playing and plane has been spawned during prep beat gesture
            tempoController.playPiece();
            isBeneathPlane = !isBeneathPlane;
            //MusicStart();
        } 
        // if the controller has gone over the plane
        else if (isBeneathPlane && controllerPosition.y > BP1.y)
        {
            isBeneathPlane = !isBeneathPlane;
        }
    }

    /// <summary>
    /// Return angle of controller relative to the point at which it collided with the plane for the first time
    /// </summary>
    /// <param name="BP1"> Position of the plane </param>
    /// <param name="currentPosition"> Current Position of baton </param>
    /// <returns> -999 if plane hasn't been hit , or angle</returns>
    private float GetAngleToFirstCollisionWithBasePlane(Vector3 BP1, Vector3 currentPosition)
    {
        if (BP1 == Vector3.zero) return -999;
        Vector3 BP1ToCurrentPosition = (currentPosition - BP1).normalized;
        Vector3 BP1ProjectedVectorTowardsCurrentPosition = (new Vector3(BP1ToCurrentPosition.x, 0,BP1ToCurrentPosition.z)).normalized;
        float angle = (float)(Math.Acos(Vector3.Dot(BP1ProjectedVectorTowardsCurrentPosition, BP1ToCurrentPosition)) * (180 / (Math.PI)));
        if (angle == 90) return -999;
        return angle;
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
            ConductorSample latestConductorSample = samples[samples.Count - 1];
            Vector3 lastPosition = samples[samples.Count - 1].position;
            Vector3 currPosition = batonObject.transform.position;
            float distanceBetweenDataPoints = GetDistanceBetweenVectors(lastPosition,currPosition);
            if (distanceBetweenDataPoints > DISTANCE_BETWEEN_MEASUREMENTS)
            {
                return distanceBetweenDataPoints + samples[samples.Count-1].distanceCoveredSoFar;
            }
            else return 0;
        }
        catch (ArgumentOutOfRangeException e)
        {
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
        //Debug.Assert(samples.Count != 0);
        // =========================
        // -- This is used to rule out dummy trials that were made by mistake
        // -- which only grabbed a few samples
        // =========================
        if (samples.Count > 10)
        {
            currentTrial++;
            trialDisplayBehaviour.ChangeTrial(currentTrial, currentBPMToRecord.ToString(), currentGestureSize.ToString());
            finalSamples.AddRange(samples);
        }
        BPMPred.ResetBPMPredictor();
        samples.Clear();
        planeHasBeenSpawned = false;
        dataShouldBeRecorded = true;
        previousYVelocity = 0;
        previousBatonPosition = Vector3.zero;
        previousControllerPosition = Vector3.zero;
        BP1 = Vector3.zero;
        RestrictRecordingData = false;
    }

    /// <summary>
    /// Remove the base plane
    /// </summary>
    public void RemovePlane()
    {
        horizontalPlane.GetComponent<Renderer>().enabled = false;
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
