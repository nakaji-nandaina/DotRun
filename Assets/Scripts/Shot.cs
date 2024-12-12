using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Shot", menuName = "Shot/Shot", order = 1)]
public class Shot : ScriptableObject
{
    public float shotSpeed = 0.5f;
    public float shotInterval= 1f;
    public float shotTimer = 0f;
    public int shotDamage = 10;
    public GameObject shotPrefab;
}
