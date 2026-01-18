using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TreeEditor;

public class CutsceneController : MonoBehaviour
{
    private int slideNumber = 1;
    [SerializeField] private TMP_Text cutsceneText; 
    public string[] dialogueLines = {"Damn it, I failed my final. Even though I pulled an all-nighter...",
    "Time to go to work...",
    "The job's pretty simple, at least. Take the customer's order.",
    "Grab the dough...",
    "Take the sauce and put it on the pizza...",
    "Grab the cheese and put it on the pizza...",
    "Sprinkle on whatever toppings the customer asked for...",
    "Then throw it in the oven, and once it's done, hand it over."};

    [SerializeField] private GameObject[] slides;
    [SerializeField] private GameObject IntroCutsceneCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void StartCutscene()
    {
        IntroCutsceneCanvas.SetActive(true);
        dialogueLines[10] = "The boss says I need to bring in " + DataController.quotaNumber.ToString() + " dollars today or else I'm fired...";
        
        if (CombatManager.Instance != null)
        {
            if (CombatManager.Instance.passQuota())
            {
                slideNumber = 10;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCutscenes()
    {
        if(slideNumber == 11)
        {
            SceneManager.LoadScene("Cooking Test");
        }
        slides[slideNumber].SetActive(true);
        cutsceneText.text = dialogueLines[slideNumber];
        slideNumber += 1;
        

    }
    public void ClickedStart()
    {
        if (CombatManager.Instance != null)
        {
            if (CombatManager.Instance.passQuota())
            {
                DataController.dayNumber += 1;
                DataController.quotaNumber = 40 + DataController.dayNumber * 10;
            }
        }

        StartCutscene();
    }

}
