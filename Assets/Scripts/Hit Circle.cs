using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HitCircle : MonoBehaviour
{

    public HitCircleColor isShadow;
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite newSprite;

    Coroutine qteCoroutine;

    QTEhandler qteScript;

    //Called by the QTE handler script, 
    // Move to spawn position
    // Spawns in the circle (setActive = true)
    // Calls Coroutine to move until pass yellow width

    public void getQTE_Reference(QTEhandler theScript)
    {
        qteScript = theScript;
    }
    public void spawnHitCircle()
    {
        //Debug.Log("SPAWNING");
        qteCoroutine = StartCoroutine(StartMoving());
    }

    public void stopMoving()
    {
        if (qteCoroutine != null)
        {
            StopCoroutine(qteCoroutine);
        }
       
    }
    private IEnumerator StartMoving()
    {
        if (isShadow == HitCircleColor.shadow)
        {
            targetImage.sprite = newSprite;
        }
        //Find how whats our speed and move the hit circle every frame
        // Depending on that speed.
        RectTransform rect = GetComponent<RectTransform>();

        float startX = rect.anchoredPosition.x;
        float endX =
            qteScript.mediumRange.anchoredPosition.x +
            (qteScript.mediumRange.sizeDelta.x * 0.5f) 
            + 100;

        float duration = qteScript.timeForQTE;
        float speed = (endX - startX) / duration;

        while (rect.anchoredPosition.x < endX)
           {
            //Debug.Log("HELLO START MOVING?");
            Vector2 pos = rect.anchoredPosition;
            pos.x += speed * Time.deltaTime;
            rect.anchoredPosition = pos;

            yield return null;
            }
        //If we exit the loop, it means we passed the range
        destroyThisHitCircle();
    }

    // Remove the circle and send info back
    // To QTE handler
    public float destroyThisHitCircle()
    {
        stopMoving();
        //Debug.Log(GetComponent<RectTransform>().anchoredPosition.x);
        float numToReturn = GetComponent<RectTransform>().anchoredPosition.x;
        qteScript.hitCircleQueue.Dequeue();
        if (qteScript.hitCircleQueue.Count > 0)
        {
            qteScript.currHitCircle = qteScript.hitCircleQueue.Peek();
        }
        Destroy(gameObject);

        /*
         *if (!isSpawningQTE && hitCircleQueue.Count == 0)
            {
                // QTE is over
                CombatManager.Instance.doDmgToEnemy(totalDmg * 1.5f);
                CombatManager.Instance.loseMoney(Random.Range(0f,2f));
                totalDmg = 0;
                if(CombatManager.Instance.CurrentUIState != CombatUIState.notInDream)
                {
                    CombatManager.Instance.SetUIState(CombatUIState.PlayerTurn);
                }
            }
         */
        // Check if we are done with the QTE here becuz it can also get auto deleted
        // From passing the yellow zone.
        if (!qteScript.isSpawningQTE && qteScript.hitCircleQueue.Count == 0)
        {
            CombatManager.Instance.doDmgToEnemy(qteScript.totalDmg * 1.5f);
            CombatManager.Instance.loseMoney(Random.Range(0f, 2f));
            qteScript.totalDmg = 0;
            if (CombatManager.Instance.CurrentUIState != CombatUIState.notInDream)
            {
                CombatManager.Instance.SetUIState(CombatUIState.PlayerTurn);
            }
        }
        return numToReturn;
    }

}
