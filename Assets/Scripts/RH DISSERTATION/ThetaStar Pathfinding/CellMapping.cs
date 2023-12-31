using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// THIS SCRIPT HAS BEEN MODIFIED FROM COMP250 TO ACT AS AN ELEMENT IN MY DISSERTATION.
/// Edited lines have been pointed out in comments and brand new functions will also be clearly labelled 
/// 
/// 
/// 
/// GIT LOG WILL SHOW CHANGES AT https://github.falmouth.ac.uk/Games-Academy-Student-Work-22-23/RH246018-Dissertation/commits/main


/// Resources read and watched (seperate from Academic refrences):  
/// https://news.movel.ai/theta-star/ (A valuable guide for introducing Theta* elements)
/// https://theory.stanford.edu/~amitp/GameProgramming/ (Help for understanding A* and good refrence for imlementation) 
/// https://youtu.be/-L-WgKMFuhE (Sebastian Lague's A* series was useful refrence for the basic layout in Unity)
/// https://johntgz.github.io/2020/08/31/theta_star/#enter-the-theta (Help for understanding A* and good refrence for imlementation) 

/// <summary>
/// Generates the cell map which is used by the Theta Star Algorithm to find untraversable areas to pathfind around.
/// </summary>
public class CellMapping : MonoBehaviour
{
    public Transform player;
    public LayerMask untraversableMask;
    public LayerMask visionCone;
    public LayerMask spawnLayer;
    public Vector3 worldSize;   // size of the world that the map generates in
    public float cellRadius;   // radius of the cells that populate the map
    [SerializeField] float collisionPaddingSize;
    public Cell[,,] cellMap; // the overall map 

    [SerializeField] public List<Cell> heatUpCellList = new List<Cell>(); /// DISSERTATION ADDITION
    [SerializeField] public List<Cell> spawnableCellList = new List<Cell>(); /// DISSERTATION ADDITION
    [SerializeField] public List<Cell> initSpawnableCellList = new List<Cell>(); /// DISSERTATION ADDITION


    [SerializeField] bool showSpawnableArea; /// DISSERTATION ADDITION


    [Header("Heatmap Adjustments")]         /// DISSERTATION ADDITION
    [SerializeField] float heatNoiseGate;  /// DISSERTATION ADDITION
    [SerializeField] float heatSpeed;     /// DISSERTATION ADDITION
    [SerializeField] float coolSpeed;    /// DISSERTATION ADDITION

    [Header("Spawning Settings")]
    [Tooltip("This changes the distance at which cells will disallow their neighbours to spawn plants")]
    public int neighbourCheckDistance;      /// DISSERTATION ADDITION


    [HideInInspector]public List<Cell> path;     // the path that has been generated (inserted in here so we can debug it and see the gizmos draw the path)

    private bool heatChkRunning;         /// DISSERTATION ADDITION
    private Coroutine tempratureUpdate; /// DISSERTATION ADDITION

    float cellsExplored;     /// DISSERTATION ADDITION
    float explorationValue; /// DISSERTATION ADDITION

    private float cellDiameter;                 
    private int c_Width, c_Height, c_Length;  // cell number in width, height, length
    private Vector3 startPoint;              // the start point of the cell map


    private void Awake()
    {
        cellDiameter = cellRadius * 2;
        c_Width = (int)(worldSize.x / cellDiameter);    // getting the number of cells in Width
        c_Height = (int)(worldSize.y / cellDiameter);  // getting the number of cells in Height
        c_Length = (int)(worldSize.z / cellDiameter); // getting the number of cells in Length
        InitMap(); //Initilizing the cell map

        GlobalSpawnableCellCheck();
        ///ADDED FOR DISSERTATION
        /*
        randomCell = cellMap[Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)];

        heatUpCellList.Add(randomCell); 
        */
        /*
        foreach(Cell cell in FindNeighbours(randomCell))
        {
            heatUpCellList.Add(cell);
        }  
        */
        ///ADDED FOR DISSERTATION
    }

    /// <summary>
    /// ADDED FOR DISSERTATION
    /// </summary>
    private void FixedUpdate()
    {
        if (!heatChkRunning)
        {
            tempratureUpdate = StartCoroutine(TempratureUpdate());
        }
    }


    /// <summary>
    /// grid initilization started here
    /// collision check also takes place in here but this might be changed 
    /// Modified from Sebastian lagues tutorial (https://youtu.be/-L-WgKMFuhE) 
    /// </summary>
    public void InitMap()
    {
          // setting cell number and cellMap size (resolution)
        cellMap = new Cell[c_Width, c_Height , c_Length];
        // setting start point to where the bottom left of the grid would start generating from.
        startPoint = transform.position - (Vector3.right * worldSize.x * 0.5f) - (Vector3.up * worldSize.y * 0.5f) - (Vector3.forward * worldSize.z * 0.5f);


        for (int x = 0; x < c_Width; x++) // looping through each axis to populate the cellmap with cells
        {
            for (int y = 0; y < c_Height; y++)      
            {
                for (int z = 0; z < c_Length; z++)
                {
                        // we add the x y z components to each cell
                    Vector3 position = startPoint + Vector3.right * (x * cellDiameter + cellRadius) + Vector3.up * (y * cellDiameter + cellRadius) + Vector3.forward * (z * cellDiameter + cellRadius);
                     // we then check to see if this cell is traversable (It used to be checking with the cell radius)
                    // checking with cell diameter means we see if theres an object anywhere near this cell
                    bool spawnable;
                    bool traversable = !Physics.CheckSphere(position, 1,untraversableMask);
                    ///adding in check for if spawning system can use area as a spawning location.
                    if (traversable && y == c_Height * 0.5f && Physics.CheckSphere(position, 1, spawnLayer))
                    {
                        spawnable = !Physics.CheckSphere(position, collisionPaddingSize, untraversableMask);
                    }
                    else
                    {
                        spawnable = false;
                    }

                    cellMap[x, y, z] = new Cell(traversable, position,x,y,z,0, spawnable); /// this has had a 0 added to the Cell properties to set all tempratures to 0 for dissertation
                }
            }
        }
    }
    /// <summary>
    /// Grabs a cell location (vector3) in world space 
    /// Modified from Sebastian lagues tutorial (https://youtu.be/-L-WgKMFuhE)
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Cell CellFromWorldPoint(Vector3 worldPos)
    {
        float percentX = AxisPercentage(worldPos, "x");
        float percentY = AxisPercentage(worldPos, "y");
        float percentZ = AxisPercentage(worldPos, "z");

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = (int)(c_Width * percentX);
        int y = (int)(c_Height * percentY);
        int z = (int)(c_Length * percentZ);

        return cellMap[x, y, z];
    }

    /// <summary>
    /// Gets the percentage along the cellmap on specified axis.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="Axis"></param>
    /// <returns></returns>
    public float AxisPercentage(Vector3 worldPos, string Axis)
    {
        if (Axis == "x")
        {
            return (worldPos.x + worldSize.x * 0.5f) / worldSize.x;
        }
        if (Axis == "y")
        {
            return (worldPos.y + worldSize.y * 0.5f) / worldSize.y;
        }
        if (Axis == "z")
        {
            return (worldPos.z + worldSize.z * 0.5f) / worldSize.z;
        }
        else
        {
            Debug.Log("AXIS IS NOT VALID, Please enter x, y or z");
            return 0;
        }
    }

    /// <summary>
    /// adds all cells as neighbours within neighbourCheckDistance cell radius 
    /// Modified from Sebastian lagues tutorial (https://youtu.be/-L-WgKMFuhE)
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public List<Cell> FindNeighbours(Cell cell, int neighbourCheckDistance) //make check distance into argument 
    {
        List<Cell> neighbours = new List<Cell>();

        //loops through the cells that are 1 away from the target cell
        for(int x = -neighbourCheckDistance; x <= neighbourCheckDistance; x++)
        {
            for (int y = -0; y <= 0; y++) // no height check
            {
                for (int z = -neighbourCheckDistance; z <= neighbourCheckDistance; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    int checkX = cell.gridX + x; // get the cells coordinates from the cell map
                    int checkY = cell.gridY + y;
                    int checkZ = cell.gridZ + z;

                    if (checkX >= 0 && checkX < c_Width)
                    {
                        if (checkY >= 0 && checkY < c_Height)
                        {
                            if (checkZ >= 0 && checkZ < c_Length)
                            {
                                // add the neighbour Cell to the cell map 
                                neighbours.Add(cellMap[checkX, checkY, checkZ]);
                            }
                        }
                    }
                }
            }
        }
        return neighbours; 
    }

    /// <summary>
    /// Update the Temprature of a cell [ADDED WORK FOR DISSERTATION]
    /// </summary>
    private IEnumerator TempratureUpdate()
    {
        heatChkRunning = true;
        float alphaLimiter;
        float hotAlphaLimiter;
        alphaLimiter = 0.01f;
        hotAlphaLimiter = 0.7f;
        foreach (Cell currentCell in cellMap)
        {
            if(Physics.CheckSphere(currentCell.worldPosition, 1, visionCone))
            {
                if (currentCell.temprature < 1)
                {
                    currentCell.temprature += heatSpeed;
                }
                if (currentCell.temprature > hotAlphaLimiter)
                {
                    currentCell.colour = new Color(currentCell.temprature, 0, -currentCell.temprature, hotAlphaLimiter);
                }
                else
                {
                    currentCell.colour = new Color(currentCell.temprature, 0, -currentCell.temprature, currentCell.temprature);
                }
                
            }
            else
            {
                if (currentCell.temprature > 0 && currentCell.temprature < heatNoiseGate)
                {
                    currentCell.temprature -= coolSpeed;
                    if (currentCell.temprature < alphaLimiter)
                    {
                        currentCell.colour = new Color(currentCell.temprature, 0,Mathf.Lerp(-currentCell.temprature, currentCell.temprature, 0.01f), alphaLimiter);

                    }
                    else
                    {
                        currentCell.colour = new Color(currentCell.temprature, 0, Mathf.Lerp(-currentCell.temprature, currentCell.temprature,0.01f), currentCell.temprature);
                    }
                }
            }
        }



        yield return new WaitForSeconds(0.1f);
        heatChkRunning = false;
        StopCoroutine(tempratureUpdate);
    }
    /// <summary>
    /// A check for the save system to grab the exploration value %.
    /// </summary>
    /// <returns></returns>
    public float explorationCheck()
    {
        cellsExplored = 0;


        foreach (Cell cell in initSpawnableCellList)
        {
            if (cell.temprature > 0.1) // every cell that has been heated passed the noise gate is counted as "explored"
            {
                cellsExplored++;
            }
        }

        explorationValue = (cellsExplored / initSpawnableCellList.Count) * 100;
        Debug.LogWarning(explorationValue);
        return explorationValue;

    }

    /// <summary>
    /// A public spawn checker for what cells have been made spawnable. Updates the spawnable cell list.
    /// </summary>
    /// <param name="cells"></param>
    public void localSpawnableCellCheck(List<Cell> cells)
    {
        foreach (Cell cell in cells)
        {
            if (cell.spawnable)
            {
                spawnableCellList.Add(cell);
            }
            else
            {
                spawnableCellList.Remove(cell);
            }
        }
    }

    /// <summary>
    /// Local spawn check for this script only when initilising the cellmap.
    /// </summary>
    /// <param name="cells"></param>
    private void GlobalSpawnableCellCheck()
    {
        foreach (Cell cell in cellMap)
        {
            if (cell.spawnable)
            {
                spawnableCellList.Add(cell);
                initSpawnableCellList.Add(cell);
            }
            else
            {
                spawnableCellList.Remove(cell);
                initSpawnableCellList.Remove(cell);
            }
        }
    }


    /// <summary>
    /// Display and debug visuals for the in scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, worldSize); //drawing the world size (bounds of the AI)

        if (cellMap != null)
        {
            Cell playersCurrentCell = CellFromWorldPoint(player.position); 


            foreach (Cell currentCell in cellMap)
            {
                if (!currentCell.traversable) // shows all untraversable cells with red wirecubes
                {
                    /*
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(currentCell.worldPosition, new Vector3(cellDiameter, cellDiameter, cellDiameter));
                    */
                }
                else
                {
                    
                    if (path != null)
                    {
                        if (path.Contains(currentCell)) // shows the path with green cubes
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawCube(currentCell.worldPosition, new Vector3(cellDiameter, cellDiameter, cellDiameter));
                        }
                    }
                    if (playersCurrentCell == currentCell) // shows current player cell (or nearest cell)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(currentCell.worldPosition, new Vector3(cellDiameter, cellDiameter, cellDiameter));
                    }
                    else
                    {
                        if (currentCell.temprature > 0)
                        {
                            Gizmos.color = currentCell.colour; /// section has been edited to work with temprature for the dissertation
                            Gizmos.DrawCube(currentCell.worldPosition, new Vector3(cellDiameter, cellDiameter, cellDiameter));
                        }
                    }
                }
                if (currentCell.spawnable && showSpawnableArea)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(currentCell.worldPosition, new Vector3(cellDiameter, cellDiameter, cellDiameter));
                }
            }
        }
    }

}
