using UnityEngine;

public class PegarLanterna : MonoBehaviour
{
    public GameObject lanternaNaMao;


    private bool podePegar = false;

    private GameObject lanternaNoChao;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            podePegar = true;
            lanternaNoChao = this.gameObject;

        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            podePegar = false;

        }
    }

    private void Update()
    {
        if (GerenciadorDePause.estaPausado) return;

        if (podePegar && EntradaJogador.InteragirPressionado)
        {

            Pegar();
        }
    }

    private void Pegar()
    {

        if (lanternaNaMao != null)
        {
            lanternaNaMao.SetActive(true);
        }

        if (lanternaNoChao != null)
        {
            Destroy(lanternaNoChao);
        }

    }
}
