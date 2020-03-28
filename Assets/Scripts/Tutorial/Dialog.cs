using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{

    public Text textDisplay;
    private List<DialogSequence> sentences;
    public float typingSpeed;
    public GameObject controllers;
    public ControllerAnimations animations;

    private int index = 0;
    private bool finishedSentence = false;

    void Start()
    {
        sentences = new List<DialogSequence>
        {
            new DialogSequence("Welcome to Interactive Orchestra, a Virtual Conducting Experience.", "text"),
            new DialogSequence("This tutorial will familarize you with the controls and features available for the application.", "text"),
            new DialogSequence("Let's get started!", "text"),
            new DialogSequence("Here are the visual representations of the controllers.", "controller1"),
            new DialogSequence("The grip button can be found resting under your middle finger, and is used to pick up and put down the baton.", "controller2"),
            new DialogSequence("Try moving your right controller close to the baton, and tap the grip button to pick it up!", "text"),
        };
        StartCoroutine(Type());
    }

    void Update()
    {
        NextSentence(sentences[index]);
    }

    IEnumerator Type()
    {
        yield return new WaitForSeconds(0.6f);
        textDisplay.text = sentences[index].sentence;       
        yield return new WaitForSeconds(3);
        finishedSentence = true;
        Debug.Log(finishedSentence);
    }

    public void NextSentence(DialogSequence sequence)
    {
        if (finishedSentence && sequence.trigger == "text")
        {
            NextSentenceHelper();
        }
        else if (sequence.trigger == "controller1")
        {
            ActivateControllers(true);
            if (finishedSentence) {
                NextSentenceHelper();
            }
        }
        else if (sequence.trigger == "controller2")
        {
            animations.ShowGripButton();
            if (finishedSentence) {
                ActivateControllers(false);
                NextSentenceHelper();
            }
            
        }
    }

    public void ActivateControllers(bool b)
    {
        controllers.SetActive(b);
    }

    public void NextSentenceHelper()
    {
        index++;
        textDisplay.text = "";
        finishedSentence = false;
        StartCoroutine(Type());
    }

    public struct DialogSequence
    {
        public string sentence;
        public string trigger;
       
        public DialogSequence(string sentence, string trigger)
        {
            this.sentence = sentence;
            this.trigger = trigger;
        }
    }
}
