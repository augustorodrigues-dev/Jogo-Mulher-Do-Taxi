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
    public List<MonoBehaviour> scriptsParaDesativar;

    [Header("Cutscene Trigger")]
    public Collider triggerCutscene;

    [Header("Mulher da Cena")]
    public Transform mulherTransform;
    public Animator mulherAnimator;
    public float velocidadeGiroMulher = 1f;
    public float velocidadeAvancoMulher = 6f;

    [Header("Cutscene Config")]
    public Transform pontoFrentePlayer;
    public Transform direcaoMulher;
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
    public GameObject colliderMortePrefab;
    private GameObject colliderMorteInstance;

    private readonly ControleScriptsCutscene controleScripts = new ControleScriptsCutscene();
    private static readonly WaitForSeconds esperaUmSegundo = new WaitForSeconds(1f);
    private static readonly WaitForSeconds esperaOlhar = new WaitForSeconds(1.5f);
    private bool executando;
    private bool passosAnteriores;
    private bool[] estadoLuzes;
    private bool cutsceneAtiva = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!GerenciadorDePause.estaPausado && !cutsceneAtiva && other.CompareTag("Player") &&
            interacaoGaveta != null && interacaoGaveta.gavetaFoiAberta &&
            playerObject != null && playerCamera != null && mulherTransform != null &&
            pontoFrentePlayer != null && direcaoMulher != null)
        {
            cutsceneAtiva = true;
            StartCoroutine(ExecutarCutscene());
        }
    }

    private IEnumerator ExecutarCutscene()
    {

        executando = true;
        passosAnteriores = movimentoPlayer != null && movimentoPlayer.desativarPassos;
        controleScripts.Desativar(scriptsParaDesativar, this);

        if (movimentoPlayer != null)
        {
            movimentoPlayer.desativarPassos = true;
        }

        while ((playerObject.transform.position - pontoFrentePlayer.position).sqrMagnitude > 0.0025f)
        {
            playerObject.transform.position = Vector3.MoveTowards(
                playerObject.transform.position,
                pontoFrentePlayer.position,
                velocidadeMovimentoPlayer * Time.deltaTime
            );
            yield return null;
        }

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

        if (audioSource != null && somOlhar != null) audioSource.PlayOneShot(somOlhar);

        yield return esperaOlhar;

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

        StartCoroutine(PiscarLuzes());

        yield return esperaUmSegundo;

        if (audioSource != null && somGrito != null) audioSource.PlayOneShot(somGrito);
        if (audioSource != null && efeitoSusto != null) audioSource.PlayOneShot(efeitoSusto);

        Vector3 alvoCamera = playerCamera.transform.position + playerCamera.transform.forward * 2.0f;
        alvoCamera.y = mulherTransform.position.y;
        while ((mulherTransform.position - alvoCamera).sqrMagnitude > 0.01f)
        {
            mulherTransform.position = Vector3.MoveTowards(
                mulherTransform.position,
                alvoCamera,
                velocidadeAvancoMulher * Time.deltaTime
            );
            yield return null;
        }

        yield return esperaUmSegundo;

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

        yield return esperaUmSegundo;

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

        RestaurarControle();

        //colliderMorteInstance = Instantiate(colliderMortePrefab, mulherTransform.position, mulherTransform.rotation);
        //colliderMorteInstance.transform.SetParent(mulherTransform);
    }

    private IEnumerator PiscarLuzes()
    {
        if (estadoLuzes == null || estadoLuzes.Length != luzesPiscar.Count)
            estadoLuzes = new bool[luzesPiscar.Count];
        for (int i = 0; i < luzesPiscar.Count; i++)
            estadoLuzes[i] = luzesPiscar[i] != null && luzesPiscar[i].enabled;
        // Uma espera por execução, reutilizada nas dez alternâncias.
        WaitForSeconds espera = new WaitForSeconds(Mathf.Max(0f, tempoPiscar));
        for (int i = 0; i < 10; i++)
        {
            foreach (Light luz in luzesPiscar)
                if (luz != null) luz.enabled = !luz.enabled;
            yield return espera;
        }
        RestaurarLuzes();
    }

    private void RestaurarLuzes()
    {
        if (estadoLuzes == null) return;
        for (int i = 0; i < luzesPiscar.Count && i < estadoLuzes.Length; i++)
            if (luzesPiscar[i] != null) luzesPiscar[i].enabled = estadoLuzes[i];
    }

    private void RestaurarControle()
    {
        ControleScriptsCutscene.SincronizarCamera(playerObject, playerCamera);
        if (movimentoPlayer != null) movimentoPlayer.desativarPassos = passosAnteriores;
        controleScripts.Restaurar();
        executando = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        RestaurarLuzes();
        if (executando) RestaurarControle();
    }
}
