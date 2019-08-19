using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TrialDisplayBehaviour : MonoBehaviour {

    private Text textComponent;

    void Start () {
        textComponent = GetComponent<Text>();
    }
    /// <summary>
    /// Change display text of the current trial that is being recorded
    /// </summary>
    /// <param name="trialNumber"> Current trial number </param>
    /// <param name="BPM"<> Current BPM being recorded </param>
    /// <param name="size"> Current gesture size being measure </param>
    public void changeTrial(int trialNumber,string BPM, string size)
    {
        textComponent.text = "Trial: " + (trialNumber - 1) + " of " +"20" + "\n" + "BPM: " + BPM + " Size: " + size ;
    }

    /// <summary>
    /// Display recording text
    /// </summary>
    public void displayRecordScreen()
    {
        textComponent.text = "Data has been recorded!"+"\n" + "Thank you Jaelem";

    }

    public void updateValuesWithConductorSample(OVRVelocityTracker.ConductorSample sample)
    {
        textComponent.text = "Trial:" + sample.trial + "\n" +
        "ID:" + sample.id + "\n" +
        "VelocityVector:" + sample.velocityVector+"\n" +
        "Position:" + sample.position +"\n" +
        "VelocityMagnitude:" + sample.velocityMagnitude + "\n" +
        "TimeRelativeToPrep:" + sample.timeRelativeToPrep +"\n" +
        "AngleToBP1:" + sample.angleToBP1 + "\n" +
        "Acceleration:" + sample.acceleration +"\n" +
        "DistanceCoveredSoFar:" + sample.distanceCoveredSoFar +"\n" +
        "GestureSize:" + sample.gestureSize;

    }


}
