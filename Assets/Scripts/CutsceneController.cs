using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    private int slideNumber = 0;
    [SerializeField] private TMP_Text cutsceneText; 
    public string[] dialogueLines = { "Damn it, I failed my final. Even though I pulled an all-nighter...",
    "Time to go to work...",
    "The job's pretty simple, at least. Take the customer's order.",
    "Grab the dough...",
    "Take the sauce and put it on the pizza...",
    "Grab the cheese and put it on the pizza...",
    "Sprinkle on whatever toppings the customer asked for...",
    "Then throw it in the oven, and once it's done, hand it over.",
    "The boss says I need to bring in" + DataController.quotaNumber.ToString() + "dollars today or else I'm fired..."};

    [SerializeField] private GameObject[] slides;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CombatManager.Instance != null)
        {
        }
        if (DataController.hasDied == true)
        {
            slideNumber = 8;
        }else
        {
            ShowCutscenes();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowCutscenes()
    {
        
        slides[slideNumber].SetActive(true);
        cutsceneText.text = dialogueLines[slideNumber];
        slideNumber += 1;
        if(slideNumber == 8)
        {
            SceneManager.LoadScene("Cooking Test");
        }

        

    }

}
