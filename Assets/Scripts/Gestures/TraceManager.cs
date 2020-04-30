using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Connects the traces to music events and exposes some functions to enable and disable
/// </summary>
public class TraceManager : MonoBehaviour
{
    public GameObject prepTrace;
    public GameObject beatTrace;
    private bool PrepIsDone = false;
    private bool SongisDone = false;

    public void Start()
    {
        TempoController.PlayPiece += (tempo) =>
        {
            SetPrepIsDone(true);
            ActivateBeatTrace();
        };

        TempoController.PieceStop += (tempo) =>
        {
            SetSongIsDone(true);
            ActivatePrepTrace();
        };

        TempoController.PieceInterrupt += (tempo) =>
        {
            SetSongIsDone(true);
            ActivatePrepTrace();
        };
    }

    public bool GetSongIsDone()
    {
        return this.SongisDone;
    }

    public void SetSongIsDone(bool val)
    {
        this.SongisDone = val;
    }

    public bool GetPrepIsDone()
    {
        return this.PrepIsDone;
    }

    public void SetPrepIsDone(bool val)
    {
        this.PrepIsDone = val;
    }

    public void ActivatePrepTrace()
    {
        LookAtPlayer(prepTrace.transform);
        LookAtPlayer(beatTrace.transform);
        prepTrace.SetActive(true);
        beatTrace.SetActive(false);
    }

    public void ActivateBeatTrace()
    {
        LookAtPlayer(prepTrace.transform);
        LookAtPlayer(beatTrace.transform);
        prepTrace.SetActive(false);
        beatTrace.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void LookAtPlayer(Transform trace)
    {
        Vector3 tracePosition = trace.transform.position;
        Vector3 playerPosition = Camera.main.transform.position;
        tracePosition.Set(tracePosition.x, 0, tracePosition.z);
        playerPosition.Set(playerPosition.x, 0, playerPosition.z);
        trace.transform.right = (playerPosition - tracePosition);
    }

    public void DisableTraces()
    {
        prepTrace.SetActive(false);
        prepTrace.SetActive(false);
    }
}
