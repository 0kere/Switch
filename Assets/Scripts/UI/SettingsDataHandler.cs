using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData
{
    public Vector2 resolution;
    public bool isFullScreen;
    public float volumeValue;
    public float speedValue;
    public SettingsData() { }
    public SettingsData(Vector2 res, bool fullScreen, float volVal, float speedVal) { resolution = res; isFullScreen = fullScreen; volumeValue = volVal; speedValue = speedVal; }
}

public class SettingsDataHandler : MonoBehaviour //handle saving settings data into playerprefs
{
    private static SettingsDataHandler sdh;
    public static SettingsDataHandler instance
    {
        get 
        {
            if (!sdh)
            {
                sdh = FindObjectOfType<SettingsDataHandler>();
                if (!sdh)
                {
                    Debug.LogWarning("No Settings Data Handler in scene");
                }
                else
                {
                    instance.Init();
                }
            }
            return sdh;
        }
    }
    private void Init() { }
    public static SettingsData defaultSettings = new SettingsData(new Vector2(1920,1080), true, 1, 250);
    public static SettingsData LoadData()
    {
        SettingsData loadData = new SettingsData();
        if (!PlayerPrefs.HasKey("VolumeValue"))
        {
            loadData = defaultSettings; //if it doesnt contain a key then the player is yet to save settings so return default
        }
        else
        { 
            loadData.isFullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen"));
            loadData.resolution = new Vector2(PlayerPrefs.GetFloat("resWidth"), PlayerPrefs.GetFloat("resHeight"));
            loadData.speedValue = PlayerPrefs.GetFloat("SpeedValue");
            loadData.volumeValue = PlayerPrefs.GetFloat("VolumeValue");
        }
        return loadData;
    }

    public static void SaveData(SettingsData saveData)
    {
        PlayerPrefs.SetFloat("resWidth", saveData.resolution.x);
        PlayerPrefs.SetFloat("resHeight", saveData.resolution.y);
        PlayerPrefs.SetFloat("VolumeValue", saveData.volumeValue);
        PlayerPrefs.SetFloat("SpeedValue", saveData.speedValue);
        PlayerPrefs.SetInt("FullScreen", System.Convert.ToInt32(saveData.isFullScreen));
        PlayerPrefs.Save();
    }
}
