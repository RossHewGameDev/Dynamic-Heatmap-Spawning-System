using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///Spawning System controls the spawning of  resources in the scene over time.
/// 
/// 
/// </summary>
public class SpawningSystem : MonoBehaviour
{

    public int sizeOfEdgeList; // reports size of the list: Helps monitor performance of the script.
    public int sizeOfList;    // reports size of the list: Helps monitor performance of the script.

    public enum SystemTypeRunning
    {
        Static,   // enables Static spawning system
        Random,  // enables Random spawning system
        Heatmap // enables Dynamic spawning system
    }

    [SerializeField] public int minExplorationBeforeSpawning = 100; // minimum exploration required of map before spawning resources

    [SerializeField] public CellMapping cellMapping;

    [SerializeField] public int resourceNumber; // Spawning systems target resource amount
    [SerializeField] public int resourceGap;   // Spawning systems distance that it tries to spawn plants at

    [SerializeField] public int cellUpdateRange; // The Range of cells that get updated by the spawning system in a frame
    [SerializeField] public GameObject plantPrefab;

    public SystemTypeRunning systemTypeRunning;

    public List<Cell> resourceCells = new List<Cell>();

    Cell spawn;
    float cellDiameter;
    bool tooClose;
    bool spawningFinished;
    int succesfullySpawned = 0;
    public float waitTimeDelay = 0f;

    bool spawnBounce = true;

    public List<Transform> staticPlants = new List<Transform>();


    private List<Cell> cellsInExploredVision = new List<Cell>();       // cells within the viewed heatmap.
    private List<Cell> cellsOutsideExploredVision = new List<Cell>(); // cells outside the viewed heatmap.
    private List<Cell> edgeOfVision = new List<Cell>();      // cells on the edge of the viewed heatmap.
    public List<Cell> plantsOnEdge = new List<Cell>();      // cells that contain plants on the edge of the viewed heatmap

    Coroutine spawningActive;
    int staticCellToSpawn = 0;



    private void Start()
    {
        cellDiameter = cellMapping.mapCellDiameter;
    }

    void Update()
    {

        if (cellMapping.spawnableCellList.Count > sizeOfList)  // reports back the max size of the list to help watch for infinite lists
        {
            sizeOfList = cellMapping.spawnableCellList.Count;
        }

        if (edgeOfVision.Count > sizeOfEdgeList) // reports back the size of the edge list to monitor its size
        {
            sizeOfEdgeList = edgeOfVision.Count;
        }





        if (spawningActive == null)
        {
            spawningActive = StartCoroutine(Populate()); //Running the spawning system
        }
    }

    /// <summary>
    /// Calls the correct system type to what is selected.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Populate()
    {

        if (systemTypeRunning == SystemTypeRunning.Static)
        {
            StartCoroutine(StaticSpawns());
        }
        if (systemTypeRunning == SystemTypeRunning.Random)
        {
            StartCoroutine(RandomSpawns());
        }
        if (systemTypeRunning == SystemTypeRunning.Heatmap)
        {
            StartCoroutine(DynamicHeatmapSpawns());
        }

        yield return new WaitForFixedUpdate();
    }




    /// <summary>
    /// Calls GetRandomCell and creates a time delay for spawning. Limits amount of cells to be spawned to the resource number.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RandomSpawns()
    {
        DynamicWaitTimeDelay();
        yield return new WaitForSeconds(waitTimeDelay);

        if (resourceCells.Count > resourceNumber - 1)
        {
            spawningActive = null;
        }
        else
        {
            GetRandomCell();
        }

        yield return new WaitForEndOfFrame();
        StopCoroutine(spawningActive);
    }
    /// <summary>
    /// Updates the wait time delay for the random and dynamic systems
    /// </summary>
    /// <returns></returns>
    public float DynamicWaitTimeDelay()
    {
        if (waitTimeDelay >= 10)
        {
            return waitTimeDelay = 10;
        }
        if (waitTimeDelay <= 2)
        {
            return waitTimeDelay += 0.2f;
        }
        else
        {
            return waitTimeDelay += 2f;
        }
    }

    /// <summary>
    /// Calls GetStaticCell and creates a time delay for spawning. Limited by number of static cells.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StaticSpawns()
    {
        if (waitTimeDelay >= 8)
        {
            waitTimeDelay = 8;
        }
        if (waitTimeDelay <= 2)
        {
            waitTimeDelay += 0.1f;
        }
        else
        {
            waitTimeDelay += 0.4f;
        }

        yield return new WaitForSeconds(waitTimeDelay);
        



        GetStaticCell();
        yield return new WaitForEndOfFrame();
        StopCoroutine(spawningActive);
    }

    /// <summary>
    /// Calls GetHeatmapSpawnCell and creates a time delay for spawning. Limits amount of cells to be spawned to the resource number.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DynamicHeatmapSpawns()
    {
        DynamicWaitTimeDelay();
        yield return new WaitForSeconds(waitTimeDelay);

        if (resourceCells.Count > resourceNumber - 1)
        {
            spawningActive = null;
        }
        else
        {
            GetHeatmapSpawnCell();
        }

        yield return new WaitForEndOfFrame();
        StopCoroutine(spawningActive);
    }




    /// <summary>
    /// Finds a random cell position to be used in Random spawn mode.
    /// </summary>
    private void GetRandomCell()
    {
        Cell p_Spawn;

        
        
        if (cellMapping.spawnableCellList.Count != 0)
        {
            p_Spawn = cellMapping.spawnableCellList[Random.Range(0, cellMapping.spawnableCellList.Count)]; // find a random cell that is in the spawnable list

            UpdateSpawnableList(p_Spawn);

            StartCoroutine(SpawnSolver(p_Spawn));
        }
        else
        {
            spawningActive = null;
        }
    }

    /// <summary>
    /// Finds Static cell positions to spawn in static spawn mode. Bounces through the list spawning them.
    /// </summary>
    private void GetStaticCell()
    {
        Cell p_Spawn;

        if (staticCellToSpawn == staticPlants.Count - 1) // Cycle through all static spawn spots by bouncing between the first and last spawn location
        {
            spawnBounce = false;
        }
        if (staticCellToSpawn == 0)
        {
            spawnBounce = true;
        }

        p_Spawn = cellMapping.CellFromWorldPoint(staticPlants[staticCellToSpawn].position);
        
        if (spawnBounce)
        {
            staticCellToSpawn++;
        }
        else
        {
            staticCellToSpawn--;
        }
        
        StartCoroutine(SpawnSolver(p_Spawn));
    }

    /// <summary>
    /// Finds a cell when Dynamic Heatmap mode is being used
    /// </summary>
    public void GetHeatmapSpawnCell()
    {
        Cell p_Spawn;


        foreach (Cell cell in cellMapping.initSpawnableCellList)
        {
            //CELLS FOUND TO BE OUTSIDE VISION
            if (cell.temprature < 0.1 && !cellsOutsideExploredVision.Contains(cell))
            {
                cellsOutsideExploredVision.Add(cell);
            }
            //CELLS FOUND TO BE IN VISION
            if (cell.temprature > 0.1 && !cellsInExploredVision.Contains(cell))
            {
                cellsInExploredVision.Add(cell);
                edgeOfVision.Remove(cell);
                cellsOutsideExploredVision.Remove(cell);
            }
            //FOR EACH CELL WITHIN VISION
            if (cellsInExploredVision.Contains(cell))
            {   //FIND NEIGHBOURS THAT ARE ON THE EDGE
                foreach (Cell neighbour in cellMapping.FindNeighbours(cell, 3))
                {
                    if (cellsOutsideExploredVision.Contains(neighbour) && neighbour.temprature < 0.08f && !edgeOfVision.Contains(neighbour))
                    {
                        edgeOfVision.Add(neighbour);
                    }
                    else
                    {
                        edgeOfVision.Remove(neighbour);
                    }
                }
            }
        }

        // If player has not explored enough, do not start the Spawning.
        if (cellsInExploredVision.Count > minExplorationBeforeSpawning)
        {
            if (plantsOnEdge.Count < 4 && edgeOfVision.Count != 0) // limit the amount of plants that can spawn on the edge of exploration
            {

                p_Spawn = edgeOfVision[Random.Range(0, edgeOfVision.Count)]; // find a cell on the edge of vision
                Debug.LogWarning("Spawning on edge");

                UpdateSpawnableList(p_Spawn);
                plantsOnEdge.Add(p_Spawn);
                StartCoroutine(SpawnSolver(p_Spawn));


            }
            else
            {
                p_Spawn = cellsOutsideExploredVision[Random.Range(0, cellsOutsideExploredVision.Count)]; // find a cell that hasnt been explored yet
                Debug.Log("Spawning outside explored area");
                UpdateSpawnableList(p_Spawn);
                StartCoroutine(SpawnSolver(p_Spawn));
            }
        }
        else if (cellMapping.spawnableCellList.Count != 0)
        {
            p_Spawn = cellMapping.spawnableCellList[Random.Range(0, cellMapping.spawnableCellList.Count)]; // find a random cell that is in the spawnable list
            UpdateSpawnableList(p_Spawn);
            StartCoroutine(SpawnSolver(p_Spawn));

        }
        else
        {
            spawningActive = null;
        }
    }


    /// <summary>
    /// Spawns a plant on the selected spawn point. Checks for if it is occupied when static and refuses null spawn location.
    /// </summary>
    /// <param name="p_Spawn"></param>
    /// <returns></returns>
    public IEnumerator SpawnSolver(Cell p_Spawn)
    {
        yield return new WaitForFixedUpdate();
        // the first resource cell is just random from the list.
        if (resourceCells.Count == 0) 
        {
            resourceCells.Add(p_Spawn);
            plantOnPoint(p_Spawn.worldPosition);
            succesfullySpawned++;
            spawningActive = null;
        }
        else
        {

            // if the plant is too close to other plants, choose another location to spawn
            tooClose = false;
            // switch statement for each of the systems: Static, Random, Heatmap
            switch (systemTypeRunning)
            {
                case SystemTypeRunning.Static:
                    foreach (Cell cell in resourceCells)
                    {
                        // prevents spawning of static plants on top of eachother 
                        if (Vector3.Distance(p_Spawn.worldPosition, cell.worldPosition) < resourceGap)
                        { 
                            tooClose = true;
                            spawningActive = null;
                            spawn = null;
                            break;
                        }
                        else
                        {
                            spawn = p_Spawn;
                        }
                    }
                    
                    break;
                case SystemTypeRunning.Random:
                    foreach (Cell cell in resourceCells)
                    {
                        // prevents spawning of plants on top of eachother 
                        if (Vector3.Distance(p_Spawn.worldPosition, cell.worldPosition) < resourceGap)
                        {
                            tooClose = true;
                            spawningActive = null;
                            spawn = null;
                            break;
                        }
                        else
                        {
                            spawn = p_Spawn;
                        }
                    }
                    break;
                case SystemTypeRunning.Heatmap:
                    foreach (Cell cell in resourceCells)
                    {
                        // prevents spawning of plants on top of eachother 
                        if (Vector3.Distance(p_Spawn.worldPosition, cell.worldPosition) < resourceGap)
                        {
                            tooClose = true;
                            spawningActive = null;
                            spawn = null;
                            break;
                        }
                        else
                        {
                            spawn = p_Spawn;
                        }
                    }
                    break;
            }

            if (spawn != null)
            {
                resourceCells.Add(spawn); // add spawned plant to the resources list
                succesfullySpawned++;
                plantOnPoint(spawn.worldPosition); // spawn plant on the selected area
                UpdateSpawnableList(spawn);
                spawningActive = null;
            }
        }
    }



    // // // // // 

    /// <summary>
    /// Places a plant on the Vector3 in world space
    /// </summary>
    /// <param name="spawnPoint"></param>
    private void plantOnPoint(Vector3 spawnPoint)
    {
        Instantiate(plantPrefab, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    /// <summary>
    /// Updates the Cell Map with which cell is now occupied and that it needs to update its neighbours.
    /// </summary>
    /// <param name="p_Spawn"></param>
    private void UpdateSpawnableList(Cell p_Spawn)
    {
        foreach (Cell cell in cellMapping.FindNeighbours(p_Spawn, cellUpdateRange))
        {
            cell.spawnable = false;
            // temp
            edgeOfVision.Remove(cell);
            cellsOutsideExploredVision.Remove(cell);
            // temp
            cellMapping.spawnableCellList.Remove(cell);
        }
        p_Spawn.spawnable = false;
        cellMapping.spawnableCellList.Remove(p_Spawn);

        cellMapping.localSpawnableCellCheck(cellMapping.FindNeighbours(p_Spawn, cellUpdateRange)); // updating the cells neighbours in cellmap spawnable list

    }




/// <summary>
/// Spawn points designated with yellow boxes
/// </summary>
    private void OnDrawGizmos()
    {
    if (cellMapping != null && resourceCells.Count > 1)
        {
            foreach (Cell cell in resourceCells) // cells with plants on are made yellow cubes.
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(cell.worldPosition, new Vector3(cellDiameter, cellDiameter, cellDiameter));
            }

            foreach (Cell cell in cellsOutsideExploredVision) // Colour cells outside of the vision blue
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(cell.worldPosition, new Vector3(1f, 1f, 1f));
            }

            foreach (Cell cell in edgeOfVision) // Colour cells on the edge of the vision yellow
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(cell.worldPosition, new Vector3(1.5f, 1.5f, 1.5f));
            }
        }
    }

}
