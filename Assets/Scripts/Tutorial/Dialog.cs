using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
This Script manages the dialog text as well as any event triggers within the tutorial. 
*/
public class Dialog : MonoBehaviour
{
    public GameObject canvas;
    public Text textDisplay;
    private List<DialogSequence> sentences;
    public GameObject controllers;
    public ControllerAnimations animations;
    public GameObject traceManager;

    private int index = 0;
    private bool finishedSentence = false;
    public bool batonIsGrabbed = false;
    float triggerPressed = 0;


    void Start()
    {
        traceManager.SetActive(false);
        sentences = new List<DialogSequence>
        {
            new DialogSequence("Welcome to Interactive Orchestra, a Virtual Conducting Experience.", "text"),
            new DialogSequence("This tutorial will familarize you with the controls and features available for the application.", "text"),
            new DialogSequence("Let's get started!", "text"),
            new DialogSequence("Here are the visual representations of the controllers.", "controller1"),
            new DialogSequence("The grip button can be found resting under your middle finger, and is used to pick up and put down the baton.", "controller2"),
            new DialogSequence("Try moving your right controller close to the baton, and tap the grip button to pick it up!", "gripAction"),
            new DialogSequence("Well done!", "text"),
            new DialogSequence("Now let's try conducting!", "text"),
            new DialogSequence("To conduct, hold down the trigger while waving the baton. Give it a try!", "trigger"),
            new DialogSequence("Great job! You should always hold down the trigger in order to keep conducting.", "text"),
            new DialogSequence("Let's try cueing the orchestra to start playing the piece.", "text"),
            new DialogSequence("While holding the trigger, trace the gesture in front of you.", "prep"),
            new DialogSequence("Great job! Now keeping the trigger held down, continue to trace the gestures in front of you!.", "text"),
            new DialogSequence("Notice how the faster or slower your pace of conducting is, the orchestra will react to match your speed.", "text"),
            new DialogSequence("Keep conducting until the end of the song!", "end"),
            new DialogSequence("Congratulations, you have just successfully finished your first song!", "text"),
            new DialogSequence("Feel free to continue to practice conducting, and explore the other features of Interactive Orchestra!", "text"),
        };
        StartCoroutine(Type());
    }

    void Update()
    {
        triggerPressed = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        if (index < sentences.Count)
        {
            NextSentence(sentences[index]);
        }
    }

    /// <summary>
    /// Activates the tutorial canvas and displays the next dialog.
    /// </summary>
    IEnumerator Type()
    {
        if (!canvas.activeSelf)
        {
            canvas.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        textDisplay.text = sentences[index].sentence;
        yield return new WaitForSeconds(3);
        finishedSentence = true;
    }

    /// <summary>
    /// Manages event triggers for different types of dialog.
    /// </summary>
    public void NextSentence(DialogSequence sequence)
    {
        if (finishedSentence && sequence.trigger == "text")
        {
            NextSentenceHelper();
        }
        else if (sequence.trigger == "controller1")
        {
            ActivateControllers(true);
            if (finishedSentence)
            {
                NextSentenceHelper();
            }
        }
        else if (sequence.trigger == "controller2")
        {
            animations.ShowGripButton();
            if (finishedSentence)
            {
                ActivateControllers(false);
                NextSentenceHelper();
            }
        }
        else if (sequence.trigger == "gripAction")
        {
            if (GetBatonIsGrabbed())
            {
                NextSentenceHelper();
            }
        }
        else if (sequence.trigger == "trigger")
        {
            ActivateControllers(true);
            animations.ShowTriggerButton();
            ActivateControllers(false);
            if (GetBatonIsGrabbed() && triggerPressed > 0.8f)
            {
                NextSentenceHelper();
            }
        }
        // need to figure out logic here
        else if (sequence.trigger == "prep")
        {
            traceManager.SetActive(true);
            if (traceManager.GetComponent<TraceManager>().GetPrepIsDone())
            {
                NextSentenceHelper();
            }
        }
        else if (sequence.trigger == "end")
        {
            if (traceManager.GetComponent<TraceManager>().GetSongIsDone())
            {
                NextSentenceHelper();
            }
        }
    }

    /// <summary>
    /// Sets tutorial controllers active in scene.
    /// </summary>
    public void ActivateControllers(bool b)
    {
        controllers.SetActive(b);
    }

    /// <summary>
    /// Ensures we have not reached end of tutorial dialogs before calling next dialog.
    /// </summary>
    public void NextSentenceHelper()
    {
        index++;
        textDisplay.text = "";
        finishedSentence = false;
        Debug.Log("index is: " + index);
        Debug.Log("sentences is: " + this.sentences.Count);
        if (index >= this.sentences.Count)
        {
            canvas.SetActive(false);
        }
        else
        {
            StartCoroutine(Type());
        }
    }

    /// <summary>
    /// Gets batonIsGrabbed.
    /// </summary>
    public void BatonIsGrabbed()
    {
        batonIsGrabbed = true;
    }

    /// <summary>
    /// Sets batonIsGrabbed.
    /// </summary>
    public bool GetBatonIsGrabbed()
    {
        return this.batonIsGrabbed;
    }

    /// <summary>
    /// structure that a DialogSequence consists of.
    /// </summary>
    public struct DialogSequence
    {
        public string sentence;
        public string trigger;

        /// <summary>
        /// Constructor for a DialogSequence. 
        /// </summary>
        public DialogSequence(string sentence, string trigger)
        {
            this.sentence = sentence;
            this.trigger = trigger;
        }
    }
}
