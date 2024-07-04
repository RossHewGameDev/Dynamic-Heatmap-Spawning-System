using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// This script is attached to the plant spawn location prefab
// It is used to keep track of the plants in a group and add cells back to the spawnable list after a plant is destroyed
// This was written hastily at the end of the project and could be improved upon massively...
public class PlantSpawnLocation : MonoBehaviour
{
    // Add cells back to spawnable list every 25 seconds
    private const int addCellsBackTimer = 25;
    [SerializeField] CellMapping cellMapping;
    [SerializeField] SpawningSystem spawningSystem; 
    [SerializeField] Interaction interactionSC;
    [SerializeField] float areaSize;
    public LayerMask spawnLayer;
    public LayerMask untraversableMask;
    [SerializeField] public List<GameObject> plantsInGroup = new List<GameObject>();

    Cell plantCell;

    // Delegate for when a plant is removed from the group
    public delegate void PlantGroupRemoved(List<Cell> originCell);
    public static PlantGroupRemoved plantGroupRemoved;


    private void Start()
    {
        Interaction.plantDelete += RemoveFromGroup;
    }

    private void OnDestroy()
    {
        Interaction.plantDelete -= RemoveFromGroup;
    }

    void Update()
    {
        // Check if there are no plants in the group and add cells back to the spawnable list
        if (plantsInGroup.Count == 0)
        {
            plantCell = cellMapping.CellFromWorldPoint(transform.position);
            spawningSystem.resourceCells.Remove(plantCell);

            StartCoroutine(AddCellsBackToSpawnable(plantCell));
            
        }
    }

    /// <summary>
    /// // Remove the plant from the group and destroy it
    /// </summary>
    private void RemoveFromGroup()
    {
        if (plantsInGroup.Contains(interactionSC.lastPlantDestroyed))
        {
            plantsInGroup.Remove(interactionSC.lastPlantDestroyed); //remove from list

            //add back to list stuff here

            Destroy(interactionSC.lastPlantDestroyed); // destroy in game
        }
    }

    /// <summary>
    ///  Add cells back to the spawnable list after a plant is destroyed
    /// </summary>
    private IEnumerator AddCellsBackToSpawnable(Cell originCell) 
    {
        yield return new WaitForSeconds(addCellsBackTimer);

        // Check if the cell is traversable and if it is in the spawn layer
        foreach (Cell cell in cellMapping.FindNeighbours(originCell, 6))
        {
            if (cell.traversable && Physics.CheckSphere(cell.worldPosition, 1, spawnLayer))
            {
                cell.spawnable = !Physics.CheckSphere(cell.worldPosition, 5, untraversableMask);
                cellMapping.spawnableCellList.Add(cell);
            }
            else
            {
                cell.spawnable = false;
            }
        }
        originCell.spawnable = true;
        cellMapping.spawnableCellList.Add(originCell);

        spawningSystem.plantsOnEdge.Remove(originCell);

        // cellMapping.localSpawnableCellCheck(cellMapping.FindNeighbours(originCell,6)); // why isnt this an event? make it an observer pattern!

        plantGroupRemoved(cellMapping.FindNeighbours(originCell,6)); // invoke event to inform the spawning system that the plant group has been removed and local cells need updating

        Destroy(gameObject); // having it destroy at the end of the wait
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * areaSize);
    }
}
