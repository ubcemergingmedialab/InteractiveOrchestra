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

    private List<ConductorSample> samples;
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


    private void Start()
    {
        samples = new List<ConductorSample>();
        isPlaying = false;
        teleported = false;
        playbackIndex = 0;
        conductingpos = new Vector3(view.transform.position.x, view.transform.position.y, view.transform.position.z);
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
        samples.Add(sample);
        //Debug.Log(batonObject.transform.position);
        //Debug.Log("Samples: " + samples.Count);
    }

    public void ClearSamples()
    {
        samples.Clear();
    }

    public void StartPlayback()
    {
        isPlaying = !isPlaying;
        velocityTracker.setBatonObject(playbackBaton.transform.Find("Baton_Tip2").gameObject);
    }

    private void FixedUpdate()
    {
        //Debug.Log("poll");
        if(isPlaying)
        {
            if (playbackIndex >= samples.Count)
            {
                playbackIndex = 0;
                isPlaying = false;
                playbackBaton.SetActive(isPlaying);
                view.transform.position = conductingpos;
                view.transform.Rotate(0, 180, 0);
                // turns the button's state off
                ButtonState b = button.GetComponent<ButtonState>();
                b.ToggleButtonState(false);
                teleported = false;
                velocityTracker.setBatonObject(batonObject.transform.Find("Baton_Tip").gameObject);
            }
            else
            {
                // teleports user to musicians POV
                if (!teleported)
                {
                    view.transform.position = new Vector3(view.transform.position.x, view.transform.position.y, view.transform.position.z + 25);
                    view.transform.Rotate(0, 180, 0);
                    teleported = true;
                }

                playbackBaton.SetActive(isPlaying);
                //Debug.Log("playing");
                playbackBaton.transform.position = samples[playbackIndex].position;
                playbackBaton.transform.rotation = samples[playbackIndex].rotation;

                //Debug.Log(samples[playbackIndex]);
                playbackIndex = (playbackIndex % samples.Count) + 1;

                velocityTracker.SpawnPlaneIfNotSpawned();
                velocityTracker.SetTimeSincePrevCollisionWithBasePlane();
            }
        }
    }
}
