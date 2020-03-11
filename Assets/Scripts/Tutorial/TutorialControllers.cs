using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControllers : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller m_controller;
    [SerializeField]
    private Animator m_animator = null;

    // Update is called once per frame
    void Update()
    {
        m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, m_controller) ? 1.0f : 0.0f);
        m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, m_controller) ? 1.0f : 0.0f);
        m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x);
        m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).y);
        m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller));
        m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller));
    }
}
