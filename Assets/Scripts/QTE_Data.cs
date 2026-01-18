using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QTE_Data", menuName = "Scriptable Objects/QTE_Data")]
//old name was HitCircleData
public class QTE_Data : ScriptableObject
{
    public List<HitCircleData> hitCircles;
}

[System.Serializable]
public struct HitCircleData
{
    public float spawnTime;
    public HitCircleColor color;
}

public enum HitCircleColor
{
    normal,
    shadow
}

/*
using UnityEngine;
// Use the CreateAssetMenu attribute to allow creating instances of this ScriptableObject from the Unity Editor.
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class SpawnManagerScriptableObject : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate;
    public Vector3[] spawnPoints;
}
*/