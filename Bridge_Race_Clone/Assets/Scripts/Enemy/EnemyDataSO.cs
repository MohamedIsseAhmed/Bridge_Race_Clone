using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    public Transform[] targetPaths;
    public int RandomNumberForStoppinCollcting;
    public int maxRandomNumber;
    public int minRandomNumber;
    public float rayDistance=2;
    public float rayOffsetDistance = 0.1f;
    public float distanceToCube = 1f;
    public LayerMask groundLayer;
}
