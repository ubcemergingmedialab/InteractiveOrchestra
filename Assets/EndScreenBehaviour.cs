using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenBehaviour : MonoBehaviour
{
    #region Variables
    [SerializeField] private Sprite victoryScreen;
    [SerializeField] private Sprite failScreen;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject GestureRelatedObjects;
    #endregion

    private void Awake()
    {
        TempoController.PieceStop += TriggerEndMenu;
       // GetComponent<Renderer>().enabled = false;
    }

    public void TriggerEndMenu(float dummyParam)
    {

       // GetComponent<Renderer>().enabled = true;
        animator.SetBool("ExperienceEnded",true);
        GestureRelatedObjects.SetActive(false);
    }

    public void TurnOffMenu()
    {
       // GetComponent<Renderer>().enabled = false;
        animator.SetBool("ExperienceEnded", false);
    }
}

