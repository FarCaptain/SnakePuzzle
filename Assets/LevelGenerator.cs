using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] ItemSpawner itemSpawner;

    private void Start()
    {
        //set boundaries
        itemSpawner.SpawnAbyss(new Vector2Int(0, 0), new Vector2Int(1, 26), new Vector2Int(0,0));

        itemSpawner.SpawnWater(new Vector2Int(10, 0), new Vector2Int(13, 26));
    }
}
