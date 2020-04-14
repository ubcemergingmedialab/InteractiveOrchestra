using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles the behaviour of the music piece adjustable slide feature.
/// </summary>
public class SliderBehaviour : MonoBehaviour {

    private Slider slider;
    private Text textToShow;

    /// <summary>
    /// Grab slider component and subscribe slider activation to when piece is played 
    /// </summary>
    private void Awake()
    {
        slider = GetComponent<Slider>();
        textToShow = GetComponentInChildren<Text>();
        TempoController.PlayPiece += SliderActivated;
        TempoController.PieceInterrupt += SliderReset;
        TempoController.PieceStop += SliderReset;
    }

    /// <summary>
    /// Slider activated fires when piece is played.
    /// Subscribes tempo movement to tempo on update method
    /// </summary>
    /// <param name="dummyParam"> tempo controller delegate takes a float </param>
    void SliderActivated(float dummyParam)
    {
        TempoController.TempoOnUpdate += SliderMove;
    }

    /// <summary>
    /// Fires on update, increases the slider value
    /// </summary>
    public void SliderMove()
    {
        slider.value += (Time.deltaTime)/30;
        int timeInSong = (int)(slider.value * 30);
        string displayTimeString = timeInSong + "";
        if (timeInSong < 10)
        {
            displayTimeString = "0" + timeInSong;
        }
        string ScrollbarText = string.Format("00:{0}",timeInSong);
        textToShow.text = ScrollbarText;
    }

    /// <summary>
    /// Called when song ends, reset the value back to 0 
    /// </summary>
    /// <param name="dummyParam"></param>
    public void SliderReset(float dummyParam)
    {
        TempoController.TempoOnUpdate -= SliderMove;
        slider.value = 0;
    }

}
