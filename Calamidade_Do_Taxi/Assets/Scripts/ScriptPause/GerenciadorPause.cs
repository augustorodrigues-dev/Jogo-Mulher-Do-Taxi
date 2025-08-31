using UnityEngine;
using UnityEngine.SceneManagement; 

public class GerenciadorDePause : MonoBehaviour
{
    
    public GameObject painelDePause;

    
    public static bool estaPausado = false;

    void Start()
    {
        
        painelDePause.SetActive(false);
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
            {
                
                RetomarJogo();
            }
            else
            {
                
                PausarJogo();
            }
        }
    }

    
    void PausarJogo()
    {
        estaPausado = true;
        painelDePause.SetActive(true); 

        
        Time.timeScale = 0f;

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
    public void RetomarJogo()
    {
        estaPausado = false;
        painelDePause.SetActive(false); 

        
        Time.timeScale = 1f;

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    public void VoltarAoMenu()
    {
        
        Time.timeScale = 1f;
        
        
        SceneManager.LoadScene("Menu"); 
    }
}