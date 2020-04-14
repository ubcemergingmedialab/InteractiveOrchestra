using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This Script acts as wrapper functionality for triggering animations from the TutorialControllers Animator.  
*/
public class ControllerAnimations : MonoBehaviour
{
    public Animator animator;

    /// <summary>
    /// Plays the 'grip' animation.
    /// </summary>
    public void ShowGripButton()
    {
        animator.SetTrigger("grip");
    }

    /// <summary>
    /// Plays the 'trigger' animation.
    /// </summary>
    public void ShowTriggerButton() 
    {
        animator.SetTrigger("trigger");
    }
}
