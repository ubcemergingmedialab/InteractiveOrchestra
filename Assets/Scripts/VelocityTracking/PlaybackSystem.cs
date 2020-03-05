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

    [SerializeField]
    public GameObject batonObject;
    public GameObject playbackBaton;
    public GameObject button;

    private void Awake()
    {
        samples = new List<ConductorSample>();
        isPlaying = false;
        playbackIndex = 0;
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
        Vector3 pos = batonObject.transform.position;
        pos.z += 3;
        pos.y += 1;
        ConductorSample sample = new ConductorSample(pos, batonObject.transform.rotation);
        // these are some linear transformations to the vector to make it easier to see
        samples.Add(sample);
        Debug.Log(batonObject.transform.position);
        Debug.Log("Samples: " + samples.Count);
    }

    public void ClearSamples()
    {
        samples.Clear();
    }

    public void StartPlayback()
    {
        isPlaying = !isPlaying;
        Debug.Log(isPlaying);
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
                // turns the button's state off
                ButtonState b = button.GetComponent<ButtonState>();
                b.ToggleButtonState(false);
            }
            else
            {
                playbackBaton.SetActive(isPlaying);
                Debug.Log("playing");
                playbackBaton.transform.position = samples[playbackIndex].position;
                playbackBaton.transform.rotation = samples[playbackIndex].rotation;
                Debug.Log(samples[playbackIndex]);
                playbackIndex = (playbackIndex % samples.Count) + 1;
            }
        }
    }
}
