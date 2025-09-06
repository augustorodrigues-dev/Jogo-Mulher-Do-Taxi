using UnityEngine;

public class Chave : MonoBehaviour
{
    
    private void OnTriggerStay(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            InventarioPlayer player = other.GetComponent<InventarioPlayer>();

            
            if (player != null && player.estaAgachado)
            {
                
                player.temChave = true;

                Debug.Log("Você pegou a chave agachado!");

                
                Destroy(gameObject);
            }
        }
    }
}