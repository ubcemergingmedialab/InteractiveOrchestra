using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackSystem : MonoBehaviour
{
    #region Struct
    /// <summary>
    /// Struct that contains relevant information of the controller.
    /// Collected at each frame
    /// </summary>

    #endregion

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


    private void Start()
    {
        recording = new List<ConductorSample>();
        isPlaying = false;
        teleported = false;
        playbackIndex = 0;
        conductingpos = new Vector3(view.transform.position.x, view.transform.position.y, view.transform.position.z);
        button.GetComponent<ButtonState>();
    }

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

    public void GrabSample()
    {
        Vector3 pos = new Vector3(batonObject.transform.position.x, batonObject.transform.position.y+1, batonObject.transform.position.z+3);
        ConductorSample sample = new ConductorSample(pos, batonObject.transform.rotation);
        // these are some linear transformations to the vector to make it easier to see
        recording.Add(sample);
        //Debug.Log(batonObject.transform.position);
        //Debug.Log("Samples: " + samples.Count);
    }

    public void ClearSamples()
    {
        recording.Clear();
    }

    public void StartPlayback()
    {
        if (recording.Count > 0)
        {
            isPlaying = !isPlaying;
            velocityTracker.setBatonObject(playbackBaton.transform.Find("Baton_Tip2").gameObject);
        }
        else
        {
            Debug.Log("No recording to playback!");
            ButtonState b = button.GetComponent<ButtonState>();
            b.ToggleButtonState(false);
        }
    }

    private void FixedUpdate()
    {
        if(isPlaying)
        {
            // reached the end of the playback stored within the samples array
            if (playbackIndex >= recording.Count)
            {
                playbackIndex = 0;
                isPlaying = false;
                playbackBaton.SetActive(isPlaying);
                batonObject.SetActive(true);
                gestureRelated.SetActive(false);
                view.transform.position = conductingpos;
                view.transform.Rotate(0, 180, 0);
                // turns the button's state off
                ButtonState b = button.GetComponent<ButtonState>();
                b.ToggleButtonState(false);
                teleported = false;
                velocityTracker.setBatonObject(batonObject.transform.Find("Baton_Tip").gameObject);
                tempoController.StopPiece();
            }
            else
            {
                StartCoroutine(Teleport());
            }
        }
    }

    public IEnumerator Teleport()
    {
        if (!teleported)
        {
            transitions.FadeIn();
            view.transform.position = new Vector3(view.transform.position.x, view.transform.position.y, view.transform.position.z + 25);
            view.transform.Rotate(0, 180, 0);
            teleported = true;
            playbackBaton.SetActive(true);
            batonObject.SetActive(false);
            gestureRelated.SetActive(true);
            yield return new WaitForSeconds(2f);
        }
        //Debug.Log("playing");
        playbackBaton.transform.position = recording[playbackIndex].position;
        playbackBaton.transform.rotation = recording[playbackIndex].rotation;

        //Debug.Log(samples[playbackIndex]);
        playbackIndex = (playbackIndex % recording.Count) + 1;

        velocityTracker.SpawnPlaneIfNotSpawned();
        velocityTracker.SetTimeSincePrevCollisionWithBasePlane();
    }
}
