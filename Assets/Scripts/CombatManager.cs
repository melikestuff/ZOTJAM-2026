using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

// Set up enum so that we know what state we are in during the shift
// notInDream - disable every combat related UI
// PlayerTurn - enable the buttons showcasing what actions the player can take
// QTE - enable the Quick time event UI
public enum CombatUIState
{
    notInDream, // Disable all Combat UI
    PlayerTurn, // Also means were in dream world
    QTE         // TIME TO PLAY OSU
}

public class CombatManager : MonoBehaviour
{
    #region Variables
    // Declare variables
    //
    // Handle any relevant combat data
    [SerializeField] private float enemyHP = 50;
    [SerializeField] private float Money = 0;
    [SerializeField] private float QuotaRequired = 100;
    public QTE_Data QuickTimePresses;

    public TextMeshProUGUI quota;
    public TextMeshProUGUI moneySoFar;

    public bool isBlocking;
    //public int dmg = 5;
    #endregion
    

    #region Singleton Setup   
    
    // Set up singleton reference
    // Call this script from outside scripts by doing CombatHandler.Instance.Function();
    private static CombatManager _instance;
    public static CombatManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("Another instance was detected!");
            Destroy(this.gameObject);
        } else {
            //DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }
    #endregion

    #region UI state Event bus
    // UI State event bus
    // After finishing this.. It was kindaaa overkill
    // As i coulda just hard-coded a reference to this script
    public static event Action<CombatUIState> OnUIStateChanged;

    [SerializeField] private CombatUIState currentUIState = CombatUIState.notInDream;

    public CombatUIState CurrentUIState => currentUIState;

    private void Start()
    {
        quota.text = QuotaRequired.ToString();
        moneySoFar.text = Money.ToString();

    }
    // Public method so that outside scripts can send a signal
    // Changing all scripts attached to this bus
    public void SetUIState(CombatUIState newState)
    {
        if (currentUIState == newState)
            return;

        currentUIState = newState;
        Debug.Log($"Combat UI State changed to: {newState}");
        if(currentUIState == CombatUIState.notInDream)
        {
            Debug.Log("Exiting Dream World, resetting enemy HP");
            enemyHP = 50; // Reset enemy HP when exiting dream world
        }
        OnUIStateChanged?.Invoke(newState);
    }
    #endregion

    //Due to player not inputting anything at all
    //Need a way of detection when a hit circle goes pass range
    // And gets deleted
    public void doDmgToEnemy(float dmg)
    {
        if (isBlocking)
        {
            return;
        }
        enemyHP -= dmg;
        Debug.Log("damage: "+ dmg + "enemy took damage just now");
        if (enemyHP <= 0)
        {
            Debug.Log("Enemy Defeated!");
            // Exit combat UI
            SetUIState(CombatUIState.notInDream);
        }
    }

    public void loseMoney(float amt)
    {
        moneySoFar.text = Money.ToString("#.00");
        // Check if were blocking
        // Variable is set true when clicking on block button in
        // UI Combat Buttons cs
        if (isBlocking){
            Debug.Log("Blocked Money Loss!");
            return;
        }

        // Got lazy to encourage skill, so players just lose
        // A random amt of money.
        Money -= amt;
        if (Money < 0)
        {
            Money = 0;
        }
        Debug.Log("Lost Money: " + amt);
    }
}
