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

    private List<Vector3> samples;
    private bool isPlaying;
    private int playbackIndex;

    [SerializeField]
    public GameObject batonObject;
    public GameObject playbackBaton;

    private void Start()
    {
        samples = new List<Vector3>();
        isPlaying = false;
        playbackIndex = 0;
    }

    public void GrabSample()
    {
        Vector3 pos = batonObject.transform.position;
        pos.z = pos.z + 3;
        pos.y = pos.y + 3;
        samples.Add(pos);
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
        playbackBaton.SetActive(isPlaying);
        Debug.Log(isPlaying);
    }

    private void FixedUpdate()
    {
        Debug.Log("poll");
        if(isPlaying)
        {
            Debug.Log("playing");
            playbackBaton.transform.position = samples[playbackIndex];
            Debug.Log(samples[playbackIndex]);
            playbackIndex = (playbackIndex % samples.Count) + 1;
           if (playbackIndex >= samples.Count)
            {
                playbackIndex = 0;
                isPlaying = false;
                playbackBaton.SetActive(isPlaying);
            }
        }
    }
}
