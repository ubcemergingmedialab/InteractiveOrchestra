using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progressBarMoverOnTrigger : MonoBehaviour {
    [SerializeField]
    private Slider progressBarSlider;
    private static float songLength;
    private float timeStarter;
    private bool mPressed;
    private float valueToAdd;
    private bool isPiecePlaying;

	// Use this for initialization
	void Start () {
        //  progressBarSlider = GameObject.Find("Slider").GetComponent<Slider>();
        mPressed = false;
        songLength = 69.0f;
        // isPiecePlaying = HorizontalPlane.getTempoController().getIsPiecePlaying();
	}
	
	// Update is called once per frame
	void Update () {
        if (mPressed == false)
        {
            if (Input.GetKeyDown("m") && progressBarSlider.value != 1)
            {
                //Debug.Log("Slider value: " + progressBarSlider.value.ToString());
                mPressed = true;
                timeStarter = Time.time;
                // float valueToAdd = Time.time / songLength;
               // progressBarSlider.value = progressBarSlider.value + valueToAdd;
                //Debug.Log("Slider value: " + progressBarSlider.value.ToString());

            }
        } else
        {
            if (progressBarSlider.value != 1)
            {
                Debug.Log("Slider value: " + progressBarSlider.value.ToString());
                valueToAdd = (Time.time - timeStarter) / songLength;
                progressBarSlider.value = valueToAdd;
                Debug.Log("Slider value: " + progressBarSlider.value.ToString());
            }
        }
		
    }
}
