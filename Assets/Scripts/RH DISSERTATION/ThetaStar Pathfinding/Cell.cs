using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;


// TODO: Implement Composite Pattern for Cell Clusters. Groups of cells can then be treated as a single entity when an area is not traversable.

/// <summary>
/// Properties of the Cell Objects that populate the cell map
/// </summary>
public class Cell
{
    public Cell parentCell;
    public bool traversable;        // defines whether the cell can be pathed through
    public Vector3 worldPosition;   

    public int gridX;
    public int gridY;
    public int gridZ;

    public float gCost;
    public float hCost;

    public float temprature;
    public Color colour;
    public bool spawnable;  


    public bool isEdgeCell; // is this cell currently on the edge of explored space?



    private float defaultCellWeight = 50; // Weight for the edge cells in the heatmap
    private float tempratureWeight = 100; // Weight for the temprature in the heatmap
    private float distanceWeight = 100; // Weight for the temprature in the heatmap

    // DEBUGGING
    public bool isCell25
    {
        get
        {
            if (gridX == 20 && gridY == 5 && gridZ == 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private float _closestPlantDistance;

    /// <summary>
    /// Constructor for the Cell object. 
    /// </summary>
    /// <param name="_traversable"></param>
    /// <param name="_worldPosition"></param>
    /// <param name="_gridX"></param>
    /// <param name="_gridY"></param>
    /// <param name="_gridZ"></param>
    public Cell(bool _traversable, Vector3 _worldPosition, int _gridX, int _gridY, int _gridZ, float _temprature, bool _spawnable)
    {
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;
        traversable = _traversable;
        worldPosition = _worldPosition;
        temprature = _temprature;              // temprature has been added for the heatmap
        colour = new Color(0f, 0f, 1f, 0.01f);// Colour has been added for the heatmap
        spawnable = _spawnable;              // Spawnable has been added for the heatmap


        DebugPrioritySystem();

    }

    private void DebugPrioritySystem()
    {
        // Debugging for the priority system
        if (gridX == 20 && gridY == 5 && gridZ == 20)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = worldPosition;
        }
    }


    /// <summary>
    /// Legacy for the Theta* pathfinding 
    /// </summary>
    public float fCost // cell heuristics cost
    {
        get
        {
            return gCost + hCost;
        }
    }

    /// <summary>
    /// Priority rating for the cell, used to determine the best cell to spawn a plant on.
    /// The priority rating is calculated based on the temprature and distance from the spawned plants.
    /// Lower values are higher priority!
    /// </summary>
    public float spawnPriority
    {
        get
        {   
            if (isEdgeCell)
            {
                return (temprature * tempratureWeight) + _closestPlantDistance;
            }
            else
            {
                return (temprature * tempratureWeight) + defaultCellWeight + _closestPlantDistance ;
            }
            
        }
    }

    /// <summary>
    /// Distance from the closest plant to the cell
    /// </summary>
    public float closestPlantDistance
    {
        get
        {
            return _closestPlantDistance;
        }
        set
        {
            _closestPlantDistance = value;
        }
    }

    /// <summary>
    /// Calculates the distance from the closest plant to the cell from the resourceCells list
    /// </summary>
    /// <param name="_resourceCells"></param>
    /// <returns></returns>
    public void ClosestPlantDistance(List<Cell> _resourceCells)
    {
        // distance should be set after the list has been populated. 250 is an arbitrary max value.
        float distanceToClosestPlant = 100;
        
        // resetting the closest plant distance
        closestPlantDistance = distanceToClosestPlant; 

        // If there are no plants, the distance is 0. 
        if (_resourceCells.Count != 0) 
        {
            foreach (Cell cell in _resourceCells)
            {
                float distance = Vector3.Distance(cell.worldPosition, worldPosition);

                if (distance < distanceToClosestPlant)
                {
                    distanceToClosestPlant = distance;
                }
            }
        }

        // Set the closest plant distance
        closestPlantDistance = distanceWeight - distanceToClosestPlant;
    }

}
