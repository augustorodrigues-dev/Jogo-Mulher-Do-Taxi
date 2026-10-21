using UnityEngine;

public class InventarioPlayer : MonoBehaviour
{
    public bool temChave = false;
    public bool estaAgachado = false;
    public KeyCode teclaAgachar = KeyCode.LeftControl;
    private MovimentoPlayer movimento;

    private void Awake()
    {
        movimento = GetComponent<MovimentoPlayer>();
    }

    private void Update()
    {
        // Com controlador, somente ele altera a postura e publica o estado.
        if (movimento != null || GerenciadorDePause.estaPausado) return;
        if (Input.GetKeyDown(teclaAgachar)) estaAgachado = !estaAgachado;
    }
}
