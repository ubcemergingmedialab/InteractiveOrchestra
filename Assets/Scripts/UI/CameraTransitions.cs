using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script acts as the wrapper for transition animations in game.
/// </summary>
public class CameraTransitions : MonoBehaviour
{
    public Animator animator;
    public GameObject text;
    public bool transitioning;
    private Text txt;

    private void Start()
    {
        txt = text.GetComponent<Text>();
        txt.text = "";
    }

    /// <summary>
    /// Camera view fade out from black screen to normal effect. 
    /// </summary>
    public void FadeOut()
    {
        animator.SetBool("FadeIn", false);
        txt.text = "";
    }

    /// <summary>
    /// Camera view fade into black screen with optional text.
    /// <param name="msg"> text input to be displayed on camera view. </param>
    /// </summary>
    public void FadeIn(string msg)
    {
        animator.SetBool("FadeIn", true);
        txt.text = msg;
    }
}
