using System.Collections.Generic;
using UnityEngine;

public class Chave : MonoBehaviour
{
    public KeyCode teclaParaPegar = KeyCode.E;
    private readonly HashSet<Collider> contatos = new HashSet<Collider>();
    private InventarioPlayer player;
    private MovimentoPlayer movimento;
    private bool coletada;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        InventarioPlayer candidato = other.GetComponentInParent<InventarioPlayer>();
        if (candidato == null || (player != null && player != candidato)) return;
        contatos.Add(other);
        player = candidato;
        movimento = player.GetComponent<MovimentoPlayer>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!contatos.Remove(other) || contatos.Count != 0) return;
        player = null;
        movimento = null;
    }

    private void OnDisable()
    {
        contatos.Clear();
        player = null;
        movimento = null;
    }

    private void Update()
    {
        if (coletada || GerenciadorDePause.estaPausado || player == null) return;
        bool agachado = movimento != null ? movimento.EstaAgachado : player.estaAgachado;
        if ((movimento != null && !movimento.isActiveAndEnabled) || !agachado ||
            !Input.GetKeyDown(teclaParaPegar)) return;
        coletada = true;
        player.temChave = true;
        Destroy(gameObject);
    }
}
