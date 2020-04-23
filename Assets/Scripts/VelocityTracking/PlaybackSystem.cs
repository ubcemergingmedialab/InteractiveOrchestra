using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script manages the record and playback of the last conducting sample taken.
/// </summary>
public class PlaybackSystem : MonoBehaviour
{
    private List<ConductorSample> recording;
    private bool isPlaying;
    private int playbackIndex;
    private bool teleported;
    private Vector3 conductingpos;

    [SerializeField]
    public GameObject batonObject;
    public GameObject playbackBaton;
    public GameObject button;
    public GameObject view;
    public OVRVelocityTracker velocityTracker;
    public TempoController tempoController;
    public GameObject gestureRelated;
    public CameraTransitions transitions;

    #region Struct

    /// <summary>
    /// Struct that contains relevant information of the controller.
    /// Collected at each frame
    /// </summary>

    private struct ConductorSample
    {
        public Vector3 position;
        public Quaternion rotation;

        public ConductorSample(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    #endregion

    #region Class Methods

    private void Start()
    {
        recording = new List<ConductorSample>();
        isPlaying = false;
        teleported = false;
        transitions.transitioning = false;
        playbackIndex = 0;
        conductingpos = new Vector3(view.transform.position.x, view.transform.position.y, view.transform.position.z);
        button.GetComponent<ButtonState>();
    }

    /// <summary>
    /// Creates a conducting sample based on baton's position and rotation and add to recording.
    /// </summary>
    public void GrabSample()
    {
        Vector3 pos = new Vector3(batonObject.transform.position.x, batonObject.transform.position.y + 1, batonObject.transform.position.z + 3);
        ConductorSample sample = new ConductorSample(pos, batonObject.transform.rotation);
        // these are some linear transformations to the vector to make it easier to see
        recording.Add(sample);
    }

    /// <summary>
    /// Clear all samples from recording.
    /// </summary>
    public void ClearSamples()
    {
        recording.Clear();
    }

    /// <summary>
    /// Trigger for playback button. 
    /// </summary>
    public void StartPlayback()
    {
        if (recording.Count > 0)
        {
            isPlaying = !isPlaying;
            velocityTracker.SetBatonObject(playbackBaton.transform.Find("Baton_Tip2").gameObject);
        }
        // if there are have not been any conducting gestures done by user.
        else
        {
            Debug.Log("No recording to playback!");
            ButtonState b = button.GetComponent<ButtonState>();
            b.ToggleButtonState(false);
        }
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            // reached the end of the playback stored within the samples array
            if (playbackIndex >= recording.Count)
            {
                StartCoroutine(TransitionToConductorView());
            }
            else
            {
                if (!teleported && !transitions.transitioning)
                {
                    StartCoroutine(TransitionToAudienceView());
                }
                else if (teleported && !transitions.transitioning)
                {
                    // playback occurs here
                    playbackBaton.transform.position = recording[playbackIndex].position;
                    playbackBaton.transform.rotation = recording[playbackIndex].rotation;
                    playbackIndex = (playbackIndex % recording.Count) + 1;
                    // activates velocityTracker functionality
                    velocityTracker.SpawnPlaneIfNotSpawned();
                    velocityTracker.SetTimeSincePrevCollisionWithBasePlane();
                }
            }
        }
    }

    /// <summary>
    /// Activates fade animation and changes user position and sets playback baton active.
    /// </summary>
    private IEnumerator TransitionToAudienceView()
    {
        transitions.transitioning = true;
        transitions.FadeIn("Starting Playback");
        yield return new WaitForSeconds(2.5f);

        SetToAudiencePosition();

        batonObject.SetActive(false);
        gestureRelated.SetActive(true);

        yield return new WaitForSeconds(1f);
        teleported = true;
        transitions.FadeOut();

        yield return new WaitForSeconds(2.5f);
        playbackBaton.SetActive(true);
        transitions.transitioning = false;
    }

    /// <summary>
    /// changes user position back to conducting position and resets all fields back to default.
    /// </summary>
    private IEnumerator TransitionToConductorView()
    {
        playbackIndex = 0;
        isPlaying = false;
        tempoController.StopPiece();
        gestureRelated.SetActive(false);
        playbackBaton.SetActive(false);
        transitions.FadeIn("Ending Playback");

        yield return new WaitForSeconds(2.5f);
        batonObject.SetActive(true);
        SetToConductorPosition();

        yield return new WaitForSeconds(1f);
        ButtonState b = button.GetComponent<ButtonState>();
        // turns the button's state off
        b.ToggleButtonState(false);
        teleported = false;
        transitions.FadeOut();
        velocityTracker.SetBatonObject(batonObject.transform.Find("Baton_Tip").gameObject);

        yield return new WaitForSeconds(2.5f);
    }

    /// <summary>
    /// Switches camera to the musicians' pov.
    /// </summary>
    private void SetToAudiencePosition()
    {
        view.transform.position = new Vector3(view.transform.position.x, view.transform.position.y, view.transform.position.z + 20);
        view.transform.Rotate(0, 180, 0);
    }

    /// <summary>
    /// changes user position back to conducting position.
    /// </summary>
    private void SetToConductorPosition()
    {
        view.transform.position = conductingpos;
        view.transform.Rotate(0, 180, 0);
    }

    #endregion

}
