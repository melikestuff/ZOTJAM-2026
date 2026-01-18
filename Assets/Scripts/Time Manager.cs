using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float totalTime = 300f; // Total time in seconds
    public float timeRemaining; //remaining in seconds

    [SerializeField] private GameObject screenToDisplay;
    public void Start()
    {
        timeRemaining = totalTime;
    }

    public void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            
            screenToDisplay.SetActive(true);
        }
    }
}

