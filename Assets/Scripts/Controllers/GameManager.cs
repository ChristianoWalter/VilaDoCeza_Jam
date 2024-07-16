using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameScreens
{
    gameUI,
    mainMenu,
    pauseMenu,
    config,
    levelSelect,
    loading
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Screens Manager")]
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject configPanel;
    [SerializeField] GameObject levelSelectPanel;
    [SerializeField] GameObject loadingPanel;
    bool gameIsPaused;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // M�todo para execu��o do pause do jogo
    public void PauseUnpauseGame()
    {
        if (!gameIsPaused)
        {
            SwitchScreen(GameScreens.pauseMenu);
            gameIsPaused = true;
        }
        else
        {
            SwitchScreen(GameScreens.gameUI);
            gameIsPaused = false;
        }

        PlayerController.instance.PausePlayerMovement();
    }

    // M�todo destinado � mudan�a geral de telas
    public void SwitchScreen(GameScreens _screen)
    {
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        configPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        loadingPanel.SetActive(false);

        switch (_screen)
        {
            case GameScreens.gameUI:
                gamePanel.SetActive(true);
                break;
            case GameScreens.mainMenu: 
                gamePanel.SetActive(true); 
                break;
            case GameScreens.pauseMenu:
                pauseMenuPanel.SetActive(true);
                break;
            case GameScreens.config:
                configPanel.SetActive(true);
                break;
            case GameScreens.levelSelect:
                levelSelectPanel.SetActive(true);
                break;
            case GameScreens.loading:
                loadingPanel.SetActive(true);
                break;
        }
    }

    // M�todo para ser chamado por bot�o e executar mudan�a para tela de menu principal
    public void SwitchToMainMenu()
    {
        SwitchScreen(GameScreens.mainMenu);
    }
    
    // M�todo para ser chamado por bot�o e executar mudan�a para tela de pause
    public void SwitchToPauseMenu()
    {
        SwitchScreen(GameScreens.pauseMenu);
    }
    
    // M�todo para ser chamado por bot�o e executar mudan�a para tela de configura��es
    public void SwitchToConfig()
    {
        SwitchScreen(GameScreens.config);
    }
    
    // M�todo para ser chamado por bot�o e executar mudan�a para tela de sele��o de mapa
    public void SwitchToLevelSelect()
    {
        SwitchScreen(GameScreens.levelSelect);
    }
}
