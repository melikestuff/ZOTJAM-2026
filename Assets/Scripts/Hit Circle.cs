using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitCircle : MonoBehaviour
{

    [SerializeField]
    private bool isRed;
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
        RectTransform rect = GetComponent<RectTransform>();

        float startX = rect.anchoredPosition.x;
        float endX =
            qteScript.mediumRange.anchoredPosition.x +
            (qteScript.mediumRange.sizeDelta.x * 0.5f);

        float duration = qteScript.timeForQTE;
        float speed = (endX - startX) / duration;

        while (rect.anchoredPosition.x < endX)
           {
            Vector2 pos = rect.anchoredPosition;
            pos.x += speed * Time.deltaTime;
            rect.anchoredPosition = pos;

            // Input check
            /*
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("Input registered");
                yield break;
            }
            */
        yield return null;
        }

    // Out of range
    Destroy(gameObject);
    }


}
