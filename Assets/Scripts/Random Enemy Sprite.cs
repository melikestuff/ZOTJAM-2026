using System.Collections.Generic;
using UnityEngine;

public class RandomEnemySprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> sprites;

    #region Event Listener
    [SerializeField] 
    private CombatUIState visibleExceptState = CombatUIState.notInDream;
    [SerializeField]
    private GameObject QTE_UI;

    [SerializeField] private GameObject interactables;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject nightmareScene;

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
        SetRandomSprite();
        if (newState != visibleExceptState)
        {
            QTE_UI.SetActive(true);
            interactables.SetActive(false);
            background.SetActive(false);
            nightmareScene.SetActive(true);
        }
        else{
            QTE_UI.SetActive(false);
            interactables.SetActive(true);
            background.SetActive(true);
            nightmareScene.SetActive(false);
        }
    }
    #endregion
    void Start()
    {
        SetRandomSprite();
    }

    void SetRandomSprite()
    {
        if (sprites == null || sprites.Count == 0)
            return;

        int index = Random.Range(0, sprites.Count); // max is exclusive
        spriteRenderer.sprite = sprites[index];
    }

}
