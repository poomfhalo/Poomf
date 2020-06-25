using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour, IGeneralSettingsProvider
{
    [Header("UI")]
    // The UI panel that contains the game's settings
    [SerializeField] private GameObject settingsUIMenu;
    // UI panel that asks the user if they want to exit without saving
    [SerializeField] private GameObject closeWithoutSavingUI;
    // Contains the UI elements relative to the Audio volume settings
    [SerializeField] private SliderControlledSetting sfxUISettings;
    // Contains the UI elements relative to the Music volume settings
    [SerializeField] private SliderControlledSetting musicUISettings;
    // Contains the UI elements of video settings
    [SerializeField] private VideoSettingsUI videoSettingsUI;
    [Header("Settings")]
    // The asset that manages the audio settings
    [SerializeField] private AudioManager audioManager;
    // The asset that manages the video settings
    [SerializeField] private VideoManager videoManager;
    // Any resolution that's less than this resolution will be omitted
    // In other words, this is the minimum supported resolution
    // X: width, Y: height
    [SerializeField] private Vector2 resolutionThreshold;
    [Header("Assets", order = 0)]
    [Header("Audio", order = 1)]
    [SerializeField] private AudioMixer currentMixer;
    [Header("Default Settings")]
    [SerializeField] private AudioManager defaultAudioSettings;
    [SerializeField] private VideoManager defaultVideoSettings;

    // Used to store audio settings before hitting the Apply button
    private AudioManager tempAudioSettings;
    // Used to store the video settings before hitting the Apply button
    private VideoManager tempVideoSettings;
    // A flag used to check if the user has changed any audio settings or not
    private bool audioSettingsChanged = false;
    // A flag used to check if the user changed any video settings or not
    private bool videoSettingsChanged = false;
    // A list of available resolutions
    private List<Resolution> resolutions = new List<Resolution>();

    private void Start()
    {
        // Get the supported resolutions, omit any ones that are less than the threshold
        Resolution[] allResolutions = Screen.resolutions;
        for (int i = 0; i < allResolutions.Length; i++)
        {
            if (allResolutions[i].width < resolutionThreshold.x || allResolutions[i].height < resolutionThreshold.y)
            {
                // Dont add it, it's below the threshold
                continue;
            }
            else
            {
                resolutions.Add(allResolutions[i]);
            }
        }
        // Initialize the audio UI with the saved audio settings
        audioManager.isCurrent = true;
        audioManager.Initialize(this);
        sfxUISettings.Initialize(audioManager.GetSFXVolume());
        musicUISettings.Initialize(audioManager.GetMusicVolume());

        // Initialize Video UI
        videoManager.isCurrent = true;
        videoManager.Initialize(this);
        videoSettingsUI.Initialize(this);

        // Check if it's the first run here
    }
    #region General
    // Called when the settings button is pressed
    public void OnSettingsButtonPressed()
    {
        if (settingsUIMenu.activeSelf)
        {
            // This means that the settings UI is already open, don't do anything
            return;
        }
        // Show the UI menu
        settingsUIMenu.SetActive(true);
        // Create temp assets to store temp settings
        tempAudioSettings = ScriptableObject.CreateInstance("AudioManager") as AudioManager;
        tempVideoSettings = ScriptableObject.CreateInstance("VideoManager") as VideoManager;
        // Tell Unity garbage collector not to unload these assets, we will unload them ourselves when the settings menu is closed
        tempAudioSettings.hideFlags = HideFlags.HideAndDontSave;
        tempVideoSettings.hideFlags = HideFlags.HideAndDontSave;
        // Set their initial values to be equal to teh current settings
        tempAudioSettings.UpdateAllSettings(audioManager);
        tempVideoSettings.UpdateAllSettings(videoManager);
        // Make sure the settings changed flags are not raised
        ResetFlags();
    }

    // Called when the user hits apply
    public void OnApplyButtonPressed()
    {
        // First, check if the user changed any settings or not
        if (audioSettingsChanged || videoSettingsChanged)
        {
            if (audioSettingsChanged)
            {
                // Apply the new audio settings
                audioManager.UpdateAllSettings(tempAudioSettings);
            }
            if (videoSettingsChanged)
            {
                // Apply the new video settings
                videoManager.UpdateAllSettings(tempVideoSettings);
            }
        }
        else
        {
            // Somehow the apply button was enabled although the user didn't change anything!
            Debug.LogWarning("Apply button was enabled without changing any settings!");
        }

        // Close the settings window
        CloseSettingsWindow();
    }

    // Called when the Close button is pressed
    public void OnCloseButtonPressed()
    {
        // If the user changed anything, ask them if they really want to exit
        if (audioSettingsChanged || videoSettingsChanged)
        {
            closeWithoutSavingUI.SetActive(true);
        }
        else
        {
            CloseSettingsWindow();
        }
    }

    // Restores the previous settings and closes the settings window
    public void OnCloseWithoutSaving()
    {
        if (audioSettingsChanged)
        {
            // Revert the audio settings UI changes
            sfxUISettings.SetValue(audioManager.GetSFXVolume());
            musicUISettings.SetValue(audioManager.GetMusicVolume());
        }
        if (videoSettingsChanged)
        {
            // Revert the video settings UI changes
            videoSettingsUI.UpdateAllUI(videoManager.IsFullscreen(), videoManager.GetQualityIndex(), videoManager.GetResolutionIndex());
        }

        // Close the prompt window
        closeWithoutSavingUI.SetActive(false);
        // Close the settings window
        CloseSettingsWindow();
    }

    public void CloseSettingsWindow()
    {
        // Hide the settings window
        settingsUIMenu.SetActive(false);
        // Unload the temp settings assets
        Destroy(tempAudioSettings);
        Destroy(tempVideoSettings);
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
    // Resets the "settings have changed" flags
    void ResetFlags()
    {
        audioSettingsChanged = false;
        videoSettingsChanged = false;
    }
    #endregion

    #region Audio
    // Called when the SFX slider changes, the changes won't be saved unless the Apply button is pressed
    public void OnSfxSliderChanged(float value)
    {
        if (tempAudioSettings == null)
        {
            // This means that the slider changed without being in the settings menu (When the game first runs for example)
            return;
        }
        // Change the UI text
        sfxUISettings.UpdateTextValue(value);
        // Save the new value in the temp audio settings
        tempAudioSettings.SetSFXVolume(value);
        audioSettingsChanged = true;
    }

    // Called when the Music slider changes, the changes won't be saved unless the Apply button is pressed
    public void OnMusicSliderChanged(float value)
    {
        if (tempAudioSettings == null)
        {
            // This means that the slider changed without being in the settings menu (When the game first runs for example)
            return;
        }
        // Change the UI text
        musicUISettings.UpdateTextValue(value);
        // Save the new value in the temp audio settings
        tempAudioSettings.SetMusicVolume(value);
        audioSettingsChanged = true;
    }
    #endregion

    #region Video
    public void OnResolutionSelected(int index)
    {
        if (tempVideoSettings == null)
        {
            // This means that the selection changed without being in the settings menu (When the game first runs for example)
            return;
        }
        tempVideoSettings.SetResolution(index);
        videoSettingsChanged = true;
    }
    public void OnFullscreenToggled(bool fullscreen)
    {
        if (tempVideoSettings == null)
        {
            // This means that the toggle changed without being in the settings menu (When the game first runs for example)
            return;
        }
        tempVideoSettings.SetFullscreen(fullscreen);
        videoSettingsChanged = true;
    }
    public void OnQualitySelected(int index)
    {
        if (tempVideoSettings == null)
        {
            // This means that the selection changed without being in the settings menu (When the game first runs for example)
            return;
        }
        tempVideoSettings.SetQuality((Quality)index);
        videoSettingsChanged = true;
    }
    #endregion

    #region IGeneralSettingsProvider
    public List<Resolution> GetResolutionsList()
    {
        return resolutions.GetCopy();
    }
    public IVideoProvider GetVideoSettings()
    {
        return videoManager as IVideoProvider;
    }
    public IVideoProvider GetDefaultVideoSettings()
    {
        return defaultVideoSettings as IVideoProvider;
    }
    public IAudioProvider GetAudioSettings()
    {
        return audioManager as IAudioProvider;
    }
    public IAudioProvider GetDefaultAudioSettings()
    {
        return defaultAudioSettings as IAudioProvider;
    }
    public AudioMixer GetCurrentAudioMixer()
    {
        return currentMixer;
    }

    #endregion
}
