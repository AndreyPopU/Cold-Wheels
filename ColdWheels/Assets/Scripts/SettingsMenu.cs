using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class SettingsMenu : MonoBehaviour
{
    public static bool loadedSettings = false;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Slider sensitivitySlider;
    public Text sensitivityText;
    public Slider soundSlider;
    public Text soundText;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public CanvasGroup fadePanel;
    public Button[] transitionButtons;
    public Color activeColor;
    public Vector3 outOfScreen;
    public Texture2D cursorSprite;
    public Text fullscreenButtonText;

    Resolution[] resolutions;

    private bool isFullscreen;
    private bool sound;

    void Start()
    {
        fadePanel.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!loadedSettings)
        {
            LoadSettings();
        
            Vector2 hotSpot = new Vector2(cursorSprite.width / 2f, cursorSprite.height / 2f);
            Cursor.SetCursor(cursorSprite, hotSpot, CursorMode.Auto);
            loadedSettings = true;
        }

        sensitivityText.text = String.Format("{0:0.00}", sensitivitySlider.value);
        float soundClamped = (soundSlider.value + 80) / 80;
        soundText.text = String.Format("{0:0.00}", soundClamped);

        StartCoroutine(Fade());
    }

    public void FindResolution(bool saved)
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolution = 0;
        if (saved) currentResolution = resolutionDropdown.value = PlayerPrefs.GetInt(SaveLoadManager.resolutionString);

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (!saved) { if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) currentResolution = i; }
        }

        options.Add(Screen.resolutions[Screen.resolutions.Length - 1].width + " x " + Screen.resolutions[Screen.resolutions.Length - 1].height);

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();

        SetResolution(currentResolution);
    }

    public void LoadSettings()
    {
        //if (PlayerPrefs.HasKey(SaveLoadManager.qualityString)) qualityDropdown.value = PlayerPrefs.GetInt(SaveLoadManager.qualityString);
        //else qualityDropdown.value = 0;
        //qualityDropdown.RefreshShownValue();

        FindResolution(PlayerPrefs.HasKey(SaveLoadManager.resolutionString));

        if (PlayerPrefs.HasKey(SaveLoadManager.sensitivityString)) sensitivitySlider.value = PlayerPrefs.GetFloat(SaveLoadManager.sensitivityString);
        else sensitivitySlider.value = 2;

        if (PlayerPrefs.HasKey(SaveLoadManager.soundString)) { soundSlider.value = PlayerPrefs.GetFloat(SaveLoadManager.soundString); print("There"); }
        else soundSlider.value = -40;

        if (PlayerPrefs.HasKey(SaveLoadManager.fullscreenString)) isFullscreen = SaveLoadManager.IntToBool(PlayerPrefs.GetInt(SaveLoadManager.fullscreenString));
        else isFullscreen = Screen.fullScreen;
        Fullscreen();
    }

    public void OpenMenuVoid(GameObject menu) { StartCoroutine(OpenMenu(menu)); }

    public void CloseMenuRight(GameObject menu)
    {
        //SaveLoadManager.SaveSettings(soundSlider.value, sensitivitySlider.value, SaveLoadManager.BoolToInt(isFullscreen),
        //    resolutionDropdown.value, qualityDropdown.value, difficultyDropdown.value);
        StartCoroutine(CloseMenu(menu, false));
    }
    public void CloseMenuLeft(GameObject menu) { StartCoroutine(CloseMenu(menu, true)); }

    public IEnumerator OpenMenu(GameObject menu)
    {
        foreach (Button button in transitionButtons)
        {
            button.interactable = false;
        }

        while (true)
        {
            menu.transform.localPosition = Vector3.Lerp(menu.transform.localPosition, Vector3.zero, .175f);
            if (Mathf.Abs(menu.transform.localPosition.x) < 2) break;

            yield return new WaitForSeconds(.02f);
        }

        menu.transform.localPosition = Vector3.zero;

        foreach (Button button in transitionButtons)
        {
            button.interactable = true;
        }
    }

    public IEnumerator CloseMenu(GameObject menu, bool left)
    {
        Vector3 pos = Vector3.zero;

        if (left)
        {
            pos = outOfScreen;

            while (true)
            {
                menu.transform.localPosition = Vector3.Lerp(menu.transform.localPosition, pos, .175f);
                if (menu.transform.localPosition.x - pos.x < 5) break;

                yield return new WaitForSeconds(.02f);
            }
        }
        else
        {
            pos = -outOfScreen;

            while (true)
            {
                menu.transform.localPosition = Vector3.Lerp(menu.transform.localPosition, pos, .175f);
                if (menu.transform.localPosition.x - Mathf.Abs(pos.x) > -5) break;

                yield return new WaitForSeconds(.02f);
            }
        }

        menu.transform.localPosition = pos;
    }

    // Quality
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    // Resolution
    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);
    }

    // Sound on/off
    public void Sound()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponent<Button>().GetComponentInChildren<Text>();

            if (sound) { sound = false; buttonText.color = Color.white; }
            else { sound = true; buttonText.color = activeColor; }
        }

        // enable sound
        // ambient sound
    }

    // Fullscreen
    public void Fullscreen()
    {
        if (isFullscreen) { isFullscreen = false; fullscreenButtonText.color = Color.white; }
        else { isFullscreen = true; fullscreenButtonText.color = activeColor; }

        Screen.fullScreen = isFullscreen;
    }

    public IEnumerator Fade()
    {
        if (fadePanel.alpha > 0)
        {
            while (fadePanel.alpha > 0)
            {
                fadePanel.alpha -= Time.deltaTime;
                // Fade out music
                yield return null;
            }
        }
        else if (fadePanel.alpha < 1)
        {
            Cursor.visible = false;
            while (fadePanel.alpha < 1)
            {
                fadePanel.alpha += 1.5f * Time.deltaTime;
                // Fade in music
                yield return null;
            }
        }
    }

    public void ExitGame() { StartCoroutine(Exit()); }

    public IEnumerator Exit()
    {
        fadePanel.gameObject.SetActive(true);

        StartCoroutine(Fade());

        while (fadePanel.alpha < 1) { yield return new WaitForSeconds(.02f); }

        print("quit");
        Application.Quit();
    }

    public void UpdateSensitivityText()
    {
        sensitivityText.text = String.Format("{0:0.00}", sensitivitySlider.value);
    }

    public void UpdateSoundText()
    {
        float soundClamped = (soundSlider.value + 80) / 80;
        soundText.text = String.Format("{0:0.00}", soundClamped);
    }
}
