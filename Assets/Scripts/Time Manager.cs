using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public float totalTime = 300f; // Total time in seconds
    public float timeRemaining; //remaining in seconds

    [SerializeField] private List<string> winningMsgs;
    [SerializeField] private List<string> losingMsgs;

    [SerializeField] private TextMeshProUGUI msgToDisplay;
    [SerializeField] private TextMeshProUGUI quotaStatusDisplay;

    [SerializeField] private GameObject screenToDisplay;

    [SerializeField] private TextMeshProUGUI timerDisplay;
    public void Start()
    {

        timeRemaining = totalTime;
    }

    public void Update()
    {
        if (timeRemaining > 0)
        {
            // Update timer display
            timerDisplay.text = "Time Remaining: " + Mathf.CeilToInt(timeRemaining).ToString() + "s";
            timeRemaining -= Time.deltaTime;
        }
        else if (screenToDisplay.activeSelf == false)
        {
            timeRemaining = 0;
            // Time's up, check quota and display appropriate message
            // This if statemente means that we passed the quota
            if (CombatManager.Instance.passQuota())
            {
                msgToDisplay.text = winningMsgs[UnityEngine.Random.Range(0, winningMsgs.Count)];
                quotaStatusDisplay.text = "Quota Met!";
            }
            else
            {
                msgToDisplay.text = losingMsgs[UnityEngine.Random.Range(0, losingMsgs.Count)];
                quotaStatusDisplay.text = "Quota Not Met!";
            }
            screenToDisplay.SetActive(true);
        }
    }

    public void startMenuButton()
    {
        SceneManager.LoadScene("Title Screen");
        Debug.Log("Start Menu Button Pressed");
    }
}

