using UnityEngine;

public class BotaoNaTela : MonoBehaviour
{
    public GameObject painelInstrução;
    public Collider localColisao;

    bool estaNaArea;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            estaNaArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            estaNaArea = false;
    }
    void Start()
    {
        estaNaArea = false;
    }


    void Update()
    {
        if (estaNaArea)
        {
            painelInstrução.SetActive(true);
        }
        else
        {
            painelInstrução.SetActive(false);   
        }
    }
}
