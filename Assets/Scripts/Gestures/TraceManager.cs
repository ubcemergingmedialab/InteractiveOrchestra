using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class TraceManager : MonoBehaviour
{
    public GameObject prepTrace;
    public GameObject beatTrace;

    public void Start()
    {
        TempoController.PlayPiece += (tempo) =>
        {
            LookAtPlayer(prepTrace.transform);
            LookAtPlayer(beatTrace.transform);
            prepTrace.SetActive(false);
            beatTrace.SetActive(true);
        };

        TempoController.PieceStop += (tempo) =>
        {
            LookAtPlayer(prepTrace.transform);
            LookAtPlayer(beatTrace.transform);
            prepTrace.SetActive(true);
            beatTrace.SetActive(false);
        };

        TempoController.PieceInterrupt += (tempo) =>
        {
            LookAtPlayer(prepTrace.transform);
            LookAtPlayer(beatTrace.transform);
            prepTrace.SetActive(true);
            beatTrace.SetActive(false);
        };
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
}
