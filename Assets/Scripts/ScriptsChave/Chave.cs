
using UnityEngine;

public class Chave : MonoBehaviour
{
    public KeyCode teclaParaPegar = KeyCode.E; 

    private void OnTriggerStay(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            
            InventarioPlayer player = other.GetComponent<InventarioPlayer>();

            
            if (player != null && player.estaAgachado && Input.GetKeyDown(teclaParaPegar)) 
            {
                
                player.temChave = true;

                Debug.Log("Você pegou a chave agachado pressionando " + teclaParaPegar.ToString() + "!");

               
                Destroy(gameObject);
            }
        }
    }
}