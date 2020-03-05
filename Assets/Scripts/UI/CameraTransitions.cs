using System.Collections;
using UnityEngine;
using UnityEngine.UI;


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

    public void FadeOut()
    {
        animator.SetBool("FadeIn", false);
        txt.text = "";
    }

    public void FadeIn(string msg)
    {
        animator.SetBool("FadeIn", true);
        txt.text = msg;
    }
}
