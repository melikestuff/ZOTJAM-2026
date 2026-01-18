using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class QTEhandler : MonoBehaviour
{
    #region Allow For Easy Inspector editing

    // References to the Good and Bad ranges
    // And also store their widths for usage later
    public RectTransform mediumRange;
    public float mediumRangeWidth;

    public RectTransform goodRange;
    public float goodRangeWidth;

    private void OnValidate()
    {
        //I lowkey have no idea what this does i searched it up on the internet lol
        //But pretty much unity complains when we change the Rect.transform 
        // While were stil in editor, as it fires a signal to something else
        // And gives us a warning, (it still works but it was scary so i spent like
        // an hour looking it up).
        // TLDR THANK YOU INTERNET
    #if UNITY_EDITOR
        EditorApplication.delayCall += ApplySizes;
    #endif
    }

    private void ApplySizes()
    {
        if (!this) return; // object deleted or domain reload

        if (mediumRange)
            mediumRange.sizeDelta = new Vector2(mediumRangeWidth, mediumRange.sizeDelta.y);

        if (goodRange)
            goodRange.sizeDelta = new Vector2(goodRangeWidth, goodRange.sizeDelta.y);
    }
    #endregion
    
    #region Event Listener
    [SerializeField] 
    private CombatUIState visibleDuringState = CombatUIState.QTE;
    [SerializeField]
    private GameObject QTE_UI;
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
            QTE_UI.SetActive(true);
            startQTE(CombatManager.Instance.QuickTimePresses);
        }
        else{
            QTE_UI.SetActive(false);
        }
    }
    #endregion
    
    // For some reason, the parent wouldnt render the hit circles.
    //[SerializeField] 
    //private GameObject parentPoolObj;

    [SerializeField] 
    private List<float> hitCircles = new List<float>();

    // Since Hit circles spawn, the earlist one is First, 
    // So it can follow a FIFO
    // Pop the front and it automatically tells us which one is next
    [SerializeField]
    private Queue<GameObject> hitCircleQueue = new Queue<GameObject>();
    
    [SerializeField]
    private Transform circleSpawnPoint;

    [SerializeField]
    private GameObject hitCirclePrefab;

    // This is just so that we can see which one is left most in 
    // Inspector
    [SerializeField]
    private GameObject currHitCircle;

    public float timeForQTE;

    // This function is called when QTE happens
    // Called by Combat Manager after recieving the call from UI Combat Buttons
    public void startQTE(List<float> hitCirclesToSpawn){
        StartCoroutine(SpawnHitCircles(hitCirclesToSpawn));
    }

    // Since I can't run a update method here
    // Using a coroutine to run thru all elements in the given list
    private IEnumerator SpawnHitCircles(List<float> hitCirclesToSpawn)
    {
    float timeSoFar = 0f;
    int index = 0;

    while (index < hitCirclesToSpawn.Count)
    {
        timeSoFar += Time.deltaTime;

        if (timeSoFar >= hitCirclesToSpawn[index])
            {
            // Spawn a new hit circle and add it to the queue
            GameObject circle = Instantiate(hitCirclePrefab, circleSpawnPoint);
            circle.GetComponent<HitCircle>().getQTE_Reference(this);
            //circle.Transform.x = 0;

            Debug.Log("I SPAWNED A HIT CIRCLE");
            // Do some queue magic, so we know
            // Which one is always the left most one
            // Since they all move at the same speed

            hitCircleQueue.Enqueue(circle);
            currHitCircle = hitCircleQueue.Peek(); 
            //Debug.Log(goodRange.anchoredPosition.x);
            //Debug.Log(goodRangeWidth);
            
            circle.GetComponent<HitCircle>().spawnHitCircle();
            index++;
            }

        yield return null; // wait for next frame
        }
    }



}