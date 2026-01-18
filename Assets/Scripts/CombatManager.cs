using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private float playerHP = 50;
    public QTE_Data QuickTimePresses;
    public int dmg = 5;
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

    // Public method so that outside scripts can send a signal
    // Changing all scripts attached to this bus
    public void SetUIState(CombatUIState newState)
    {
        if (currentUIState == newState)
            return;

        currentUIState = newState;
        Debug.Log($"Combat UI State changed to: {newState}");

        OnUIStateChanged?.Invoke(newState);
    }
    #endregion

    // Temp combat logic & functions
    // As of now, "Combat UI" uses this to send information back to this script
    public void doDamage(float dmg, bool isPlayer){
        if(isPlayer){
            playerHP-=dmg;
            Debug.Log("damage: "+ dmg + ",isPlayer: "+ isPlayer + "player took damage just now");
        }
        else{
            enemyHP-=dmg;
            Debug.Log("damage: "+ dmg + ",isPlayer: "+ isPlayer + "enemy took damage just now");
        }
    }

    /*
    // Public method to know when to start the QTE
    public void startQTE(List<float> qtEvents){
        Debug.Log("QTE STARTED");
    }
    */

}
