using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutscenePerseguicao : MonoBehaviour
{

    public InteracaoGavetaCutscene interacaoGaveta;
    public MovimentoPlayer movimentoPlayer;


    [Header("Player")]
    public GameObject playerObject;
    public Camera playerCamera;
    public List<MonoBehaviour> scriptsParaDesativar; // scripts que controlam o player

    [Header("Cutscene Trigger")]
    public Collider triggerCutscene;

    [Header("Mulher da Cena")]
    public Transform mulherTransform;
    public Animator mulherAnimator; // se tiver animações
    public float velocidadeGiroMulher = 1f;
    public float velocidadeAvancoMulher = 6f;

    [Header("Cutscene Config")]
    public Transform pontoFrentePlayer; // ponto até onde o player anda
    public Transform direcaoMulher; // onde o player vai olhar (posição da mulher)
    public float velocidadeMovimentoPlayer = 2f;
    public float velocidadeRotacaoCamera = 2f;

    [Header("Luzes")]
    public List<Light> luzesPiscar = new List<Light>();
    public float tempoPiscar = 0.1f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somOlhar;
    public AudioClip somGrito;
    public AudioClip efeitoSusto;

    [Header("Collider de Morte")]
    public GameObject colliderMortePrefab; // prefab com collider + script de morte
    private GameObject colliderMorteInstance;

    private bool cutsceneAtiva = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!cutsceneAtiva && other.CompareTag("Player") && interacaoGaveta.gavetaFoiAberta)
        {
            cutsceneAtiva = true;
            StartCoroutine(ExecutarCutscene());
        }
    }

    private IEnumerator ExecutarCutscene()
    {
        // Desativar controles do player
        foreach (var script in scriptsParaDesativar)
            script.enabled = false;

        if (movimentoPlayer != null)
        {
            movimentoPlayer.desativarPassos = true;
        }

        // Player anda para frente
        while (Vector3.Distance(playerObject.transform.position, pontoFrentePlayer.position) > 0.05f)
        {
            playerObject.transform.position = Vector3.MoveTowards(
                playerObject.transform.position,
                pontoFrentePlayer.position,
                velocidadeMovimentoPlayer * Time.deltaTime
            );
            yield return null;
        }

        // Player olha para a mulher
        Quaternion targetRotation = Quaternion.LookRotation(direcaoMulher.position - playerCamera.transform.position);
        while (Quaternion.Angle(playerCamera.transform.rotation, targetRotation) > 0.5f)
        {
            playerCamera.transform.rotation = Quaternion.Slerp(
                playerCamera.transform.rotation,
                targetRotation,
                velocidadeRotacaoCamera * Time.deltaTime
            );
            yield return null;
        }

        // Tocar som de olhar
        if (somOlhar != null) audioSource.PlayOneShot(somOlhar);

        yield return new WaitForSeconds(1.5f);

        // Mulher se vira devagar
        Quaternion mulherTargetRot = Quaternion.LookRotation(playerObject.transform.position - mulherTransform.position);
        while (Quaternion.Angle(mulherTransform.rotation, mulherTargetRot) > 1f)
        {
            mulherTransform.rotation = Quaternion.Slerp(
                mulherTransform.rotation,
                mulherTargetRot,
                velocidadeGiroMulher * Time.deltaTime
            );
            yield return null;
        }

        // Luzes piscam
        StartCoroutine(PiscarLuzes());

        yield return new WaitForSeconds(1f);

        // Mulher avança na direção da câmera (jumpscare)
        if (somGrito != null) audioSource.PlayOneShot(somGrito);
        if (efeitoSusto != null) audioSource.PlayOneShot(efeitoSusto);

        Vector3 alvoCamera = playerCamera.transform.position + playerCamera.transform.forward * 2.0f;
        alvoCamera.y = mulherTransform.position.y; // mantém no nível da mulher (chão)
        while (Vector3.Distance(mulherTransform.position, alvoCamera) > 0.1f)
        {
            mulherTransform.position = Vector3.MoveTowards(
                mulherTransform.position,
                alvoCamera,
                velocidadeAvancoMulher * Time.deltaTime
            );
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Player cai no chão (simulação de rotação da câmera)
        Quaternion rotacaoChao = Quaternion.Euler(70, playerCamera.transform.rotation.eulerAngles.y, 0);
        float t = 0;
        while (t < 1)
        {
            playerCamera.transform.rotation = Quaternion.Slerp(
                playerCamera.transform.rotation,
                rotacaoChao,
                t
            );
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Player levanta novamente
        Quaternion rotacaoLevantado = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
        t = 0;
        while (t < 1)
        {
            playerCamera.transform.rotation = Quaternion.Slerp(
                playerCamera.transform.rotation,
                rotacaoLevantado,
                t
            );
            t += Time.deltaTime;
            yield return null;
        }

        // Reativar controles do player
        foreach (var script in scriptsParaDesativar)
            script.enabled = true;

        if (movimentoPlayer != null)
{
        movimentoPlayer.desativarPassos = false;
}

        // Criar collider de morte na frente da mulher
        //colliderMorteInstance = Instantiate(colliderMortePrefab, mulherTransform.position, mulherTransform.rotation);
        //colliderMorteInstance.transform.SetParent(mulherTransform);
    }

    private IEnumerator PiscarLuzes()
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (var luz in luzesPiscar)
                luz.enabled = !luz.enabled;
            yield return new WaitForSeconds(tempoPiscar);
        }
        foreach (var luz in luzesPiscar)
            luz.enabled = true; // volta ao normal
    }
}
