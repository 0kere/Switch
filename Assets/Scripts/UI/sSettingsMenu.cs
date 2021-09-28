using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Linq;

public class sSettingsMenu : MonoBehaviour
{
    [Header("Screen Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    List<Resolution> resolution = new List<Resolution>();
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Sound Settings")]
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private Slider musicVolSlider;
    [SerializeField] private TextMeshProUGUI volumePercentageText;

    [Header("Game Settings")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TextMeshProUGUI speedValueText;

    [SerializeField] private Button resetButton;
    [SerializeField] private TMP_InputField customLevelDataPath;
    void Awake()
    {
        resolution = Screen.resolutions.ToList(); //adapted from (Alienmadness, 2017)
        resolution = resolution.Distinct().ToList();
        resolutionDropDown.onValueChanged.AddListener(delegate { Screen.SetResolution(resolution[resolutionDropDown.value].width, resolution[resolutionDropDown.value].height, fullScreenToggle.isOn); });
        for (int i = 0; i < resolution.Count; i++)
        {
            resolutionDropDown.options[i].text = resolution[i].width + "x" + resolution[i].height;
            resolutionDropDown.value = i;
            if (!resolutionDropDown.options.Contains(new TMP_Dropdown.OptionData(resolutionDropDown.options[i].text)))
            { 
                resolutionDropDown.options.Add(new TMP_Dropdown.OptionData(resolutionDropDown.options[i].text));
            }
        }
        fullScreenToggle.onValueChanged.AddListener(delegate { ToggleFullScreen(); });
        musicVolSlider.onValueChanged.AddListener(delegate { UpdateVolumeSlider(musicVolSlider.value); });
        speedSlider.onValueChanged.AddListener(delegate { UpdateSpeedSlider(speedSlider.value); });
        resetButton.onClick.AddListener(delegate{ ResetPressed(); });

        if (customLevelDataPath is object)
        { 
            customLevelDataPath.text = Application.persistentDataPath;
        }

    }

    private void Start()
    {
        SettingsData curSettings = SettingsDataHandler.LoadData();
        UpdateSettings(curSettings);
        
    }
    private void UpdateSpeedSlider(float value)
    {
        GameManger.instance.playerSpeed = value;
        speedValueText.text = value.ToString();
    }

    private void UpdateVolumeSlider(float value)
    {
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        volumePercentageText.text = Mathf.Round((value / musicVolSlider.maxValue) * 100f).ToString() + "%";
    }

    private void ToggleFullScreen()
    {
        if (fullScreenToggle.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    private void ResetPressed()
    {
        SettingsData def = SettingsDataHandler.defaultSettings;
        UpdateSettings(def);
    }

    private void OnEnable() //needs to set the players current setting values
    {
        //SettingsData curSettings = SettingsDataHandler.LoadData();
        //UpdateSettings(curSettings);
    }

    private void OnDisable()
    {
        Vector2 curRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        SettingsData curSettings = new SettingsData(curRes, fullScreenToggle.isOn, musicVolSlider.value, speedSlider.value);
        SettingsDataHandler.SaveData(curSettings);
    }

    private void UpdateSettings(SettingsData data)
    {
        if (Screen.currentResolution.height != (int)data.resolution.y && Screen.currentResolution.width != (int)data.resolution.x) //dont update res if it hasnt changed. 
        { //There is currently an issue with the screen flickering black when you first open the settings menu in a scene as this gets set. It doesnt happen again till the scene is reloaded which would suggest this statement helps
            //just unsure on how to get rid of the initial flicker
            Screen.SetResolution((int)data.resolution.x, (int)data.resolution.y, data.isFullScreen);
        }
        int resDropValue = resolution.IndexOf(Screen.currentResolution);
        resolutionDropDown.value = resDropValue;
        fullScreenToggle.isOn = data.isFullScreen;
        UpdateSpeedSlider(data.speedValue);
        speedSlider.value = data.speedValue;
        UpdateVolumeSlider(data.volumeValue);
        musicVolSlider.value = data.volumeValue;
        GameManger.instance.UpdateSpeed(data.speedValue);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
