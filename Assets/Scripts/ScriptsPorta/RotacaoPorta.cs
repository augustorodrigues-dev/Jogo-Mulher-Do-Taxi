using UnityEngine;

public class RotacaoPorta : MonoBehaviour
{
    private bool emMovimento;
    private bool isOpen = false;
    private bool podeAbrir = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    public float rotationSpeed = 2f;
    public float openAngle = 120f;
    public Collider doorCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            podeAbrir = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            podeAbrir = false;
    }

    void Start()
    {
        if (doorCollider != null) doorCollider.enabled = true;
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        if (GerenciadorDePause.estaPausado) return;

        if (podeAbrir && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            emMovimento = true;
            if (doorCollider != null) doorCollider.enabled = !isOpen;
        }

        if (!emMovimento) return;
        Quaternion alvo = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, alvo, Time.deltaTime * rotationSpeed);
        if (Quaternion.Angle(transform.rotation, alvo) <= 0.1f)
        {
            transform.rotation = alvo;
            emMovimento = false;
        }
    }
}
