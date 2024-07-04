using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This script is used to interact with the plants and the hive in the game.
/// </summary>
public class Interaction : MonoBehaviour
{
    [SerializeField] Camera _camera;

    private PlayerInput playerInput;
    private InputAction interactAction;

    public int plantsCollected;
    public int hiveStored;

    public GameObject lastPlantDestroyed;

    public delegate void PlantDelete();
    public static event PlantDelete plantDelete;



    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];

        Cursor.lockState = CursorLockMode.Locked; // locks cursor 
    }

    private void Update()
    {
        if (interactAction.triggered)
        {
            interact();
        }

        Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 2.5f, Color.green);

    }
    /// <summary>
    /// fires a raycast from the player to pick up plants. if the player has 12 plants, they cannot pickup anymore.
    /// </summary>
    private void interact()
    {
        RaycastHit hit;

        if (Physics.SphereCast(_camera.transform.position, 1, _camera.transform.forward, out hit, 2.5f))
        {
            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("Plant"))
            {
                if (plantsCollected < 12)
                {
                    lastPlantDestroyed = objectHit.gameObject;
                    plantDelete();// invoking destroy event
                    plantsCollected++;
                }
            }
            
            
            if (objectHit.CompareTag("Hive"))
            {
                hiveStored += plantsCollected;
                plantsCollected = 0;
            }
            // Do something with the object that was hit by the raycast.
        }
    }


}
