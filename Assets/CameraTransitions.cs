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
        txt.text = "";
    }

    public void FadeIn(string msg)
    {
        animator.SetBool("FadeIn", true);
        txt.text = msg;
    }
}
