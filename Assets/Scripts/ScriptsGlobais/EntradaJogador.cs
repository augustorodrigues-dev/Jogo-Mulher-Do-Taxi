using UnityEngine;
using UnityEngine.InputSystem;

// Compartilha as ações do projeto entre os componentes, inclusive durante a pausa.
public static class EntradaJogador
{
    private static InputAction mover, olhar, correr, agachar, pular, interagir, lanterna, pausa;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Inicializar()
    {
        InputActionMap mapa = InputSystem.actions.FindActionMap("Player", true);
        mover = mapa.FindAction("Move", true);
        olhar = mapa.FindAction("Look", true);
        correr = mapa.FindAction("Sprint", true);
        agachar = mapa.FindAction("Crouch", true);
        pular = mapa.FindAction("Jump", true);
        interagir = mapa.FindAction("Interact", true);
        lanterna = mapa.FindAction("Lanterna", true);
        pausa = mapa.FindAction("Pause", true);
        mapa.Enable();
    }

    public static Vector2 Movimento => Vector2.ClampMagnitude(mover.ReadValue<Vector2>(), 1f);
    public static bool CorrerSegurado => correr.IsPressed();
    public static bool AgacharSegurado => agachar.IsPressed();
    public static bool AgacharPressionado => agachar.WasPressedThisFrame();
    public static bool PularPressionado => pular.WasPressedThisFrame();
    public static bool InteragirPressionado => interagir.WasPressedThisFrame();
    public static bool LanternaPressionada => lanterna.WasPressedThisFrame();
    public static bool PausaPressionada => pausa.WasPressedThisFrame();

    public static Vector2 Olhar
    {
        get
        {
            Vector2 valor = olhar.ReadValue<Vector2>();
            // O mouse já entrega deslocamento por quadro. Sticks entregam velocidade.
            return olhar.activeControl?.device is Pointer
                ? valor * 0.1f
                : valor * (120f * Time.unscaledDeltaTime);
        }
    }
}
