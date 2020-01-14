
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRTK.Controllables.PhysicsBased;

/// <summary>
/// The Purpose of this script is to change the speed of the music based on local tempo inputted by the player.
/// The velocity is caculated by taking a ratio between the localBPM and MasterBPM.
/// The local BPM is determined by the prep gesture
/// </summary>
[RequireComponent(typeof(AudioMaster))]
public class TempoController : MonoBehaviour
{
    private bool gestureCaptured;
    private bool isPlaying = false;

    public string rtpcID;

    // -- Set to -1 if piece is not playing
    private float timeSincePieceStart = -1f;
    private float eventStartTime;
    private float MasterBPM = 100f;
    private float localBPM = 100f;
    
    [SerializeField] private PerformanceIndicator performanceIndicator;
    private AudioMaster am;

    public delegate void TempoControllerDelegate(float localBPM);
    public delegate void TempoControllerDelegateUpdate();

    public static event TempoControllerDelegate PlayPiece;
    public static event TempoControllerDelegate PieceStop;
    public static event TempoControllerDelegate PieceInterrupt;
    public static event TempoControllerDelegateUpdate TempoOnUpdate;


    #region Unity Methods
    /// <summary>
    /// Initialize Wwise relevant objects as well as beat values
    /// </summary>
    void Start()
    {
        ulong GameObjectID = AkSoundEngine.GetAkGameObjectID(gameObject);
        am = GetComponent<AudioMaster>();
    }

   
    void FixedUpdate()
    {
        if (TempoOnUpdate != null) TempoOnUpdate();
        if (timeSincePieceStart >= 0)
        {
            timeSincePieceStart += Time.deltaTime;
        }
        if (timeSincePieceStart > 30)
        {
            PieceStop(1);
            StopPiece();
        }
    }
    #endregion

    #region Class Methods

    public bool getIsPiecePlaying()
    {
        return this.isPlaying;
    }

    /// <summary>
    /// Access Wwise functionality to play current piece if not already playing and the prep beat gesture has been completed
    /// </summary>
    public void playPiece()
    {
        StopCoroutine(BeginOrchestraPiece(localBPM));
        if (!isPlaying && IsPrepComplete)
        {
            StartCoroutine(BeginOrchestraPiece(localBPM));
        }
    }

    /// <summary>
    /// Coroutine that cues orchestra given some delay
    /// </summary>
    /// <param name="localBPM"> BPM at which piece should start </param>
    /// <returns> returns once piece begins </returns>
    IEnumerator BeginOrchestraPiece(float localBPM)
    {
        Debug.Log("Before Wait");
        isPlaying = true;
        performanceIndicator.SetTargetBPM();
        timeSincePieceStart = 0f;
        PlayPiece(localBPM);
        yield return new WaitForSeconds(OrchestraDelay.Instance.GetCurrentOrchDelay() * 0.001f);
        Debug.Log("Piece Now Play");
        AkSoundEngine.PostEvent("PieceBegins", this.gameObject);
        AkSoundEngine.SetRTPCValue(rtpcID, localBPM);
        yield return null; 
    }

    /// <summary>
    /// Updates the BPM of the piece based on dynamic localBPM
    /// </summary>
    public void UpdateOrchestraPiece()
    {
        AkSoundEngine.SetRTPCValue(rtpcID, this.localBPM);
    }

    /// <summary>
    /// Access Wwise functionality to pause current piece. 
    /// </summary>
    public void StopPiece()
    {
        am.StopEvent("PieceBegins",0);
        timeSincePieceStart = -1f;
        if (PieceStop != null) PieceInterrupt(1);
        isPlaying = false;
    }

    /// <returns>Return current localBPM</returns>
    public float GetLocalBPM()
    {
        return localBPM;
    }

    /// <summary>
    /// Set local BPM to new val
    /// </summary>
    /// <param name="newBPM"> New BPM to set localBPM </param>
    public void SetNewBPM(int newBPM)
    {
        localBPM = newBPM;
    }

    /// <summary>
    /// Get whether prep is complete and set it
    /// </summary>
    public bool IsPrepComplete { get; set; }

    #endregion
}
