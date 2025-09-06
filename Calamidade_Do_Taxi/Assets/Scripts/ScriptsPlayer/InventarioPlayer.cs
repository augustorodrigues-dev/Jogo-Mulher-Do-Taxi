using UnityEngine;

public class InventarioPlayer : MonoBehaviour
{
    public bool temChave = false;
    public bool estaAgachado = false; // <-- NOVA VARIÁVEL

    public KeyCode teclaAgachar = KeyCode.LeftControl; // Tecla para agachar

    // Para o efeito visual de agachar (opcional)
    void Start()
    {
        
    }

    void Update()
    {
        // Verifica se o jogador pressionou a tecla de agachar
        if (Input.GetKeyDown(teclaAgachar))
        {
            estaAgachado = true;
            // Efeito visual: diminui a altura do jogador no eixo Y
            
        }
        // Verifica se o jogador soltou a tecla de agachar
        else if (Input.GetKeyUp(teclaAgachar))
        {
            estaAgachado = false;
            // Efeito visual: retorna o jogador à altura original
            
        }
    }
}