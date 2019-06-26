using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// Class in charge of evaluating conductor samples taken from VelocityTracker
/// </summary>
public class VelocitySampleProcessor : VelocityTracker
{
    #region Variables
    [SerializeField]
    ConductorSample[] referenceSamples = { };
    [SerializeField]
    //The new BPM will be affected by the difference between the new velocity and the store velocity. This weight is used to affect how strongly this velocity difference will affect the tempo
    public float weight = 12.43f;
    [SerializeField]
    // This puts more weight on the magnitude of the velocity. A difference among velocities with larger magnitude will affect the final BPM more. 
    public float scalingVelocityWeight = 1;
    [SerializeField]
    public float epsillon = 0.05f;

    public float DirectionOffset = 0.3f;
    public float PositionOffset = 0.7f;
    public float VelocityAndDirectionThreshhold = 0.10000000000f;
    public float DirectionWeight = 1.0f;
    public float PositionWeight = 1.0f;
    public float maxAngle = 90.0f;
    public float modelBPM = 100;
    public static float prevTargetBPM = 100;
    public static VelocitySampleProcessor instance;

    private int OffsetTogetBeat = 25;
    private bool GotBeat1 = false;
    private bool GotBeat2 = false;
    private bool GotBeat3 = false;
    private bool GotBeat4 = false;
    private float perfectBeatLength = 0.6f;
    private float timeAtBeat1 = 0;
    private float timeAtBeat2 = 0;
    private float timeAtBeat3 = 0;
    private float timeAtBeat4 = 0;

    private List<ConductorSample> ConductorSampleFromXML;
    private List<float> AdjacentRealtimeDifferences;
    private float pieceVelocity = 75.0f;
    private float LengthOfModelBPMBar = 2.4f;
    private float BeatLength = 0;

    private Vector3 Beat1_velocity;
    private Vector3 Beat1_position;
    private Vector3 Beat2_velocity;
    private Vector3 Beat2_position;
    private Vector3 Beat3_velocity;
    private Vector3 Beat3_position;
    private Vector3 Beat4_velocity;
    private Vector3 Beat4_position;
    #endregion

    #region Unity Methods


    /// <summary>
    /// Initialize beat spot values. 
    /// Implements a singleton functionality
    /// Read file of conductor samples, if available 
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Beat1_velocity = new Vector3(0, 0, 0);
        Beat1_position = new Vector3(0, 0, 0);
        Beat2_velocity = new Vector3(0, 0, 0);
        Beat2_position = new Vector3(0, 0, 0);
        Beat3_velocity = new Vector3(0, 0, 0);
        Beat3_position = new Vector3(0, 0, 0);
        Beat4_velocity = new Vector3(0, 0, 0);
        Beat4_position = new Vector3(0, 0, 0);

    DontDestroyOnLoad(this.gameObject);
    ConductorSampleFromXML = new List<ConductorSample>();
    AdjacentRealtimeDifferences = new List<float>();
    ReadFile();
    SetBeatSpots();
     
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// Assigns the position at which the beats land given the read file values
    /// </summary>
    private void SetBeatSpots()
    {
        int countBeat1 = 1;
        int countBeat2 = 0;
        int countBeat3 = 0;
        int countBeat4 = 0;

        for (int i = 1; i < referenceSamples.Length-1; i++)
        {
            if(referenceSamples[i].beat == 1)
            {
                countBeat1++;
            }
            if (referenceSamples[i].beat == 2)
            {
                countBeat2++;
            }
            if (referenceSamples[i].beat == 3)
            {
                countBeat3++;
            }
            if (referenceSamples[i].beat == 4)
            {
                countBeat4++;
            }
            if(referenceSamples[i].beat > referenceSamples[i - 1].beat)
            {
                if (referenceSamples[i].beat == 2)
                {
                    Beat1_position = referenceSamples[i - countBeat1 / 2].position;
                    Beat1_velocity = referenceSamples[i - countBeat1 / 2].direction;
     
                }
                if (referenceSamples[i].beat == 3)
                {
                    Beat2_position = referenceSamples[i - countBeat2 / 2].position;
                    Beat2_velocity = referenceSamples[i - countBeat2 / 2].direction;
            
                }
                if (referenceSamples[i].beat == 4)
                {
                    Beat3_position = referenceSamples[i - countBeat3 / 2].position;
                    Beat3_velocity = referenceSamples[i - countBeat3 / 2].direction;
                  
                }
            }
            if(i == referenceSamples.Length - 2)
            {
                Beat4_position = referenceSamples[i - countBeat4 / 2].position;
              
                Beat4_velocity = referenceSamples[i - countBeat4 / 2].direction;
            }
            
        }
    }


    /// <summary>
    /// Determines whether current Conductor Sample lands at any beat point
    /// Checks angle of Velocity vector at established beat points, and 
    /// The vector formed by subtracting current position, with beat point position. 
    /// </summary>
    /// <param name="sample"></param>
    /// <param name="device"></param>
    public void EvaluateSample(ConductorSample sample, SteamVR_Controller.Device device)
    {
        // In the current implementation we interpoalte to get our ideal velocity because we're accounting for frame skipping.
        // (If we read a sample at say t = 1, but we have no reading at t = 1. We need to get our readings at t = .98 and t = 1.01
        // And calculate what our speed would be in between.)
        float barLengthAccordingToPrevTargetBPM = (60 * 4) / prevTargetBPM;
        float timeRelativeToBar = sample.time / barLengthAccordingToPrevTargetBPM;
        float targetVelocity = GetTargetVelocity(timeRelativeToBar);
        float diff = sample.velocity - targetVelocity;
        string details = string.Format("Target velocity for time [{0}] is {1}, {2} provided.", timeRelativeToBar, targetVelocity, sample.velocity);
       
        switch (sample.beat)
        {
            case 1:
                float precisionVal_1 = GetPrecisionValueFromVectors(sample.direction, Beat1_velocity, sample.position, Beat1_position);
                Vector3 vectorToCurrentPosition_1 = Vector3.Normalize(sample.position - Beat1_position);
                float angle_1 = Vector3.Angle(vectorToCurrentPosition_1, Beat1_velocity);
       
                if(angle_1 < maxAngle && !GotBeat1)
                {
                  
                    device.TriggerHapticPulse();
                    GotBeat1 = true;
                    GotBeat2 = false;
                    GotBeat3 = false; 
                    GotBeat4 = false;

                    timeAtBeat1 = sample.time;
                    timeAtBeat2 = 0;
                    timeAtBeat3 = 0;

                    if (timeAtBeat4 != 0)
                    {
                        BeatLength = timeAtBeat1 - timeAtBeat4;
                        // If the difference between beat length and perfect beat length is substantial enough, then we modify the prevTargetBPM.
                        if(Math.Abs(BeatLength - perfectBeatLength) > epsillon)
                        {
                            prevTargetBPM = 60 / BeatLength;
                        }
                    }

                }
                Debug.Log("At beat " + sample.beat + " the angle was " + angle_1);
                break;
            case 2:
                float precisionVal_2 = GetPrecisionValueFromVectors(sample.direction, Beat2_velocity, sample.position, Beat2_position);
                Vector3 vectorToCurrentPosition_2 = Vector3.Normalize(sample.position - Beat2_position);
                float angle_2 = Vector3.Angle(vectorToCurrentPosition_2, Beat2_velocity);

                if (angle_2 < maxAngle && !GotBeat2)
                {
                    Debug.Log("We found Beat 2 holy shit");
                    device.TriggerHapticPulse();
                    GotBeat1 = false;
                    GotBeat2 = true;
                    GotBeat3 = false;
                    GotBeat4 = false;

                    timeAtBeat2 = sample.time;
                    timeAtBeat3 = 0;
                    timeAtBeat4 = 0;

                    if (timeAtBeat1 != 0)
                    {
                        BeatLength = timeAtBeat2 - timeAtBeat1;
                        if (Math.Abs(BeatLength - perfectBeatLength) > epsillon)
                        {
                            prevTargetBPM = 60 / BeatLength;
                        }
                    }


                }
                Debug.Log("At beat " + sample.beat + " the angle was " + angle_2);
                break;
            case 3:
                float precisionVal_3 = GetPrecisionValueFromVectors(sample.direction, Beat3_velocity, sample.position, Beat3_position);
                Vector3 vectorToCurrentPosition_3 = Vector3.Normalize(sample.position - Beat3_position);
                float angle_3 = Vector3.Angle(vectorToCurrentPosition_3, Beat3_velocity);

                if (angle_3 < maxAngle && !GotBeat3)
                {
                    Debug.Log("We found Beat 3 holy shit");
                    device.TriggerHapticPulse();
                    GotBeat1 = false;
                    GotBeat2 = false;
                    GotBeat3 = true;
                    GotBeat4 = false;

                    timeAtBeat3 = sample.time;

                    timeAtBeat4 = 0;
                    timeAtBeat1 = 0;

                    if (timeAtBeat2 != 0)
                    {
                        BeatLength = timeAtBeat3 - timeAtBeat2;
                        if (Math.Abs(BeatLength - perfectBeatLength) > epsillon)
                        {
                            prevTargetBPM = 60 / BeatLength;
                        }
                    }
                }
                Debug.Log("At beat " + sample.beat + " the angle was " + angle_3);
                break;
            case 4:
                float precisionVal_4 = GetPrecisionValueFromVectors(sample.direction, Beat4_velocity, sample.position, Beat4_position);
                Vector3 vectorToCurrentPosition_4 = Vector3.Normalize(sample.position - Beat4_position);
                float angle_4 = Vector3.Angle(vectorToCurrentPosition_4, Beat4_velocity);

                if (angle_4 < maxAngle && !GotBeat4)
                {
                    Debug.Log("We found Beat 4 holy shit");
                    device.TriggerHapticPulse();
                    GotBeat1 = false;
                    GotBeat2 = false;
                    GotBeat3 = false;
                    GotBeat4 = true;

                    
                    timeAtBeat4 = sample.time;

                    timeAtBeat1 = 0;
                    timeAtBeat2 = 0;

                    if (timeAtBeat3 != 0)
                    {
                        BeatLength = timeAtBeat4 - timeAtBeat3;
                        if (Math.Abs(BeatLength - perfectBeatLength) > epsillon)
                        {
                            prevTargetBPM = 60 / BeatLength;
                        }
                    }

                }
                Debug.Log("At beat " + sample.beat + " the angle was " + angle_4);
                break;
            default:
                break;
        }
    }
    

    /// <summary>
    /// Looks for first conductor sample in referenceSamples to exceed the current sampleTime
    /// Provided by Rhys Patterson
    /// </summary>
    /// <param name="sampleTime"> Time value of current sample </param>
    /// <returns> Interpolated velocity value given sample time </returns>
    float GetTargetVelocity(float sampleTime)
    {
        // Iterate through our model list
        for (int i = 0; i < referenceSamples.Length; i++)
        {
            float targetTime = referenceSamples[i].time;
            // Move onto the next sample if this sample is earlier than the provided sample
            // Note: For optimisation in final solution, the index of the previous sample is useful
            // to store, to iterate from this point rather than from the start every time
            bool isEarlySample = targetTime < sampleTime;
            if (isEarlySample)
            {
                continue;
            }

            // Use this sample as the base velocity for the target
            float targetVelocity = referenceSamples[i].velocity;

            if (i > 0)
            {
                // Calculate a more precise velocity by interpolating towards the previous sample
                // by the difference in timestamps between these samples


                float sampleInterp = Mathf.InverseLerp(referenceSamples[i].time, referenceSamples[i - 1].time, sampleTime);
                targetVelocity = Mathf.Lerp(targetVelocity, referenceSamples[i - 1].velocity, sampleInterp);

            }
            return targetVelocity;
        }

        // Use the last sample in the set if the provided sample exceeds last reference sample time
        return referenceSamples[referenceSamples.Length - 1].velocity;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Calculates some value formed by the differences in sample, and beat, position and velocity values. 
    /// </summary>
    /// <param name="sampleDirection"> Velocity vector of current sample. </param>
    /// <param name="beat_velocity"> Velocity vector of beat sample </param>
    /// <param name="samplePosition"> Position vector of current sample </param>
    /// <param name="beat_position"> Position vector of beat sample </param>
    /// <returns> Positive precision value, scales with difference in position and velocity and modifiable weights. </returns>
    private float GetPrecisionValueFromVectors(Vector3 sampleDirection, Vector3 beat_velocity, Vector3 samplePosition, Vector3 beat_position)
    {
        float precisionVal = 0;
        precisionVal = precisionVal + ((sampleDirection.x - beat_velocity.x) + (sampleDirection.y - beat_velocity.y)) / DirectionWeight;
        precisionVal = precisionVal + ((samplePosition.x - beat_position.x) + (samplePosition.y - beat_position.y)) / PositionWeight;
        return Math.Abs(precisionVal);
    }

    /// <summary>
    /// Determines if two Vectors are about the same. 
    /// </summary>
    /// <param name="sampleV"> Sample vector recorded </param>
    /// <param name="perfectV"> Perfect vector to compare sample to </param>
    /// <param name="Offset"> Room for error </param>
    /// <returns> True if two vectors are 'Offset' away from each other in the x and y component </returns>
    private bool VectorApproximatelyEquals(Vector3 sampleV, Vector3 perfectV, float Offset)
    {
        return ApproximatelyEquals(sampleV.x, perfectV.x, Offset) &&
                ApproximatelyEquals(sampleV.y, perfectV.y, Offset);
    }

    /// <summary>
    /// Returns a precision value determined by the difference between two velocity magnitudes and the velocity magnitude that is desired. 
    /// </summary>
    /// <param name="diff"> Difference between sample magnitude and ideal magnitude </param>
    /// <param name="targetVelocity"> Magnitude of ideal velocity </param>
    /// <returns> Precision float that scales with diff and target velocity </returns>
    private float GetPrecisionVal(float diff, float targetVelocity)
    {
        return modelBPM * (diff / weight) + Mathf.Sign(diff) * targetVelocity * scalingVelocityWeight;
    }

    /// <summary>
    /// Getter method returns size of referenceSamples
    /// </summary>
    /// <returns> Size of reference samples </returns>
    public int getReferenceSize()
    {
        return referenceSamples.Length;
    }

    /// <summary>
    /// Returns true if a is within an offset of b
    /// </summary>
    /// <param name="a"> value 1 </param>
    /// <param name="b"> value 2 </param>
    /// <param name="offset"> Room for error within the equality of value 1 and value 2.  </param>
    /// <returns></returns>
    public Boolean ApproximatelyEquals(float a, float b, float offset)
    {
        float aMin = a - offset;
        float aMax = a + offset;
        if (b >= aMin && b <= aMax)
            return true;
        return false;
    }
    #endregion

    #region XML methods
    /// <summary>
    /// Called from VelocityTracker once enough samples are collected and averaged. 
    /// Creats a new XML file, populated by ConductorSamples. 
    /// </summary>
    /// <param name="samples"> List of newly recorded ConductorSamples </param>
    public void SetNewSamples(List<ConductorSample> samples)
    {

        using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/XML_Data/ConductorSamples_OcculusTrial.xml"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Samples");

            foreach(ConductorSample cs in samples)
            {
                writer.WriteStartElement("ConductorSample");

                writer.WriteElementString("Position_X", cs.position.x.ToString());
                writer.WriteElementString("Position_Y", cs.position.y.ToString());
                writer.WriteElementString("Position_Z", cs.position.z.ToString());

                writer.WriteElementString("Velocity_X", cs.direction.x.ToString());
                writer.WriteElementString("Velocity_Y", cs.direction.y.ToString());
                writer.WriteElementString("Velocity_Z", cs.direction.z.ToString());

                writer.WriteElementString("Magnitude", cs.velocity.ToString());
                writer.WriteElementString("Time", cs.time.ToString());

                writer.WriteElementString("Beat", cs.beat.ToString());

            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }

    /// <summary>
    /// Called on Awake functions. 
    /// Loads the averaged collected sample data to be used in real time. 
    /// </summary>
    public void ReadFile()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(Application.dataPath + "/XML_Data/ConductorSamples.xml");
        XmlNodeList conductorSamples = xmlDoc.GetElementsByTagName("ConductorSample");

        foreach (XmlNode cs in conductorSamples)
        {
            XmlNodeList conductorSampleComponents = cs.ChildNodes;
            Vector3 position = new Vector3(0, 0, 0);
            Vector3 velocity = new Vector3(0, 0, 0);
            float magnitude = 0;
            float time = 0;
            int beat = 0;
            foreach (XmlNode component in conductorSampleComponents)
            {
                if (component.Name == "Position_X")
                {
                    position.x = float.Parse(component.InnerText);
                }
                if (component.Name == "Position_Y")
                {
                    position.y = float.Parse(component.InnerText);
                }
                if (component.Name == "Position_Z")
                {
                    position.z = float.Parse(component.InnerText);
                }
                if (component.Name == "Velocity_X")
                {
                    velocity.x = float.Parse(component.InnerText);
                }
                if (component.Name == "Velocity_Y")
                {
                    velocity.y = float.Parse(component.InnerText);
                }
                if (component.Name == "Velocity_Z")
                {
                    velocity.z = float.Parse(component.InnerText);
                }
                if (component.Name == "Magnitude")
                {
                    magnitude = float.Parse(component.InnerText);
                }
                if (component.Name == "Time")
                {
                    time = float.Parse(component.InnerText);
                }
                if (component.Name == "Beat")
                {
                    beat = int.Parse(component.InnerText);
                }
            }
            ConductorSampleFromXML.Add(new ConductorSample(velocity, position, magnitude, time, beat));
        }
        referenceSamples = ConductorSampleFromXML.ToArray();
    }
    #endregion
    
}




//==========================================================

/*[SerializeField] ConductorSample exampleSample1 = new ConductorSample { direction = new Vector3(1,1,1), position = new Vector3(1,2,3), velocity = 1.2f,time = 0.01000001f , beat = 1};
[ContextMenu("Evaluate Example Sample 1")] void EvaluateExampleSample1() { EvaluateSample(exampleSample1); }

[SerializeField] ConductorSample exampleSample2 = new ConductorSample { direction = new Vector3(1, 1, 1), position = new Vector3(1, 2, 3), velocity = 1.6f, time = 0.05f, beat = 1 };
[ContextMenu("Evaluate Example Sample 2")] void EvaluateExampleSample2() { EvaluateSample(exampleSample2); }

[SerializeField] ConductorSample exampleSample3 = new ConductorSample { direction = new Vector3(1, 1, 1), position = new Vector3(1, 2, 3), velocity = 1.7f, time = 0.05f, beat = 1 };
[ContextMenu("Evaluate Example Sample 3")] void EvaluateExampleSample3() { EvaluateSample(exampleSample3); }

[SerializeField] ConductorSample exampleSample4 = new ConductorSample { direction = new Vector3(1, 1, 1), position = new Vector3(1, 2, 3), velocity = 1.5f, time = 0.05f, beat = 1 };
[ContextMenu("Evaluate Example Sample 4")] void EvaluateExampleSample4() { EvaluateSample(exampleSample4); }

[SerializeField] ConductorSample exampleSample5 = new ConductorSample { direction = new Vector3(1, 1, 1), position = new Vector3(1, 2, 3), velocity = 0.4f, time = 0.14f, beat = 1 };
[ContextMenu("Evaluate Example Sample 5")] void EvaluateExampleSample5() { EvaluateSample(exampleSample5); }

[SerializeField] ConductorSample exampleSample6 = new ConductorSample { direction = new Vector3(1, 1, 1), position = new Vector3(1, 2, 3), velocity = 0.5f, time = 0.14f, beat = 1 };
[ContextMenu("Evaluate Example Sample 6")] void EvaluateExampleSample6() { EvaluateSample(exampleSample6); }

[SerializeField] ConductorSample exampleSample7 = new ConductorSample { direction = new Vector3(1, 1, 1), position = new Vector3(1, 2, 3), velocity = 0.3f, time = 0.14f, beat = 1 };
[ContextMenu("Evaluate Example Sample 7")] void EvaluateExampleSample7() { EvaluateSample(exampleSample7); }

[SerializeField] ConductorSample exampleSample8 = new ConductorSample { direction = new Vector3(1, 1, 1), position = new Vector3(1, 2, 3), velocity = 1.7f, time = 0.14f, beat = 1 };
[ContextMenu("Evaluate Example Sample 8")] void EvaluateExampleSample8() { EvaluateSample(exampleSample8); }*/

/*
AdjacentRealtimeDifferences.Add(diff);
Debug.Log(AdjacentRealtimeDifferences.Count);


if(AdjacentRealtimeDifferences.Count == MaxComparisonVelocities)
{
    float avgDiff = AdjacentRealtimeDifferences.Average();
    string result = Math.Sign(avgDiff) < 0 ? "Dragging" : "Leading";
    if (avgDiff < -0.5)
    {
        prevTargetBPM = 90;
        tp.ChangePieceVelocity(pieceVelocity * (prevTargetBPM / 100), prevTargetBPM);
        Debug.LogFormat("{0}. <b><color=red>{1} by {2}%</color></b>", details, result, Math.Abs(prevTargetBPM / modelBPM));
        Debug.Log("PrevTargetBPM = " + prevTargetBPM);
    }
    else if (avgDiff > 0.5)
    {
        prevTargetBPM = 110;
        tp.ChangePieceVelocity(pieceVelocity * (prevTargetBPM / 100), prevTargetBPM);
        Debug.LogFormat("{0}. <b><color=red>{1} by {2}%</color></b>", details, result, Math.Abs(prevTargetBPM / modelBPM));
        Debug.Log("PrevTargetBPM = " + prevTargetBPM);
    }
    else
    {
        prevTargetBPM = modelBPM;
        Debug.LogFormat("{0}. <b><color=green>Perfect velocity</color></b>", details);
        Debug.Log("PrevTargetBPM = " + prevTargetBPM);

        tp.ChangePieceVelocity(pieceVelocity * (modelBPM / 100), modelBPM);
        //AkSoundEngine.SetRTPCValue(TempoController.rtpcID, 30.0f);
    }
    Debug.Log("Average Difference: " + avgDiff);
    //AdjacentRealtimeDifferences.RemoveAll(d => d != null);
    AdjacentRealtimeDifferences.Clear();
}
//This is our initial precision value. 
//float magnitude = Mathf.Abs(diff);

// Determine perfect velocity within provided margin of error (epsillon)

/*
 * 
 * 

bool isMatchingVelocity = magnitude <= epsillon;

if (isMatchingVelocity)
{
    prevTargetBPM = modelBPM;
    Debug.LogFormat("{0}. <b><color=green>Perfect velocity</color></b>", details);
    Debug.Log("PrevTargetBPM = " + prevTargetBPM);

    tp.ChangePieceVelocity(pieceVelocity * (modelBPM / 100), modelBPM);
    //AkSoundEngine.SetRTPCValue(TempoController.rtpcID, 30.0f);
    return;
}
float precisionVal = GetPrecisionVal(diff, targetVelocity);

float percent = Mathf.Round(magnitude * 100);

//prevTargetBPM = modelBPM + precisionVal;

if (diff < -0.2)
{
    prevTargetBPM = 90;
}
else if (diff > 0.2)
{
    prevTargetBPM = 110;
}


string result = Math.Sign(diff) < 0 ? "Dragging" : "Leading";
//tp.ChangePieceVelocity(pieceVelocity * (prevTargetBPM / 100));
tp.ChangePieceVelocity(pieceVelocity * (prevTargetBPM / 100),  prevTargetBPM);
Debug.LogFormat("{0}. <b><color=red>{1} by {2}%</color></b>", details, result, Math.Abs(prevTargetBPM/modelBPM));
Debug.Log("PrevTargetBPM = " + prevTargetBPM);
*/
