using System.Collections;
using UnityEngine;

public class CameraTransitions : MonoBehaviour
{

    public Animator animator;
    public bool transitioning;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //StartCoroutine(FadeIn());
            
        }
    }

    public void FadeOut()
    {
        animator.SetBool("FadeIn", false);
    }

    public void FadeIn()
    {
        animator.SetBool("FadeIn", true);
    }
}
