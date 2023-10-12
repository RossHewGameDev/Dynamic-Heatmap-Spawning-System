using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// Resources read and watched (seperate from Academic refrences):  
/// https://news.movel.ai/theta-star/ (A valuable guide for introducing Theta* elements)
/// https://theory.stanford.edu/~amitp/GameProgramming/ (Help for understanding A* and good refrence for imlementation) 
/// https://youtu.be/-L-WgKMFuhE (Sebastian Lague's A* video was useful refrence for the basic layout in Unity)
/// https://johntgz.github.io/2020/08/31/theta_star/#enter-the-theta (Help for understanding A* and good refrence for imlementation) 
/// https://youtu.be/dn1XRIaROM4 (Sebastai Lague's A* unit example was used with adjustments to help produce the basic path for the agents - doing it from scratch was quite hard)

/// <summary>
/// The pathing manager that talks to the pathfinder and the Agent(s).
/// Stores the queue of paths that have been requested by agents (Yes! In future I could even have MULTIPLE agents. scary.)
/// 
/// THIS WAS BARELY MODIFIED FROM SEBSTIAN LAGUE'S TUTORIAL --- Since the functionality is essentially the same, there was little reason to modifiying this piece of work
/// 
/// </summary>
public class PathingManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestsQueue = new Queue<PathRequest>(); // creates the path request queue
    PathRequest currentPathRequest;

    static PathingManager pathingManagerInstance;
    Pathfinder pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        pathingManagerInstance = this;             // creates a pathing manager instance 
        pathfinding = GetComponent<Pathfinder>(); // grabs the pathfinder
    }
    /// <summary>
    /// Adds a new path the the path request queue and then asks for it to be processed
    /// </summary>
    /// <param name="pathStart"></param>
    /// <param name="pathEnd"></param>
    /// <param name="callback"></param>
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) // Action Vector3[] is the path input
    {
        Debug.Log("New path Request");
        PathRequest newPathRequest = new PathRequest(pathStart, pathEnd, callback);
        pathingManagerInstance.pathRequestsQueue.Enqueue(newPathRequest);   // queues the new path from the agent
        pathingManagerInstance.TryProcessNext();
    }

    /// <summary>
    /// Attempts to process the next Path in the Queue
    /// Asks the pathfinding to create a new path from the path requests queue
    /// </summary>
    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestsQueue.Count > 0)
        {
            currentPathRequest = pathRequestsQueue.Dequeue();           // deques the path request from the queue
            isProcessingPath = true;                                   
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd); // requests a path from pathfinding
        }
    }

    /// <summary>
    /// takes the path from the Pathfinder and asks for it to be processed
    /// </summary>
    /// <param name="path"></param>
    /// <param name="success"></param>
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        Debug.Log("Path processing finished");
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    /// <summary>
    /// The Path request Structure that holds the path that has been requested. 
    /// </summary>
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }

}
