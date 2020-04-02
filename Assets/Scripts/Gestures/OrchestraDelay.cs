using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Script implements Singleton Design Pattern as a single instance for the Orchestral delay feature.
/// </summary>
public class OrchestraDelay : MonoBehaviour {

    private static OrchestraDelay instance;
    private float[] possibleOrchTimeDelays;
    private int maxIndex = 20;
    private int currentIndex = 0;
    private TextMesh displaytext;

    public static OrchestraDelay Instance {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        possibleOrchTimeDelays = new float[21];
        possibleOrchTimeDelays[0] = 0;
        possibleOrchTimeDelays[1] = 50;
        possibleOrchTimeDelays[2] = 100;
        possibleOrchTimeDelays[3] = 150;
        possibleOrchTimeDelays[4] = 200;
        possibleOrchTimeDelays[5] = 250;
        possibleOrchTimeDelays[6] = 300;
        possibleOrchTimeDelays[7] = 350;
        possibleOrchTimeDelays[8] = 400;
        possibleOrchTimeDelays[9] = 450;
        possibleOrchTimeDelays[10] = 500;
        possibleOrchTimeDelays[11] = 550;
        possibleOrchTimeDelays[12] = 600;
        possibleOrchTimeDelays[13] = 650;
        possibleOrchTimeDelays[14] = 700;
        possibleOrchTimeDelays[15] = 750;
        possibleOrchTimeDelays[16] = 800;
        possibleOrchTimeDelays[17] = 850;
        possibleOrchTimeDelays[18] = 900;
        possibleOrchTimeDelays[19] = 950;
        possibleOrchTimeDelays[20] = 1000;
        displaytext = GetComponent<TextMesh>();
        UpdateText();

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Update display text on orchdelay component for delay time
    /// </summary>
    private void UpdateText()
    {
        string displayText = string.Format("{0} ms", possibleOrchTimeDelays[currentIndex]);
        displaytext.text = displayText;
    }

    /// <summary>
    /// Increase index of time delay to refer to 
    /// </summary>
    public void IncreaseIndex()
    {
        if(currentIndex <= (maxIndex - 1))
        {
            currentIndex++;
            UpdateText();
        }
        UpdateText();
    }

    /// <summary>
    /// Decrease index of time delay to refer to 
    /// </summary>
    public void DecreaseIndex()
    {
        if (currentIndex >= 1)
        {
            currentIndex--;
            UpdateText();
        }
    }

    /// <summary>
    /// Return current delay time chosen 
    /// </summary>
    /// <returns></returns>
    public float GetCurrentOrchDelay()
    {
        return possibleOrchTimeDelays[currentIndex];
    }
}
