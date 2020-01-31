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
        samples.Add(batonObject.transform.position);
        Debug.Log(batonObject.transform.position);
    }

    public void ClearSamples()
    {
        samples.Clear();
    }

    public void StartPlayback()
    {
        playbackBaton.SetActive(true);
        isPlaying = true;
    }

    private void FixedUpdate()
    {
        if(isPlaying)
        {
            playbackBaton.transform.position = samples[playbackIndex];
            playbackIndex = (playbackIndex % samples.Count) + 1;
        }
    }
}
