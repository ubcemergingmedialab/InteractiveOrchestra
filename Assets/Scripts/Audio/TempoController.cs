
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Purpose of this script is to change the speed of the music based on local tempo inputted by the player.
/// The velocity variable is caculated by taking a ratio between the localBPM and MasterBPM.
/// The local BPM is recorded by using the beatLengthTracker which stores 
/// </summary>
[RequireComponent(typeof(AudioMaster))]
public class TempoController : MonoBehaviour
{
    #region Public variables
    public bool isPrepComplete = false;
    public Slider audioSlider;
    public OVRConductorGesture conductor;
    public float threshold;
    public string rtpcID;
    public int[] timeSignature = { 4, 4 };
    public Articulation articulation;
    public enum Articulation
    {
        LEGATO,
        STACCATO
    }
    #endregion

    #region Private Variables
    private AudioMaster am;
    private AkAmbient amb;
    private float eventStartTime;
    private float MasterBPM = 93f;
    private float localBPM = 93f;
    private float velocity = 75;
    private int numBeats;
    private int CurrBeat = 0;
    private int beatsPerBar;
    private string articulationIdentifier;
    #endregion

    #region Conductor Gesture Variables
    private bool gestureCaptured;
    private bool isPlaying = false;
    private string[] gestures;
    private string gestureString = "PREP";
    private float[] beatLengthTracker;
    private float gestureScore; 
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initialize Wwise relevant objects as well as beat values
    /// </summary>
    void Start()
    {
        for (int i = 0; i < beatsPerBar; i++)
        {
            gestures[i] = beatsPerBar + "" + timeSignature[1] + articulationIdentifier + (i % beatsPerBar + 1);
        }
        ulong GameObjectID = AkSoundEngine.GetAkGameObjectID(gameObject);
        this.numBeats = 0;
        audioSlider.minValue = 0;
        audioSlider.maxValue = 110; 
        audioSlider.value = 0;
        beatsPerBar = timeSignature[0];
        am = GetComponent<AudioMaster>();
        beatLengthTracker = new float[beatsPerBar - 1];
        articulationIdentifier = articulation.ToString().Substring(0, 1);
        CurrBeat = 0;
        gestures = new string[beatsPerBar]; 
    }

    /// <summary>
    /// Calculates average time between beats to determine the localBPM of the user. 
    /// </summary>
    void FixedUpdate()
    {
        if (gestureCaptured)
        {
            if (gestureString == gestures[CurrBeat])
            {
                if (CurrBeat == 0)
                {
                    if (!isPlaying)
                    {
                        //playPiece();
                    }
                    eventStartTime = Time.time;
                    numBeats++;
                }
                else
                {
                    beatLengthTracker[CurrBeat] = Time.time - eventStartTime;
                    numBeats++;
                    CurrBeat++;
                }
            }
            
            int barsCrossed = numBeats / beatsPerBar;
            if (numBeats % this.beatsPerBar == 0)
            {
                float avgScore = gestureScore / beatsPerBar;
                this.calculateBPM(beatLengthTracker);
                if(avgScore < threshold)
                {
                    if (velocity > 25f)
                    {
                        velocity -= 20 * avgScore;
                        Debug.Log("Should be slowing down now " + velocity);
                        AkSoundEngine.SetRTPCValue(rtpcID, velocity);
                    }
                }
                else
                {
                    if (velocity < 75f)
                    {
                        velocity += 40 * avgScore;
                    }
                    AkSoundEngine.SetRTPCValue(rtpcID, velocity);
                }
                CurrBeat = 0;
            }
            gestureCaptured = false;
        }
        updateSlider();
    }
    #endregion

    #region Class Methods



    /// <summary>
    /// Calculates the localBPM of the user given a list of beat lengths. 
    /// </summary>
    /// <param name="BeatLengthTracker"> List of delta time between beats </param>
    private void calculateBPM(float[] BeatLengthTracker)
    {
        float SumOfBeatLengths = 0; 
        foreach(float f in beatLengthTracker)
        {
            SumOfBeatLengths = SumOfBeatLengths + f;
        }
        localBPM = 60 / (SumOfBeatLengths / 3);
    }

    /// <summary>
    /// Set new gesture string
    /// </summary>
    /// <param name="gesture">Gesture string obtained from ConductorGesture.HandleOnDeveloperDefinedMatch </param>
    public void setGestureString(string gesture)
    {
        gestureString = gesture;
    }

    /// <summary>
    /// Defines if a gesture has been captured
    /// </summary>
    /// <param name="captured"> Boolean value from ConductorGesture.HandleOnDeveloperDefinedMatch</param>
    public void setGestureCaptured(bool captured)
    {
        gestureCaptured = captured;
    }

    /// <summary>
    /// Defines the correctness score of captured gesture
    /// </summary>
    /// <param name="score"> Score value from ConductorGesture.HandleOnDeveloperDefinedMatch</param>
    public void setGestureScore(float score)
    {
           gestureScore += score;
    }


    /// <summary>
    /// Access Wwise functionality to play current piece if not already playing and the prep beat gesture has been completed
    /// </summary>
    public void playPiece()
    {
        Debug.Log("Piece Starts");
        if (!isPlaying && isPrepComplete)
        {
            AkSoundEngine.PostEvent("PieceBegins", this.gameObject);
            AkSoundEngine.SetRTPCValue(rtpcID, 75 * (localBPM/MasterBPM));
            //Debug.Log("Ratio: " + (localBPM / MasterBPM));
            isPlaying = true;
        }
    }

    /// <summary>
    /// Access Wwise functionality to pause current piece. 
    /// </summary>
    public void stopPiece()
    {
        am.StopEvent("PieceBegins",0);
        this.numBeats = 0;
        CurrBeat = 0;
        conductor.Reset();

        audioSlider.minValue = 0;
        audioSlider.maxValue = 110; // hard-coded for now
        audioSlider.value = 0;

        isPlaying = false;
    }

    /// <summary>
    /// Updates current value of slider. Slider range is defined by totaly number of beats in a piece 
    /// </summary>
    private void updateSlider()
    {
        audioSlider.value = numBeats;
    }

    /// <returns>Return current gesture string (Eg. 44L1, 44L2, etc)</returns>
    public string getGestureString()
    {
        return gestureString;
    }

    /// <returns>Return current event start time. Event start time defined as the landing of the first beat of four</returns>
    public float getEventStartTime()
    {
        return eventStartTime;
    }

    /// <returns>Return current beat</returns>
    public int getCurrBeat()
    {
        return CurrBeat;
    }
    /// <returns>Return current localBPM</returns>
    public float getLocalBPM()
    {
        return localBPM;
    }

    public void setNewBPM(int newBPM)
    {
        localBPM = newBPM;
    }
    #endregion
}
