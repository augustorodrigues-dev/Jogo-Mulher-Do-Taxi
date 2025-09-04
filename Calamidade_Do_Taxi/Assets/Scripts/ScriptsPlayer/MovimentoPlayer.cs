using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimentoPlayer : MonoBehaviour // Renomeado para um nome mais geral
{
    [Header("Referências")]
    public Camera playerCamera;
    [SerializeField] private AudioSource somPassosSource; // Adicionado do script de passos

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
    [SerializeField] private float pitchCorrendo = 1.5f; // Ajustado para ser mais rápido que andando
    [SerializeField] private float pitchAgachado = 0.8f; // Ajustado para ser mais lento

    // Variáveis privadas
    private Vector3 moveDirection = Vector3.zero;
    public float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private float originalWalkSpeed; // Guarda a velocidade original
    private float originalRunSpeed;  // Guarda a velocidade original

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Guarda as velocidades originais para restaurar depois de agachar
        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;
    }

    void Update()
    {
        if (GerenciadorDePause.estaPausado)
        {
            somPassosSource.Stop(); // Garante que o som para se o jogo for pausado
            return;
        }
        
        // --- LÓGICA DE MOVIMENTO ---
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // Agachar
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

        // Pulo
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Gravidade
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Mover o personagem
        characterController.Move(moveDirection * Time.deltaTime);

        // --- LÓGICA DA CÂMERA ---
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // --- LÓGICA DOS PASSOS ---
        HandlePassos(isRunning, isCrouching);
    }

    // Função separada para organizar a lógica dos sons de passos
    private void HandlePassos(bool running, bool crouching)
    {
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

            // Toca o som se o clip ou o pitch mudaram, ou se não estiver tocando
            if (somPassosSource.clip != clipAtual || !somPassosSource.isPlaying)
            {
                somPassosSource.clip = clipAtual;
                somPassosSource.pitch = pitchAtual;
                somPassosSource.Play();
            }
        }
        else
        {
            // Para o som se o jogador parar ou pular
            somPassosSource.Stop();
        }
    }
}