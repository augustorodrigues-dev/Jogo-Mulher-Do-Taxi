using UnityEngine;

public class BotaoNaTela : MonoBehaviour
{
    public GameObject painelInstrução;
    public Collider localColisao;
    private int contatos;

    private void Start() => Mostrar(false);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && ++contatos == 1) Mostrar(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        contatos = Mathf.Max(0, contatos - 1);
        if (contatos == 0) Mostrar(false);
    }

    private void OnDisable()
    {
        contatos = 0;
        Mostrar(false);
    }

    private void Mostrar(bool visivel)
    {
        if (painelInstrução != null && painelInstrução.activeSelf != visivel)
            painelInstrução.SetActive(visivel);
    }
}
