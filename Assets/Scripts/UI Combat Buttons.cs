using UnityEngine;

public class UICombatButtons : MonoBehaviour
{
    public GameObject combatButtons;
    [SerializeField] 
    private CombatUIState visibleDuringState = CombatUIState.PlayerTurn;

    #region Event Listener
    private void OnEnable()
    {
        CombatManager.OnUIStateChanged += HandleUIStateChanged;
    }

    private void OnDisable()
    {
        CombatManager.OnUIStateChanged -= HandleUIStateChanged;
    }

    private void HandleUIStateChanged(CombatUIState newState)
    {
        if(newState == visibleDuringState){
            combatButtons.SetActive(true);
        }
        else{
            combatButtons.SetActive(false);
        }
    }
    #endregion

    public void onAttackPress(){
        Debug.Log("Attack Button Pressed");
        CombatManager.Instance.isBlocking = false;
        CombatManager.Instance.SetUIState(CombatUIState.QTE);
    }

    public void onBlockPress(){
        CombatManager.Instance.isBlocking = true;
        CombatManager.Instance.SetUIState(CombatUIState.QTE);
    }

}
