using UnityEngine;
using System.Collections;

public class HitCircle : MonoBehaviour
{

    [SerializeField]
    private bool isRed;
    Coroutine qteCoroutine;

    //Called by the QTE handler script, 
    // Move to spawn position
    // Spawns in the circle (setActive = true)
    // Calls Coroutine to move until pass yellow width

    public void spawnHitCircle()
    {
        qteCoroutine = StartCoroutine(startMoving());
    }

    public void stopMoving()
    {
        if (qteCoroutine != null)
        {
            StopCoroutine(qteCoroutine);     
        }
       
    }
    private IEnumerator startMoving()
    {
        return null;
    }
}
