using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitCircle : MonoBehaviour
{

    [SerializeField]
    public HitCircleColor isShadow;
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
        return numToReturn;
    }

}
