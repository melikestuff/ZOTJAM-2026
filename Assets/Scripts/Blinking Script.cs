using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class BlinkingScript : MonoBehaviour
{
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
        if (newState == CombatUIState.notInDream)
        {
            Debug.Log("Starting Blink Routine");
            StartCoroutine(BlinkRoutine());
        }
        else
        {
            StopAllCoroutines();
            //Ensure eyes are open
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }
    [SerializeField] private Image fadeImage;

    [Header("Timings")]
    [SerializeField] private float fadeInTime = 3f;
    [SerializeField] private float maxFadeHold = 3f;
    [SerializeField] private float closeTime = .2f;
    [SerializeField] private float closedHold = 0.2f;
    [SerializeField] private float openTime = .2f;

    [Header("Alpha Levels")]
    [SerializeField] private float maxFadeAlpha = 0.6f;

    [SerializeField] private int chanceForNightmare = 30; //percentage chance 
    void Start()
    {
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while(CombatManager.Instance.CurrentUIState == CombatUIState.notInDream)
        {
            //Slowly darken
            yield return Fade(0f, maxFadeAlpha, fadeInTime);

            //Hold at max fade
            yield return new WaitForSeconds(maxFadeHold);

            //Fully close eyes
            yield return Fade(maxFadeAlpha, 1f, closeTime);

            //Closed (background logic time)
            yield return new WaitForSeconds(closedHold);

            rollForNightmare();
            //Open eyes
            yield return Fade(1f, 0f, openTime);
        }
    }
    
    public void rollForNightmare()
    {
        int roll = Random.Range(0, 100);
        if (roll < chanceForNightmare)
        {
            chanceForNightmare = -20; //reset chance
            CombatManager.Instance.SetUIState(CombatUIState.PlayerTurn);
        }
        else
        {
            chanceForNightmare += 20;
        }
    }
    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}
