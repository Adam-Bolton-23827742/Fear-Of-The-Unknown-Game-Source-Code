using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Slider Slider;
    [SerializeField] private Slider FPSSlider;
    public TMP_Dropdown Dropdown;
    public Toggle Toggle;
    public TextMeshProUGUI Value, FPSCounter;
    [HideInInspector] public bool CanShowFPS;

    public void SetAudioText()
    {
        //Rounds the slider value and * 10 for readability
        float SliderValue = Mathf.Round(Slider.value * 10f);
        Value.text = SliderValue.ToString();
    }

    public void MasterVolume(float Float)
    {
        //Sets the audio listeners volume to the slider value
        AudioListener.volume = Slider.value;
        SetAudioText();
    }

    public void SFXVolume(float Float)
    {
        GameObject[] All = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject Object in All)
        {
            if (Object.GetComponent<AudioSource>() != null && Object.tag != "Music")
            {
                Object.GetComponent<AudioSource>().volume = Slider.value;
            }
        }

        SetAudioText();
    }

    public void MaxFPS(float Float)
    {
        //Sets the target frame rate as the slider value. I converted to int because the fps cannot be a float value.
        Application.targetFrameRate = Convert.ToInt32(Slider.value);
        Value.text = Slider.value.ToString();
    }

    public void VSync(bool Bool)
    {
        if (Toggle.isOn)
        {
            //Turns vsync on
            QualitySettings.vSyncCount = 1;
            FPSSlider.interactable = false;

            // Sets the fps slider value to match the refresh rate. I converted to float as slider values cannot be initialized as double.
            FPSSlider.value = (float)Screen.currentResolution.refreshRateRatio.value;
        }
        
        else
        {
            //Turns vsync off
            QualitySettings.vSyncCount = 0;
            FPSSlider.interactable = true;
        }
    }

    public void ScreenResolution(int Int)
    {
        if (Dropdown.value == 0)
        {
            //480p
            Screen.SetResolution(640, 480, true);
        }

        else if (Dropdown.value == 1)
        {
            //720p
            Screen.SetResolution(1280, 720, true);
        }

        else if (Dropdown.value == 2)
        {
            //1080p
            Screen.SetResolution(1920, 1080, true);
        }

        else if (Dropdown.value == 3)
        {
            //2k
            Screen.SetResolution(2560, 1440, true);
        }
        else
        {
            //4k
            Screen.SetResolution(3840, 2160, true);
        }
    }

    public void AntiAliasing(int Int)
    {
        if (Dropdown.value == 0)
        {
            //No ant aliasing
            QualitySettings.antiAliasing = 0;
        }

        else if (Dropdown.value == 1)
        {
            //2X ant aliasing
            QualitySettings.antiAliasing = 2;
        }

        else if (Dropdown.value == 2)
        {
            //4X ant aliasing
            QualitySettings.antiAliasing = 4;
        }

        else
        {
            //8X ant aliasing
            QualitySettings.antiAliasing = 8;
        }
    }

    public void ShowFPS(bool Bool)
    {
        if (Toggle.isOn)
        {
            CanShowFPS = true;
            FPSCounter.gameObject.SetActive(true);
        }

        else
        {
            CanShowFPS = false;
            FPSCounter.gameObject.SetActive(false);
        }
    }
}
