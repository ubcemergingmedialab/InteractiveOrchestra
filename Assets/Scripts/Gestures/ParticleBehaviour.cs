using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ParticleSystem particleSystem;


    public void Awake()
    {
        OVRVelocityTracker.MusicStart += ChangeAnimation;
    }
    /// <summary>
    /// Stops particle production at end of prep beat 
    /// </summary>
    public void KillParticles()
    {
        particleSystem.startLifetime = 0.0001f;
    }

    /// <summary>
    /// Starts particle production at beginning of prep beat
    /// </summary>
    public void AwakeParticles()
    {
        particleSystem.startLifetime = 1f;
    }

    public void ChangeAnimation()
    {
        Debug.Log("========================================");
        animator.parameters[0].defaultBool = true;
    }
}
