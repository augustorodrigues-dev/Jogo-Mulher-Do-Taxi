using UnityEngine;
using System.Collections;
using System.Collections.Generic; // PASSO 1: Adicionado para usar Listas

public class InteracaoGavetaCutscene : MonoBehaviour
{
    [Header("Configuração da Cutscene")]
    [Tooltip("Ponto exato para onde o jogador deve se mover.")]
    public Transform pontoAlvoPlayer;
    
    [Tooltip("Ponto exato para onde a câmera deve olhar.")]
    public Transform lookAtAlvo;

    // PASSO 2: Trocamos a variável única por uma lista de scripts
    [Tooltip("Arraste aqui TODOS os scripts do jogador que devem ser desativados durante a cutscene.")]
    public List<MonoBehaviour> scriptsParaDesativar; 

    [Tooltip("A câmera principal do jogador.")]
    public Camera playerCamera;

    // Adicione esta linha junto com as outras variáveis de "Configuração da Cutscene"
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

    public GameObject painel;

    private bool jogadorPerto = false;
    private bool emCutscene = false;
    private GameObject playerObject;


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
            playerObject = null;
        }
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E) && !emCutscene)
        {
            painel.SetActive(false);
            StartCoroutine(ExecutarCutscene());
        }
    }

    

private IEnumerator ExecutarCutscene()
{
    emCutscene = true;
    
    
    foreach (var script in scriptsParaDesativar)
    {
        script.enabled = false;
    }

    
    while (Vector3.Distance(playerObject.transform.position, pontoAlvoPlayer.position) > 0.01f)
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

    
    yield return new WaitForSeconds(1.5f);

    if (lookAtAlvoSecundario != null) 
    {
        Quaternion targetRotationSecundaria = Quaternion.LookRotation(lookAtAlvoSecundario.position - playerCamera.transform.position);
        while (Quaternion.Angle(playerCamera.transform.rotation, targetRotationSecundaria) > 0.1f)
        {
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotationSecundaria, velocidadeRotacao * Time.deltaTime);
            yield return null;
        }
    }

    
    MovimentoPlayer playerScript = playerObject.GetComponent<MovimentoPlayer>(); 

    if (playerScript != null)
    {
        Quaternion finalWorldRotation = playerCamera.transform.rotation;
        playerObject.transform.rotation = Quaternion.Euler(0, finalWorldRotation.eulerAngles.y, 0);

        float finalRotationX = finalWorldRotation.eulerAngles.x;
        if (finalRotationX > 180) { finalRotationX -= 360; }
        
        playerScript.rotationX = finalRotationX;
    }
    
    foreach (var script in scriptsParaDesativar)
    {
        script.enabled = true;
    }
    emCutscene = false;
    }
}