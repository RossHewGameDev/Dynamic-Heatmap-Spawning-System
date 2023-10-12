using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlantSpawnLocation : MonoBehaviour
{
    [SerializeField] CellMapping cellMapping;
    [SerializeField] SpawningSystem spawningSystem; 
    [SerializeField] Interaction interactionSC;
    [SerializeField] float areaSize;
    public LayerMask spawnLayer;
    public LayerMask untraversableMask;
    [SerializeField] public List<GameObject> plantsInGroup = new List<GameObject>();

    Cell plantCell;


    private void Start()
    {
        Interaction.plantDelete += RemoveFromGroup;
    }

    private void OnDestroy()
    {
        Interaction.plantDelete -= RemoveFromGroup;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (plantsInGroup.Count == 0)
        {
            plantCell = cellMapping.CellFromWorldPoint(transform.position);
            spawningSystem.resourceCells.Remove(plantCell);

            StartCoroutine(AddCellsBackToSpawnable(plantCell));
            
        }
    }

    private void RemoveFromGroup()
    {

        if (plantsInGroup.Contains(interactionSC.lastPlantDestroyed))
        {
            plantsInGroup.Remove(interactionSC.lastPlantDestroyed); //remove from list

            //add back to list stuff here


            Destroy(interactionSC.lastPlantDestroyed); // destroy in game
        }
    }

    private IEnumerator AddCellsBackToSpawnable(Cell originCell) 
    {
        yield return new WaitForSeconds(25);
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

        cellMapping.localSpawnableCellCheck(cellMapping.FindNeighbours(originCell,6));


        Destroy(gameObject); // having it destroy at the end of the wait
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * areaSize);
    }
}
