using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers.Tiles.Items;

public class Tiles : MonoBehaviour
{
    public int ChildrenCount => transform.childCount;                 

    public float GetTotalLength()
    {
        float totalLength = 0;

        for (int i = 0; i < ChildrenCount; i++)
        {
            totalLength += transform.GetChild(0).GetComponent<Tile>().tileSize;
        }

        return totalLength;
    }
}
