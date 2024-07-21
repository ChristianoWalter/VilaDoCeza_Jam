using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphsController : MonoBehaviour
{
    [Header("Resolution Settings")]
    [SerializeField] TextMeshProUGUI resolutionTxt;
    [SerializeField] Vector2[] resolutions;
    int resolutionIndex = 3;

    [Header("Graphic Settings")]
    [SerializeField] TextMeshProUGUI screenTxt;
    [SerializeField] string[] screenName;
    int screenIndex;

    private void Start()
    {
        if (PlayerPrefs.HasKey("ResolutionValue") && PlayerPrefs.HasKey("WindowsValue"))
        {
            resolutionIndex = resolutions.Length - 1;
            screenIndex = 0;
            SetScreenMode();
            SetResolution();
        }
        else
        {
            PlayerPrefs.GetInt("ResolutionValue", resolutionIndex);
            PlayerPrefs.GetInt("WindowsValue", screenIndex);
            SetResolution();
            SetScreenMode();
        }
    }

    public void ResolutionUpgrade()
    {
        if(resolutionIndex < resolutions.Length - 1)
        {
            resolutionIndex++;
            SetResolution();
        }
    }

    void SetResolution()
    {
        Screen.SetResolution((int)resolutions[resolutionIndex].x, (int)resolutions[resolutionIndex].y, true);
        resolutionTxt.text = (int)resolutions[resolutionIndex].x + " x " + (int)resolutions[resolutionIndex].y;
        PlayerPrefs.SetInt("ResolutionValue", resolutionIndex);
    }

    public void ResolutionDowngrade() 
    {
        if (resolutionIndex > 0)
        {
            resolutionIndex--;
            SetResolution();
        }
    }

    public void WindowUpgrade()
    {
        if (screenIndex < 2)
        {
            screenIndex++;
            SetScreenMode();
        }
    }

    void SetScreenMode()
    {
        switch (screenIndex)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow; 
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        screenTxt.text = screenName[screenIndex];
        PlayerPrefs.SetInt("WindowsValue", screenIndex);
    }

    public void WindowDownGrade()
    {
        if (screenIndex > 0)
        {
            screenIndex--;
            SetScreenMode();
        }
    }
}
