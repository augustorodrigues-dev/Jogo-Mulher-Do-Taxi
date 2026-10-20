
using UnityEngine;

public class RotacaoPortaPrisao : MonoBehaviour
{
    
    public float anguloAbertura = 90.0f;
    public float velocidadeAbertura = 2.0f;
    public KeyCode teclaParaAbrir = KeyCode.E; 

    private bool portaAberindo = false;
    private bool jogadorNaArea = false;
    private Quaternion rotacaoInicial;
    private Quaternion rotacaoFinal;
    private InventarioPlayer inventarioPlayer; 

    void Start()
    {
        
        rotacaoInicial = transform.rotation;
        
        rotacaoFinal = rotacaoInicial * Quaternion.Euler(0, anguloAbertura, 0);
    }

    void Update()
    {
        
        if (jogadorNaArea && Input.GetKeyDown(teclaParaAbrir))
        {
            if (inventarioPlayer != null && inventarioPlayer.temChave)
            {
                Debug.Log("Você tem a chave! Abrindo a porta.");
                portaAberindo = true;
            }
            else
            {
                Debug.Log("Você precisa da chave para abrir esta porta!");
            }
        }

        
        if (portaAberindo)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoFinal, Time.deltaTime * velocidadeAbertura);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            jogadorNaArea = true;
            
            inventarioPlayer = other.GetComponent<InventarioPlayer>();
            Debug.Log("Jogador entrou na área da porta.");
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorNaArea = false;
            inventarioPlayer = null; 
            Debug.Log("Jogador saiu da área da porta.");
        }
    }
}