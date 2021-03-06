﻿using UnityEngine;

/// <summary>
/// This Script handles the visualization of the 4/4 conducting gesture.
/// </summary>
public class ParticleBehaviour : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ParticleSystem particleSystem;

    /// <summary>
    /// Subscribe the change animation function to the music start. 
    /// </summary>
    public void Awake()
    {
        OVRVelocityTracker.MusicStart += ChangeAnimation;
        TempoController.PlayPiece += SetAnimationSpeed;
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

    /// <summary>
    /// Change animation to full gesture 
    /// </summary>
    public void ChangeAnimation()
    {
        animator.SetBool("PrepGestureCompleted", true);
    }

    /// <summary>
    /// Set speed of the animation according to the localBPM in TempoController
    /// </summary>
    public void SetAnimationSpeed(float localBPM)
    {
        animator.speed = localBPM/100;
    }

    /// <summary>
    /// API for gesture button that toggles state of this class to emit or stop emitting particle effect.
    /// </summary>
    public void ToggleGestureActivation()
    {
        particleSystem.enableEmission = !particleSystem.enableEmission;
    }
}
