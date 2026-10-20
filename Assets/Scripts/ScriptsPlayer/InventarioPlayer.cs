using UnityEngine;

public class InventarioPlayer : MonoBehaviour
{
    public bool temChave = false;
    public bool estaAgachado = false; 

    public KeyCode teclaAgachar = KeyCode.LeftControl; 

    
    void Start()
    {
        
    }

    void Update()
{
    
    if (Input.GetKeyDown(teclaAgachar))
    {
        
        estaAgachado = !estaAgachado;

    }
}
}