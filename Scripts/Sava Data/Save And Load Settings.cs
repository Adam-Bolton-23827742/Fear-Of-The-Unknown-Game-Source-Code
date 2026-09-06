using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveAndLoadSettings : MonoBehaviour
{
    [SerializeField] private Options SFX, ShowFPS, FPSSlider, VsyncToggle, Master, AntiAliasing, Resolution;

    private void Start()
    {
        // sets the timescale to prevent the game from being paused after going back to the main menu from the pause menu
        Time.timeScale = 1.0f;

        // Load the player's saved settings if they have a save file
        if (File.Exists(Application.persistentDataPath + "/OptionData.json"))
        {
            LoadData();
        }

        // Load the default settings if they do not have a save file
        else
        {
            //sets default fps and screen resolution
            Application.targetFrameRate = 60;
            Screen.SetResolution(1920, 1080, true);

            // Links functions to be executed when the values of these Ui objects change to apply new settings
            FPSSlider.Slider.onValueChanged.AddListener(FPSSlider.MaxFPS);
            VsyncToggle.Toggle.onValueChanged.AddListener(VsyncToggle.VSync);
            Master.Slider.onValueChanged.AddListener(Master.MasterVolume);
            SFX.Slider.onValueChanged.AddListener(SFX.SFXVolume);
            ShowFPS.Toggle.onValueChanged.AddListener(ShowFPS.ShowFPS);
            AntiAliasing.Dropdown.onValueChanged.AddListener(AntiAliasing.AntiAliasing);
            Resolution.Dropdown.onValueChanged.AddListener(Resolution.ScreenResolution);
        }
    }

    public void SaveData()
    {
        // Sets the values inside an instance of the settings data class
        SettingsData Settings = new SettingsData();
        Settings.MasterVolume = AudioListener.volume;
        Settings.SFXVolume = SFX.Slider.value;
        Settings.TargetFPS = Application.targetFrameRate;
        Settings.VSyncValue = QualitySettings.vSyncCount;
        Settings.CanShowFPS = ShowFPS.CanShowFPS;
        Settings.AntiAliasing = QualitySettings.antiAliasing;
        Settings.AntiAliasingValue = AntiAliasing.Dropdown.value;
        Settings.ResolutionValue = Resolution.Dropdown.value;

        // Saves the data into a json file onto the computer
        string Json = JsonUtility.ToJson(Settings);
        File.WriteAllText(Application.persistentDataPath + "/OptionData.json", Json);
        Debug.Log(Application.persistentDataPath + "/OptionData.json");
    }

    // Initializes all of the settings variables with the data stored inside the json file
    public void LoadData()
    {
        // Loads the json file into a variable
        SettingsData Settings = JsonUtility.FromJson<SettingsData>(File.ReadAllText(Application.persistentDataPath + "/OptionData.json"));

        if (SceneManager.GetActiveScene().name != "Credits")
        {
            Application.targetFrameRate = Settings.TargetFPS;
        }

        else
        {
            Application.targetFrameRate = 60;
        }

        if (FPSSlider != null)
        {
            FPSSlider.Slider.value = Settings.TargetFPS;
            // Adds on value changed functions after changing the UI object values to prevent these functions from being executed when setting these UI values
            FPSSlider.Slider.onValueChanged.AddListener(FPSSlider.MaxFPS);
            FPSSlider.Value.text = Settings.TargetFPS.ToString();
        }
        
        QualitySettings.vSyncCount = Settings.VSyncValue;

        if (VsyncToggle != null)
        {
            if (QualitySettings.vSyncCount == 0)
            {
                VsyncToggle.Toggle.isOn = false;
            }

            else
            {
                VsyncToggle.Toggle.isOn = true;
                FPSSlider.Slider.interactable = false;
            }

            VsyncToggle.Toggle.onValueChanged.AddListener(VsyncToggle.VSync);
        }

        AudioListener.volume = Settings.MasterVolume;

        if (Master != null)
        {
            Master.Slider.value = Settings.MasterVolume;
            Master.Slider.onValueChanged.AddListener(Master.MasterVolume);
            Master.SetAudioText();
        }

        if (SFX != null)
        {
            SFX.Slider.value = Settings.SFXVolume;
            SFX.Slider.onValueChanged.AddListener(SFX.SFXVolume);
            SFX.SFXVolume(0);
            SFX.SetAudioText();
        }

        if (ShowFPS != null)
        {
            ShowFPS.CanShowFPS = Settings.CanShowFPS;

            if (ShowFPS.CanShowFPS)
            {
                ShowFPS.Toggle.isOn = true;
                ShowFPS.FPSCounter.gameObject.SetActive(true);
            }

            else
            {
                ShowFPS.Toggle.isOn = false;
            }

            ShowFPS.Toggle.onValueChanged.AddListener(ShowFPS.ShowFPS);
        }

        StartCoroutine(AntiAliasingDelay(Settings));

        if (AntiAliasing != null)
        {
            AntiAliasing.Dropdown.value = Settings.AntiAliasingValue;
            AntiAliasing.Dropdown.onValueChanged.AddListener(AntiAliasing.AntiAliasing);
            Resolution.Dropdown.value = Settings.ResolutionValue;
            Resolution.Dropdown.onValueChanged.AddListener(Resolution.ScreenResolution);
        }
    }

    // A delay that sets the antyi alisasing to match the anti aliasing value stored inside the json file. This is to overwrite Unity setting the value to 0 on start.
    private IEnumerator AntiAliasingDelay(SettingsData Settings)
    {
        yield return new WaitForEndOfFrame();
        QualitySettings.antiAliasing = Settings.AntiAliasing;
    }
}

//Serializable class containing all of the player's settings data to be saved or loaded
[Serializable]
public class SettingsData
{
    public float MasterVolume, SFXVolume;
    public int VSyncValue, AntiAliasingValue, TargetFPS, ResolutionValue, AntiAliasing;
    public bool CanShowFPS;
}
