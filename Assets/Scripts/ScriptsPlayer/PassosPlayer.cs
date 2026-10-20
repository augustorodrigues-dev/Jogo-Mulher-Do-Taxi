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

    void Update()
    {
        
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            bool estaCorrendo = Input.GetKey(KeyCode.LeftShift);
            bool estaAgachado = Input.GetKey(KeyCode.LeftControl);

            
            if (estaCorrendo)
            {
                
                somPassosSource.clip = somCorrendo;
                somPassosSource.pitch = pitchCorrendo;
            }
            else if (estaAgachado)
            {
                
                somPassosSource.clip = somAgachado;
                somPassosSource.pitch = pitchAgachado;
            }
            else
            {
                somPassosSource.clip = somAndando;
                somPassosSource.pitch = pitchAndando;
            }

            
            if (!somPassosSource.isPlaying)
            {
                somPassosSource.Play();
            }
        }
        else
        {
            
            somPassosSource.Stop();
        }
    }
}