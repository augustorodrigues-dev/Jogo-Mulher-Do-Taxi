using UnityEngine;

public class PassosPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource somPassosSource;
    [SerializeField] private CharacterController characterController;

    [Header("Sons de Passos")]
    [SerializeField] private AudioClip somAndando;
    [SerializeField] private AudioClip somCorrendo;

    [SerializeField] private AudioClip somAgachado;

    [Header("Configurações de Ritmo")]
    [Tooltip("Controla a velocidade do som ao andar")]
    [SerializeField] private float pitchAndando = 1.0f;
    [Tooltip("Controla a velocidade do som ao correr. Um valor maior significa passos mais rápidos.")]
    [SerializeField] private float pitchCorrendo = 1.0f;

    [SerializeField] private float pitchAgachado = 1.0f;

    private MovimentoPlayer movimento;
    public bool Configurado => somPassosSource != null && characterController != null;
    public bool UsaAudio(AudioSource source) => somPassosSource == source;

    private void Awake()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (characterController != null) movimento = characterController.GetComponent<MovimentoPlayer>();
    }

    private void OnDisable() => Parar();

    private void Parar()
    {
        if (somPassosSource != null && somPassosSource.isPlaying) somPassosSource.Stop();
    }

    // Depois do movimento, para consultar a velocidade do frame atual.
    private void LateUpdate()
    {
        if (!Configurado) return;
        if (GerenciadorDePause.estaPausado ||
            (movimento != null && (!movimento.isActiveAndEnabled || movimento.desativarPassos)) ||
            !characterController.isGrounded || characterController.velocity.sqrMagnitude <= 0.01f)
        {
            Parar();
            return;
        }

        bool agachado = movimento != null ? movimento.EstaAgachado : Input.GetKey(KeyCode.LeftControl);
        bool correndo = !agachado && Input.GetKey(KeyCode.LeftShift);
        AudioClip clip = agachado ? somAgachado : correndo ? somCorrendo : somAndando;
        float pitch = agachado ? pitchAgachado : correndo ? pitchCorrendo : pitchAndando;
        if (clip == null) { Parar(); return; }
        if (somPassosSource.clip != clip) somPassosSource.clip = clip;
        if (somPassosSource.pitch != pitch) somPassosSource.pitch = pitch;
        if (!somPassosSource.isPlaying) somPassosSource.Play();
    }
}
