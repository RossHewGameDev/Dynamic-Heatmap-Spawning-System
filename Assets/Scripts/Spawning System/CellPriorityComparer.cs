using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPriorityComparer : IComparer<Cell>
{
    public int Compare(Cell x, Cell y)
    {
        return x.spawnPriority.CompareTo(y.spawnPriority);
    }
}
