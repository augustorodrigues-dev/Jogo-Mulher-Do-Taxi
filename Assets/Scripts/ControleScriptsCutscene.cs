using System.Collections.Generic;
using UnityEngine;

// Guarda somente os componentes que esta cutscene realmente desativou.
// A lista reutiliza sua capacidade entre execuções.
internal sealed class ControleScriptsCutscene
{
    private readonly List<MonoBehaviour> desativados = new List<MonoBehaviour>();

    public void Desativar(List<MonoBehaviour> scripts, MonoBehaviour dono)
    {
        Restaurar();
        if (scripts == null) return;
        foreach (MonoBehaviour script in scripts)
        {
            if (script == null || script == dono || !script.enabled) continue;
            desativados.Add(script);
            script.enabled = false;
        }
    }

    public void Restaurar()
    {
        foreach (MonoBehaviour script in desativados)
            if (script != null) script.enabled = true;
        desativados.Clear();
    }

    public static void SincronizarCamera(GameObject player, Camera camera)
    {
        if (player == null || camera == null) return;
        MovimentoPlayer movimento = player.GetComponent<MovimentoPlayer>();
        if (movimento == null) return;
        Quaternion rotacao = camera.transform.rotation;
        player.transform.rotation = Quaternion.Euler(0f, rotacao.eulerAngles.y, 0f);
        float pitch = Mathf.DeltaAngle(0f, rotacao.eulerAngles.x);
        movimento.rotationX = Mathf.Clamp(pitch, -movimento.lookXLimit, movimento.lookXLimit);
        camera.transform.localRotation = Quaternion.Euler(movimento.rotationX, 0f, 0f);
    }
}
