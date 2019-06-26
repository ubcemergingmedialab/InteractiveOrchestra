using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// Velocity Tracker is in charge of collecting and evaluating ConductorSamples. 
/// GatherSample is used for data evaluation.TrackAndStoreVelocity is used for ConductorSample collection. 
/// </summary>
public class VelocityTracker : OVRGestureHandle
{

    #region Variables
    private float PrevTime = 0;
    private float Ourtime = 0.05f;
    private float DeltaTime = 0.05f;
    private float OffsetToCompare = 0.012f;
    private float ModelBPM;
    private float ModelBarLength;

    private int SmoothingWindowSize = 5;
    private int Repetitions;
    private int MaxRepetitions = 2;

    private bool DataHasBeenAveraged = false;
    private bool DataHasbeenCleaned  = false;
    private bool PrintedTheAverage   = false;
    private bool Printed = false;
    private bool SetVelocitySampleProcessor = false;

    private VelocitySampleProcessor velocitySampleProcessor;
    //private WriteToText writeToText = new WriteToText();
    public static List<ConductorSample> samples;
   // private ControllerDataUpdater dataChanger;
    private ControllerDataUpdater dataChanger;
    //private ControllerDataUpdater dataChanger;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initialize game objects and other values
    /// </summary>
    private void Awake()
    {
        samples = new List<ConductorSample>();
        Repetitions = 0;
        ModelBPM = tempoController.getLocalBPM();
        ModelBarLength = (60*4)/ModelBPM;
        velocitySampleProcessor = new VelocitySampleProcessor();
        dataChanger = new ControllerDataUpdater();
        
    }
    #endregion

    #region Class Methods

    /// <summary>
    /// Called when velocity data is already provided and stored. Evaluates a collected conductorSample at every frame 
    /// </summary>
    /// <param name="gestureString"> Current gesture the sample was collected at </param>
    /// <param name="device"> Device corresponding to the baton </param>
    internal void GatherSample(string gestureString, SteamVR_Controller.Device device)
    {
        float currTime = Mathf.Round((Time.time - tempoController.getEventStartTime()) * 1000.0f) / 1000.0f;
        if (tempoController.getGestureString() != "PREP")
        {
    
        }
    }



    /// <summary>
    /// Collects conductorSamples over 'maxRepetitions' number of bars. 
    /// </summary>
    /// <param name="gestureString"> Current gesture the sample was collected at </param>
    /// <param name="device"> Device corresponding to the baton </param>
    public void TrackAndStoreVelocity(string gestureString, SteamVR_Controller.Device device)
    {
        float currTime = Mathf.Round((Time.time - tempoController.getEventStartTime()) * 1000.0f) / 1000.0f;
        if (tempoController.getGestureString() != "PREP")
        {
            Debug.Log(device.velocity);
            if (PrevTime - currTime > 0 && Repetitions < MaxRepetitions)
            {
                Repetitions++;
            }
            if (Repetitions < MaxRepetitions && Repetitions != 0)
            {
                float currentVelocity = device.velocity.magnitude;
                samples.Add(new ConductorSample(device.velocity,device.transform.pos, currentVelocity, currTime, tempoController.getCurrBeat()));
            }

            }
            if (DataHasBeenAveraged && !Printed)
            {

            foreach (ConductorSample cs in samples)
            {
                Debug.Log("Direction: " + cs.velocity + "Beat: " + cs.beat + "Time: " + cs.time);
            }

            Printed = true;
            }

            if (Printed && !SetVelocitySampleProcessor)
            {
            velocitySampleProcessor = VelocitySampleProcessor.instance;
            velocitySampleProcessor.SetNewSamples(samples);
            SetVelocitySampleProcessor = true;
            }

        PrevTime = currTime;
            
        }
    

    
    /// <summary>
    /// Iterate through list of gathered conductor samples and average.
    /// </summary>
    /// <param name="samples"> List of conductor samples collected </param>
    /// <returns> List of conductor samples corresponding to the average bar velocity at various time instances </returns>
    private List<ConductorSample> FindAverageOfSamples(List<ConductorSample> samples)
    {
        Vector3 avgDirection = new Vector3(0, 0, 0);
        Vector3 avgPosition  = new Vector3(0, 0, 0);
        float avgVelocity = 0;
        int currBeat = 0;
        ModelBPM = tempoController.getLocalBPM();
        ModelBarLength = (60 * 4) / ModelBPM;
        List<ConductorSample> FinalList = new List<ConductorSample>();
        for (float t = 0.05f; t <= 2.35f; t += 0.05f)
        {
            t = Mathf.Round(t * 1000.0f) / 1000.0f;
            List<ConductorSample> currList = samples.FindAll(cs => cs.time == t);

            List<float> velocity_t = new List<float>();
            foreach (ConductorSample cs in currList)
            {
                avgDirection.x += cs.direction.x;
                avgDirection.y += cs.direction.y;
                avgDirection.z += cs.direction.z;

                avgPosition.x += cs.position.x;
                avgPosition.y += cs.position.y;
                avgPosition.z += cs.position.z;

                avgVelocity += cs.velocity;
                velocity_t.Add(cs.velocity);
                currBeat = cs.beat;
            }
            float ListLength = (float)currList.Count;
            if (ListLength != 0)
            {
                float x = t / ModelBarLength;
                FinalList.Add(new ConductorSample(avgDirection / ListLength, avgPosition / ListLength, avgVelocity / ListLength, t / ModelBarLength, currBeat));
                
            }
            avgDirection.x = 0;
            avgDirection.y = 0;
            avgDirection.z = 0;

            avgPosition.x = 0;
            avgPosition.y = 0;
            avgPosition.z = 0;

            avgVelocity = 0;
        }

        return FinalList;
    }



    /// <summary>
    /// Take points at SmoothingWindowSize units to the right and left of each point 
    /// and average them to get a smoother overall velocity curve.
    /// </summary>
    /// <param name="samples"> List of conductor samples </param>
    /// <returns> Smoothed list of samples </returns>
    private List<ConductorSample> AverageSmoothing(List<ConductorSample> samples)
    {
        List<ConductorSample> smoothedSamples = new List<ConductorSample>();
        for (int i = SmoothingWindowSize; i < samples.Count - SmoothingWindowSize; i++)
        {

            float newAverageVelocity = 0;
            for (int j = -SmoothingWindowSize;  j < SmoothingWindowSize; j++)
            {
                newAverageVelocity = newAverageVelocity + samples[i + j].velocity;
            }
            newAverageVelocity = newAverageVelocity / (2 * SmoothingWindowSize - 1);
            Debug.Log(newAverageVelocity);
            smoothedSamples.Add(new ConductorSample(samples[i].direction, samples[i].position, newAverageVelocity, samples[i].time, samples[i].beat));


        }
        return smoothedSamples;
    }




    /// <summary>
    /// Interpolate the velocity at time intervals 0.05f.
    /// Replace all the time values collected for intervals of 0.05. Useful for averaging the data later
    /// </summary>
    /// <param name="samples"> List of conductor samples</param>
    /// <returns> List of conductor samples with modified time value </returns>
    private List<ConductorSample> RoundSamples(List<ConductorSample> samples)
    {
        ConductorSample prevSample = new ConductorSample(new Vector3(0,0,0), new Vector3(0,0,0), 0,0,0);
        List<ConductorSample> FinalList = new List<ConductorSample>();
        for (float t = 0.05f; t <= 2.4; t += 0.05f)
        {
            t = Mathf.Round(t * 1000.0f) / 1000.0f;
            foreach (ConductorSample sm in samples)
            {
                if (t < sm.time && ApproximatelyEquals(t,sm.time,OffsetToCompare))
                {
                        float sampleInterp = Mathf.InverseLerp(sm.time, prevSample.time, t);
                    
                        float newVelocity = Mathf.Lerp(sm.velocity, prevSample.velocity, sampleInterp);
                        FinalList.Add(new ConductorSample(sm.direction,sm.position,newVelocity,t,sm.beat));
                        continue;

                }
                prevSample = sm;
            }
        }
        return FinalList;
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

    #region Struct
    /// <summary>
    /// Struct that contains relevant information of the controller.
    /// Collected at each frame
    /// </summary>
    [Serializable]
    public struct ConductorSample
    {
        
        public Vector3 direction;
        public Vector3 position;
        public float velocity;
        public float time;
        public int beat;

        public ConductorSample( Vector3 d , Vector3 pos, float mag, float t, int b)
        {
            
            position = pos;
            velocity = mag;
            time = t;
            beat = b;
            direction = d.normalized;
            position.z = 0;
            direction.z = 0;

        }

    }
    #endregion


}



// CODE GRAVEYARD
/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 *  /*private Dictionary<float, List<float>> velocity44L1Mag = new Dictionary<float, List<float>>();
    private Dictionary<float, List<float>> velocity44L2Mag = new Dictionary<float, List<float>>();
    private Dictionary<float, List<float>> velocity44L3Mag = new Dictionary<float, List<float>>();
    private Dictionary<float, List<float>> velocity44L4Mag = new Dictionary<float, List<float>>();

    private Dictionary<float, List<Vector3>> velocity44L1V = new Dictionary<float, List<Vector3>>();
    private Dictionary<float, List<Vector3>> velocity44L2V = new Dictionary<float, List<Vector3>>();
    private Dictionary<float, List<Vector3>> velocity44L3V = new Dictionary<float, List<Vector3>>();
    private Dictionary<float, List<Vector3>> velocity44L4V = new Dictionary<float, List<Vector3>>();

    private Dictionary<float, float> Averagevelocity44L1Mag = new Dictionary<float, float>();
    private Dictionary<float, float> Averagevelocity44L2Mag = new Dictionary<float, float>();
    private Dictionary<float, float> Averagevelocity44L3Mag = new Dictionary<float, float>();
    private Dictionary<float, float> Averagevelocity44L4Mag = new Dictionary<float, float>();

    private Dictionary<float, Vector3> Averagevelocity44L1V = new Dictionary<float, Vector3>();
    private Dictionary<float, Vector3> Averagevelocity44L2V = new Dictionary<float, Vector3>();
    private Dictionary<float, Vector3> Averagevelocity44L3V = new Dictionary<float, Vector3>();
    private Dictionary<float, Vector3> Averagevelocity44L4V = new Dictionary<float, Vector3>();*/


/*switch (TempoController.gestureString)
{
    // Important to note that the gestures are currently hard coded. Again, in the future we should generalize these
    // Have some function that takes in a parameter relating to the time signature and then create strings based on those parameters

    case "44L1":
        /*
         * Each case does the following
         * First it checks if velocity44L1Mag (which contains the velocity squared magnitudes) contains the key for the current time
         * If it does, then add the velocity that was just recorded to the list at that key
         * If it doesn't then create a new list, add the current velocity to that list and then insert the time and list pair
         * as a new keyval pair inside our dictionary. 
         * Same process occurs for the velocity44L1V dictionary
         * */


/*if (velocity44L1Mag.ContainsKey(currTime))
{
    velocity44L1Mag[currTime].Add(device.velocity.sqrMagnitude);
}
else
{
    List<float> newListToAdd = new List<float>();
    newListToAdd.Add(device.velocity.sqrMagnitude);
    velocity44L1Mag.Add(currTime, newListToAdd);
}
if (velocity44L1V.ContainsKey(currTime))
{
    velocity44L1V[currTime].Add(device.velocity);
}
else
{
    List<Vector3> newListToAdd = new List<Vector3>();
    newListToAdd.Add(device.velocity);
    velocity44L1V.Add(currTime, newListToAdd);
}

break;
case "44L2":
if (velocity44L2Mag.ContainsKey(currTime))
{
    velocity44L2Mag[currTime].Add(device.velocity.sqrMagnitude);
}
else
{
    List<float> newListToAdd = new List<float>();
    newListToAdd.Add(device.velocity.sqrMagnitude);
    velocity44L2Mag.Add(currTime, newListToAdd);
}
if (velocity44L2V.ContainsKey(currTime))
{
    velocity44L2V[currTime].Add(device.velocity);
}
else
{
    List<Vector3> newListToAdd = new List<Vector3>();
    newListToAdd.Add(device.velocity);
    velocity44L2V.Add(currTime, newListToAdd);
}
break;
case "44L3":
if (velocity44L3Mag.ContainsKey(currTime))
{
    velocity44L3Mag[currTime].Add(device.velocity.sqrMagnitude);
}
else
{
    List<float> newListToAdd = new List<float>();
    newListToAdd.Add(device.velocity.sqrMagnitude);
    velocity44L3Mag.Add(currTime, newListToAdd);
}
if (velocity44L3V.ContainsKey(currTime))
{
    velocity44L3V[currTime].Add(device.velocity);
}
else
{
    List<Vector3> newListToAdd = new List<Vector3>();
    newListToAdd.Add(device.velocity);
    velocity44L3V.Add(currTime, newListToAdd);
}
break;
case "44L4":
if (velocity44L4Mag.ContainsKey(currTime))
{
    velocity44L4Mag[currTime].Add(device.velocity.sqrMagnitude);
}
else
{
    List<float> newListToAdd = new List<float>(0);
    newListToAdd.Add(device.velocity.sqrMagnitude);
    velocity44L4Mag.Add(currTime, newListToAdd);
}
if (velocity44L4V.ContainsKey(currTime))
{
    velocity44L4V[currTime].Add(device.velocity);
}
else
{
    List<Vector3> newListToAdd = new List<Vector3>();
    newListToAdd.Add(device.velocity);
    velocity44L4V.Add(currTime, newListToAdd);
}
break;
default:
break;
}*/


/*
       if (repetitions == 10 && !DataHasbeenCleaned)
       {
           Debug.Log("============================================================");
           Debug.Log(velocity44L1Mag.Count + " " + velocity44L2Mag.Count + " " + velocity44L3Mag.Count + " " + velocity44L4Mag.Count + " ");
           Debug.Log(velocity44L1V.Count + " " + velocity44L2V.Count + " " + velocity44L3V.Count + " " + velocity44L4V.Count + " ");

           //This checks each list in the dictionary and makes sure that is has a minimum number of data points. 
           //For instance, if after 50 trials we have a key 0.14 that only has one reading, then we should just ignore it. 

           //For generalization later on in this project we should consider putting the velocitydictionaries into a dictionary List
           // That way we can give a function a parameter indicating how many gesture we need. Say if we're doing 3/4 timing, and we can
           // instantiate these dictionaries dynamically. 
           /*CleanUpData(velocity44L1Mag);
           CleanUpData(velocity44L2Mag);
           CleanUpData(velocity44L3Mag);
           CleanUpData(velocity44L4Mag);

           CleanUpData(velocity44L1V);
           CleanUpData(velocity44L2V);
           CleanUpData(velocity44L3V);
           CleanUpData(velocity44L4V);
           Debug.Log("Cleaned!");
           Debug.Log(velocity44L1V.Count + " " + velocity44L2V.Count + " " + velocity44L3V.Count + " " + velocity44L4V.Count + " ");
           Debug.Log(velocity44L1Mag.Count + " " + velocity44L2Mag.Count + " " + velocity44L3Mag.Count + " " + velocity44L4Mag.Count + " ");
           Debug.Log("============================================================");
           DataHasbeenCleaned = true;

            * I need to figure out how to save that average data to a text file
            * Then using that dummy data, try to modify the velocity in my own controller.
            * this means that I need my own dummy data of 50 Bpm, 100 Bpm and 150 Bpm.
            * If it works then we're on a really good track. 
            * Iron Python?
            * Live Data .net
            * DXR
            * 

           // This method will find the average among each key list pair and save it as a float in the same key position for some other dictionary.




    
    private void CleanUpData<T>(Dictionary<float, List<T>> velocityList)
    {
        List<float> toRemove = new List<float>();
        foreach (float keys in velocityList.Keys)
        {
            List<T> currList = velocityList[keys];
            // At the moment this will remove any entry in the dictionary that does not contain more than at least repetitions-2 data points
            // It's hard coded for now, but we can modify this for better results later 

            if (currList.Count < repetitions - 2)
            {
                toRemove.Add(keys);
            }
        }
        //We need this for each loop because dictionaries dont like having items removed while iterating through them 
        foreach (float f in toRemove)
        {
            velocityList.Remove(f);
        }
    }


private Dictionary<float, Vector3> GetGestureVelocityAverageV(Dictionary<float, List<Vector3>> velocityGestureDictV)
    {
        Dictionary<float, Vector3> solution = new Dictionary<float, Vector3>();
        foreach (KeyValuePair<float, List<Vector3>> curr in velocityGestureDictV)
        {
            float x = 0;
            float y = 0;
            float z = 0;

            foreach (Vector3 v in curr.Value)
            {
                x += v.x;
                y += v.y;
                z += v.z;

            }
            x = x / curr.Value.Count;
            y = y / curr.Value.Count;
            z = z / curr.Value.Count;
            solution.Add(curr.Key, new Vector3(x, y, z));
        }
        return solution;
    }

    // Return a dictionary with time as the key and average velocity at that time for the value. 
    private Dictionary<float, float> GetGestureVelocityAverageMag(Dictionary<float, List<float>> velocityGestureDict)
    {

        Dictionary<float, float> solution = new Dictionary<float, float>();
        foreach (KeyValuePair<float, List<float>> curr in velocityGestureDict)
        {
            solution.Add(curr.Key, curr.Value.Average());
        }
        return solution;
    }
           */
