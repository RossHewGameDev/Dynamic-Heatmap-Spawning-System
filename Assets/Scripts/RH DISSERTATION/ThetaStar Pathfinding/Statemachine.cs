using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MindState
{
    Idle,
    Fleeing
}

/// <summary>
/// The Statemachine that controls the actions of the glider and where it wants to path to.
/// </summary>
public class Statemachine : MonoBehaviour
{

    [Header("Required")]
    [SerializeField] GameObject playerGlider;
    [SerializeField] Transform pathingTarget;

    [Header("Modifiable")]

    [Tooltip("The spots which the glider will try to prioritise fleeing too (put them in tunnels and cool areas!)")]
    [SerializeField] List<Transform> runAwaySpots;

    [Tooltip("Distance that the glider will attempt to runaway from the player at")]
    [SerializeField] float runAwayDistance;

    [Tooltip("The distance of which the glider will idle from the center of the map")]
    [SerializeField] int idleDistance;

    [Tooltip("The distance at which the glider will by minimum pathfind away from the player")]
    [SerializeField] int attemptEscapeDistance;

    [Header("DO NOT MODIFY")]
    public MindState mindState; // The current state of the gliders AI

    private bool AIRunning;   // returns if the AI is currently running or not (the entire AI can be disabled by turning this to be false)

    private bool hasBeenCaught;         // checks if the player has been caught for custom behaviour after that (without disabling the AI)
    private float distanceFromPlayer;  
    private bool sightOfPlayer;       // shows whether the player is in direct line of sight to the agent
    private HashSet<Transform> runAwaySpotsDone;

    private Coroutine statemachineAI;

    void Start()
    {
        idleDistance = (int)(idleDistance * 0.5f); // setting idle distance to a radius around 0,0,0
        mindState = MindState.Idle;               // setting default mindstate at start
        AIRunning = false;                       // starting the AI as disabled so it can be enabled after the cellmap is made
        hasBeenCaught = false;                  // AI has not been caught as the game hasnt started
        statemachineAI = null;                 // setting coroutine as null so it can be filled later
    }

    private void Update()
    {
        // Grabs the distance from the player
        distanceFromPlayer = Vector3.Distance(transform.position, playerGlider.transform.position);

        if (!AIRunning) // Makes sure that only 1 AI coroutine instance is running at a time.
        {
            statemachineAI = StartCoroutine(StateMachineAI());
        }
    }

    /// <summary>
    /// The main State Machine of the AI: Contains the high level logic and switching of different states of the AI
    /// </summary>
    /// <returns></returns>
    private IEnumerator StateMachineAI()
    {
        AIRunning = true;

        if (!hasBeenCaught) // if the AI has not been caught: the AI will be running 
        {
            if (distanceFromPlayer < runAwayDistance)
            {
                mindState = MindState.Fleeing; // changes the mindstate to fleeing
                if (lineOfSight(transform.position, playerGlider.transform.position)) // checks for line of sight with the player
                {
                    sightOfPlayer = true; 
                }
                else
                {
                    sightOfPlayer = false;
                }
            }
            else
            {
                mindState = MindState.Idle; // Changes mindstate to idle
            }
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
        AIRunning = false;
        yield return new WaitForEndOfFrame();
    }


    /// <summary>
    /// Sets a new flight target position depending on the mindstate of the AI
    /// </summary>
    public void getFlightTarget()
    {
        if (mindState == MindState.Idle)
        {
            pathingTarget.position = new Vector3(Random.Range(-idleDistance, idleDistance), Random.Range(-idleDistance, idleDistance), Random.Range(-idleDistance, idleDistance)); //Get random position
        }        
        
        if (mindState == MindState.Fleeing)
        {
            if (sightOfPlayer && runAwaySpots.Count > 2) // if there are more than 2 runaway spots in a map
            {
                Transform nearestSpot;
                foreach (Transform item in runAwaySpots)   // find the nearest runaway spot that hasnt already been used
                {
                    if (runAwaySpotsDone.Count == runAwaySpots.Count) // if the spots have all been use, just use them all again.
                    {
                        runAwaySpotsDone.Clear();
                    }
                    if (nearestSpot = null) 
                    {
                        nearestSpot = item;
                    }
                    if (Vector3.Distance(transform.position,item.position) < Vector3.Distance(transform.position, nearestSpot.position) && !runAwaySpotsDone.Contains(nearestSpot))
                    {
                        nearestSpot = item; // find the nearest spot in the list that isnt already completed
                    }
                    pathingTarget.position = nearestSpot.position; // path towards the nearest spot
                    runAwaySpotsDone.Add(nearestSpot); // add completed spot to the completed hash set
                }
            }
            //Get random position
            Vector3 possiblePos = new Vector3(Random.Range(-idleDistance, idleDistance), Random.Range(-idleDistance, idleDistance), Random.Range(-idleDistance, idleDistance));
            
            if (Vector3.Distance(possiblePos, playerGlider.transform.position) > attemptEscapeDistance) // any target point must be outside the players escape distance
            {
                pathingTarget.position = possiblePos;
            }
        }
    }

    /// <summary>
    /// Finds if there is a direct line of sight between two objects
    /// </summary>
    /// <param name="targetA"></param>
    /// <param name="targetB"></param>
    /// <returns></returns>
    private bool lineOfSight(Vector3 targetA, Vector3 targetB)
    {
        Vector3 dir = targetA - targetB;
        dir = -dir;

        dir.Normalize();
        Ray ray = new Ray(targetA, dir);
        if (!Physics.Raycast(ray, runAwayDistance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
