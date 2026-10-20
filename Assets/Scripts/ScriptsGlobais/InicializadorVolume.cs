using UnityEngine;

public class InicializadorVolume : MonoBehaviour
{
    void Awake() 
    {
        float volumeSalvo = PlayerPrefs.GetFloat("volumeGlobal", 1f);
        AudioListener.volume = volumeSalvo;
    }
}