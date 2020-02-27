using System.Collections;
using UnityEngine;

public class CameraTransitions : MonoBehaviour
{

    public Animator animator;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //StartCoroutine(FadeIn());
            
        }
    }

    public IEnumerator FadeOut()
    {
        animator.SetBool("FadeIn", false);
        yield return new WaitForSeconds(2f);
    }

    public void FadeIn()
    {
        animator.SetBool("FadeIn", true);
    }
}
