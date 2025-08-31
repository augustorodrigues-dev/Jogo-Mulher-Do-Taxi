using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lanterna : MonoBehaviour
{
    public GameObject ON;
    public GameObject OFF;
    
    public AudioSource SomLanterna; 
    private bool isON;

    void Start()
    {
        
        ON.SetActive(false);
        OFF.SetActive(true);
        isON = false;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            
            isON = !isON; 
            
            
            if (SomLanterna != null)
            {
                SomLanterna.Play();
            }

            
            ON.SetActive(isON);
            OFF.SetActive(!isON);
        }
    }
}