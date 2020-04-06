using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerAnimations : MonoBehaviour
{
    public Animator animator;

    public void ShowGripButton()
    {
        animator.SetTrigger("grip");
    }

    public void ShowTriggerButton() 
    {
        animator.SetTrigger("trigger");
    }
}
