using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteracaoGavetaCutscene : MonoBehaviour
{

    public bool gavetaFoiAberta = false;

    private readonly ControleScriptsCutscene controleScripts = new ControleScriptsCutscene();
    private static readonly WaitForSeconds pausaDramatica = new WaitForSeconds(4f);

    [Header("Configuração de Som")]
    [Tooltip("O componente AudioSource que vai tocar o som.")]
    public AudioSource somGavetaSource;

    [Tooltip("O arquivo de som (AudioClip) de abrindo a gaveta.")]
    public AudioClip somAbrindoGavetaJogador;

    public AudioSource somAbrindoGavetaSource;

    public AudioClip somGaveta;
    [Header("Configuração da Cutscene")]
    [Tooltip("Ponto exato para onde o jogador deve se mover.")]
    public Transform pontoAlvoPlayer;

    [Tooltip("Ponto exato para onde a câmera deve olhar.")]
    public Transform lookAtAlvo;

    [Tooltip("Arraste aqui TODOS os scripts do jogador que devem ser desativados durante a cutscene.")]
    public List<MonoBehaviour> scriptsParaDesativar;

    [Tooltip("A câmera principal do jogador.")]
    public Camera playerCamera;

    [Tooltip("Ponto secundário para onde a câmera deve olhar após a ação principal.")]
    public Transform lookAtAlvoSecundario;

    [Header("Configuração da Gaveta")]
    [Tooltip("O objeto da gaveta que será movido.")]
    public Transform gavetaTransform;

    [Tooltip("A distância que a gaveta vai abrir.")]
    public float distanciaParaAbrir = 0.33f;

    [Header("Velocidades")]
    public float velocidadeMovimento = 2f;
    public float velocidadeRotacao = 3f;
    public float velocidadeGaveta = 5f;

    public List<GameObject> objetosCutscene = new List<GameObject>();

    public GameObject painel;

    public List <GameObject> canvasTexto = new List<GameObject>();

    private bool jogadorPerto = false;
    private bool emCutscene = false;
    private GameObject playerObject;

    public GameObject mulherDoTaxi;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            playerObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            if (!emCutscene) playerObject = null;
        }
    }

    void Update()
    {
        if (pontoAlvoPlayer == null || lookAtAlvo == null || playerCamera == null || gavetaTransform == null)
            return;
        if (!GerenciadorDePause.estaPausado && jogadorPerto && !gavetaFoiAberta &&
            !emCutscene && playerObject != null && EntradaJogador.InteragirPressionado)
        {
            if (painel != null) painel.SetActive(false);
            foreach (GameObject textos in canvasTexto)
            {
                if (textos != null) textos.SetActive(false);
            }
            StartCoroutine(ExecutarCutscene());
            foreach (GameObject objetos in objetosCutscene) {
                if (objetos != null) objetos.SetActive(true);
            }
            // painelCutscene.SetActive(true);
        }
    }

    private IEnumerator ExecutarCutscene()
    {
        emCutscene = true;

        controleScripts.Desativar(scriptsParaDesativar, this);

        while ((playerObject.transform.position - pontoAlvoPlayer.position).sqrMagnitude > 0.0001f)
        {
            playerObject.transform.position = Vector3.MoveTowards(playerObject.transform.position, pontoAlvoPlayer.position, velocidadeMovimento * Time.deltaTime);
            yield return null;
        }

        Quaternion targetRotationGaveta = Quaternion.LookRotation(lookAtAlvo.position - playerCamera.transform.position);
        while (Quaternion.Angle(playerCamera.transform.rotation, targetRotationGaveta) > 0.1f)
        {
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotationGaveta, velocidadeRotacao * Time.deltaTime);
            yield return null;
        }

        if (somGavetaSource != null && somGaveta != null)
        {
            somGavetaSource.clip = somGaveta;
            somGavetaSource.loop = false;
            somGavetaSource.volume = 0.5f; // ajusta o volume aqui
            somGavetaSource.Play();
        }

        Vector3 posicaoFechada = gavetaTransform.position;
        Vector3 posicaoAberta = posicaoFechada + gavetaTransform.right * distanciaParaAbrir;
        float tempo = 0;

        while (tempo < 1)
        {
            gavetaTransform.position = Vector3.Lerp(posicaoFechada, posicaoAberta, tempo);
            tempo += Time.deltaTime * velocidadeGaveta;
            yield return null;
        }

        gavetaTransform.position = posicaoAberta;

        if (somAbrindoGavetaSource != null && somAbrindoGavetaJogador != null)
        {
            somAbrindoGavetaSource.clip = somAbrindoGavetaJogador;
            somAbrindoGavetaSource.loop = false;
            somAbrindoGavetaSource.Play();
        }

        // A "pausa dramática" que você mencionou
        yield return pausaDramatica;

        if (lookAtAlvoSecundario != null)
        {
            Quaternion targetRotationSecundaria = Quaternion.LookRotation(lookAtAlvoSecundario.position - playerCamera.transform.position);
            while (Quaternion.Angle(playerCamera.transform.rotation, targetRotationSecundaria) > 0.1f)
            {
                playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotationSecundaria, velocidadeRotacao * Time.deltaTime);
                yield return null;
            }
        }

        ControleScriptsCutscene.SincronizarCamera(playerObject, playerCamera);
        controleScripts.Restaurar();
        foreach (GameObject objetos in objetosCutscene)
        {
            if (objetos != null) objetos.SetActive(false);
        }
        if (mulherDoTaxi != null) mulherDoTaxi.SetActive(true);
        emCutscene = false;
        gavetaFoiAberta = true;
    }
    private void OnDisable()
    {
        if (!emCutscene) return;
        StopAllCoroutines();
        ControleScriptsCutscene.SincronizarCamera(playerObject, playerCamera);
        controleScripts.Restaurar();
        foreach (GameObject objeto in objetosCutscene)
            if (objeto != null) objeto.SetActive(false);
        emCutscene = false;
        jogadorPerto = false;
        playerObject = null;
    }
}
