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

    private Vector3 moveDirection;
    public float rotationX;
    private CharacterController characterController;
    private InventarioPlayer inventario;
    private PassosPlayer passosSeparados;
    private bool isCrouching;
    public bool EstaAgachado => isCrouching;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inventario = GetComponent<InventarioPlayer>();
        passosSeparados = GetComponent<PassosPlayer>();
        isCrouching = inventario != null && inventario.estaAgachado;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable() => PararPassos();

    private void PararPassos()
    {
        if (somPassosSource != null && somPassosSource.isPlaying)
            somPassosSource.Stop();
    }

    private void Update()
    {
        if (GerenciadorDePause.estaPausado)
        {
            PararPassos();
            return;
        }

        KeyCode teclaAgachar = inventario != null ? inventario.teclaAgachar : KeyCode.LeftControl;
        if (Input.GetKeyDown(teclaAgachar))
        {
            isCrouching = !isCrouching;
            if (inventario != null) inventario.estaAgachado = isCrouching;
        }

        float altura = isCrouching ? crouchHeight : defaultHeight;
        if (characterController.height != altura) characterController.height = altura;

        Vector2 input = Vector2.ClampMagnitude(new Vector2(
            Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1f);
        bool isRunning = !isCrouching && Input.GetKey(KeyCode.LeftShift);
        float speed = isCrouching ? crouchSpeed : isRunning ? runSpeed : walkSpeed;
        float velocidadeVertical = moveDirection.y;
        moveDirection = (transform.forward * input.y + transform.right * input.x) * speed;

        bool noChao = characterController.isGrounded;
        moveDirection.y = noChao && Input.GetButton("Jump") ? jumpPower : velocidadeVertical;
        if (!noChao) moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);

        rotationX = Mathf.Clamp(rotationX - Input.GetAxis("Mouse Y") * lookSpeed, -lookXLimit, lookXLimit);
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.rotation *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * lookSpeed, 0f);

        // Mantém os dois componentes compatíveis com cenas existentes, com um único dono do áudio.
        if (passosSeparados != null && passosSeparados.isActiveAndEnabled && passosSeparados.Configurado)
        {
            if (!passosSeparados.UsaAudio(somPassosSource)) PararPassos();
        }
        else
        {
            HandlePassos(isRunning, isCrouching, input.sqrMagnitude > 0f);
        }
    }

    private void HandlePassos(bool running, bool crouching, bool temInput)
    {
        if (somPassosSource == null) return;
        if (desativarPassos || !temInput || !characterController.isGrounded ||
            characterController.velocity.sqrMagnitude <= 0.04f)
        {
            PararPassos();
            return;
        }

        AudioClip clip = crouching ? somAgachado : running ? somCorrendo : somAndando;
        float pitch = crouching ? pitchAgachado : running ? pitchCorrendo : pitchAndando;
        if (clip == null) { PararPassos(); return; }
        if (somPassosSource.clip != clip) somPassosSource.clip = clip;
        if (somPassosSource.pitch != pitch) somPassosSource.pitch = pitch;
        if (!somPassosSource.isPlaying) somPassosSource.Play();
    }
}
