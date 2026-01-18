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

    //[SerializeField] 
    //private List<float> hitCircles = new List<float>();

    // Since Hit circles spawn, the earlist one is First, 
    // So it can follow a FIFO
    // Pop the front and it automatically tells us which one is next
    public Queue<GameObject> hitCircleQueue = new Queue<GameObject>();
    
    [SerializeField]
    private Transform circleSpawnPoint;

    [SerializeField]
    private GameObject hitCirclePrefab;

    // This is just so that we can see which one is left most in 
    // Inspector
    public GameObject currHitCircle;

    public float timeForQTE;

    // This function is called when QTE happens
    // Called by Combat Manager after recieving the call from UI Combat Buttons
    public void startQTE(QTE_Data data)
    {
        StartCoroutine(SpawnHitCircles(data.hitCircles));
    }

    // Since I can't run a update method here
    // Using a coroutine to run thru all elements in the given list
    private IEnumerator SpawnHitCircles(List<HitCircleData> hitCirclesToSpawn)
    {
    float timeSoFar = 0f;
    int index = 0;

    while (index < hitCirclesToSpawn.Count)
    {
        timeSoFar += Time.deltaTime;

        if (timeSoFar >= hitCirclesToSpawn[index].spawnTime)
            {
            // Spawn a new hit circle and add it to the queue
            GameObject circle = Instantiate(hitCirclePrefab, circleSpawnPoint);
            circle.GetComponent<HitCircle>().getQTE_Reference(this);
            circle.GetComponent<HitCircle>().isShadow = hitCirclesToSpawn[index].color; //== HitCircleColor.Blue;


            //circle.Transform.x = 0;

            //Debug.Log("I SPAWNED A HIT CIRCLE");
            // Do some queue magic, so we know
            // Which one is always the right most one
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

    // I gave up tryna make it look nice lol
    // Now every frame check if Z and X were pressed
    // Then check if theres an active hit circle
    // If theres nothing, do nothing
    // If theres a hit circle, do some logic on its position and
    // Calculate score for that
    public void Update()
    {
        if (hitCircleQueue.Count > 0 && hitCircleQueue.Peek() != null)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
            {
                HitCircle theCircle = currHitCircle.GetComponent<HitCircle>();
                HitCircleColor color = theCircle.isShadow;
                float posX = theCircle.destroyThisHitCircle();

                //Check if color is even correct
                if (color == HitCircleColor.normal)
                {
                    //Check if hit circle is in the goodRange
                    //Check if its in green range 
                    //Debug.Log("Goodrange is " + (goodRange.anchoredPosition.x - 50) + " >< " + goodRange.anchoredPosition.x + 50);
                    //Debug.Log(posX);
                    if(posX > goodRange.anchoredPosition.x - goodRangeWidth/2 && posX < goodRange.anchoredPosition.x + goodRangeWidth/2)
                    {
                        Debug.Log("Good!");
                    }
                    // Check if its in yellow now
                    else if(posX > mediumRange.anchoredPosition.x - mediumRangeWidth/2 && posX < mediumRange.anchoredPosition.x + mediumRangeWidth/2)
                    {
                        Debug.Log("Medium...");
                    }
                    //If hit circle is in niether, then assume player missed timing of 
                    // The hit circle and it is outside the range
                    else
                    {
                        Debug.Log("BAD BAD BAD BAD");
                    }
                }
            }
        }
        
    }
/*
// Check if were First in queue
            // AKa right-most circle
            //Debug.Log(qteScript.hitCircleQueue.Peek() == this);
            if(qteScript.hitCircleQueue.Peek() == gameObject)
                {
                // Input check
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
                    {
                        Debug.Log("Input registered");
                        qteScript.hitCircleQueue.Dequeue();
                        if (qteScript.hitCircleQueue.Peek() != null)
                            {
                            qteScript.currHitCircle = qteScript.hitCircleQueue.Peek();
                            }
                    yield break;
                    }
                }
*/
}