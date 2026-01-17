using UnityEngine;

public class UICombatButtons : MonoBehaviour
{
    public GameObject combatButtons;
    [SerializeField] private CombatUIState visibleDuringState = CombatUIState.PlayerTurn;

    #region Listener
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
            combatButtons.SetActive(true);
        }
    }
    #endregion

    public void onAttackPress(){
        CombatManager.Instance.doDamage(5, true);
    }

    public void onBlockPress(){
        CombatManager.Instance.doDamage(3, true);
    }

}
