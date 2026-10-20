using UnityEngine;
using UnityEngine.UI;

public class ControleVolume : MonoBehaviour
{
    public Slider BarraDeVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerPrefs.HasKey("volumeGlobal"))
        {
            PlayerPrefs.SetFloat("volumeGlobal", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    // Update is called once per frame
    public void MudarVolume()
    {
        AudioListener.volume = BarraDeVolume.value;
        Save();
    }
    private void Load()
    {
        BarraDeVolume.value = PlayerPrefs.GetFloat("volumeGlobal");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("volumeGlobal", BarraDeVolume.value);
    }
}
