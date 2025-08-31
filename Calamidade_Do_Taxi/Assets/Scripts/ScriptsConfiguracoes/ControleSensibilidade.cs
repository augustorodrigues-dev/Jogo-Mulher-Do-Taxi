using UnityEngine;
using UnityEngine.UI;

public class ControleSensibilidade : MonoBehaviour
{
    public Slider BarraSensibilidade;
    void Start()
    {
        if (PlayerPrefs.HasKey("sensibilidadeGlobal"))
        {
            PlayerPrefs.SetFloat("sensibilidadeGlobal", 1f);
            load();
        }
        else
        {
            load();
        }
    }

    public void MudarSensibilidade()
    {
        SalvarSensibilidade();
    }
    public void SalvarSensibilidade()
    {
        PlayerPrefs.SetFloat("sensibilidadeGlobal", BarraSensibilidade.value);
    }

    public void load()
    {
        if (!PlayerPrefs.HasKey("sensibilidadeGlobal"))
        {
            PlayerPrefs.SetFloat("sensibilidadeGlobal", 2f);
        }
        BarraSensibilidade.value = PlayerPrefs.GetFloat("sensibilidadeGlobal");
    }
    
}
