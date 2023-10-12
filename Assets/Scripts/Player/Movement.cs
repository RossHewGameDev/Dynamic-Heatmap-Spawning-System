using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [SerializeField] CharacterController characterController;
    [SerializeField] Camera cam;
    [Header("Player Values")]
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float rotationSpeed;
    [SerializeField] float gravity;

    private PlayerInput playerInput;
    private InputAction m_Input;
    private InputAction jumpAction;
    private Coroutine j_coroutine;
    private Quaternion targetRotation;

    private bool groundedPlayer;
    private Vector3 playerVelocity;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        m_Input = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        groundedPlayer = true;
    }

    private void Update()
    {
        movementAction();
    }

    private void movementAction()
    {
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        Vector2 moveInput = m_Input.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = move.x * cam.transform.right.normalized + move.z * cam.transform.forward.normalized;
        move.y = 0f;
        characterController.Move(move * Time.deltaTime * playerSpeed);

        
        if (!characterController.isGrounded)
        {
            //j_coroutine = StartCoroutine(jumpCooldown());
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
        
        //targetRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }


}
