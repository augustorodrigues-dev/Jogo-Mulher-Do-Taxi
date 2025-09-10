using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimentoPlayer : MonoBehaviour 
{
    public bool desativarPassos = false;
    [Header("Referências")]
    public Camera playerCamera;
    [SerializeField] private AudioSource somPassosSource; 

    [Header("Configurações de Movimento")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    [Header("Configurações da Câmera")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Configurações de Agachar")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("Sons de Passos")] 
    [SerializeField] private AudioClip somAndando;
    [SerializeField] private AudioClip somCorrendo;
    [SerializeField] private AudioClip somAgachado;

    [Header("Configurações de Ritmo dos Passos")]
    [SerializeField] private float pitchAndando = 1.0f;
    [SerializeField] private float pitchCorrendo = 1.5f; 
    [SerializeField] private float pitchAgachado = 0.8f; 

    
    private Vector3 moveDirection = Vector3.zero;
    public float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private float originalWalkSpeed; 
    private float originalRunSpeed; 

    private bool isCrouching = false; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;
    }

    void Update()
    {
        if (GerenciadorDePause.estaPausado)
        {
            somPassosSource.Stop(); 
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
    {
        
        isCrouching = !isCrouching;
    }
        
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        if (isCrouching && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = originalWalkSpeed;
            runSpeed = originalRunSpeed;
        }

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        
        characterController.Move(moveDirection * Time.deltaTime);

        
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        
        HandlePassos(isRunning, isCrouching);
    }

    
    private void HandlePassos(bool running, bool crouching)
{
    if (desativarPassos)
    {
        somPassosSource.Stop();
        return;
    }

    if (characterController.isGrounded && characterController.velocity.magnitude > 0.2f && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
    {
        AudioClip clipAtual = somAndando;
        float pitchAtual = pitchAndando;

        if (running && !crouching)
        {
            clipAtual = somCorrendo;
            pitchAtual = pitchCorrendo;
        }
        else if (crouching)
        {
            clipAtual = somAgachado;
            pitchAtual = pitchAgachado;
        }

        if (somPassosSource.clip != clipAtual || !somPassosSource.isPlaying)
        {
            somPassosSource.clip = clipAtual;
            somPassosSource.pitch = pitchAtual;
            somPassosSource.Play();
        }
    }
    else
    {
        somPassosSource.Stop();
    }
}

}