using UnityEngine;

public class RotacaoPorta : MonoBehaviour
{
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
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        if (podeAbrir && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        if (isOpen)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * rotationSpeed);
            if (doorCollider != null) doorCollider.enabled = false; 
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * rotationSpeed);
            if (doorCollider != null) doorCollider.enabled = true;  
        }
    }
}
