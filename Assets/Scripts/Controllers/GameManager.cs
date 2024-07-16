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

    // Método para execução do pause do jogo
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

    // Método destinado à mudança geral de telas
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

    // Método para ser chamado por botão e executar mudança para tela de menu principal
    public void SwitchToMainMenu()
    {
        SwitchScreen(GameScreens.mainMenu);
    }
    
    // Método para ser chamado por botão e executar mudança para tela de pause
    public void SwitchToPauseMenu()
    {
        SwitchScreen(GameScreens.pauseMenu);
    }
    
    // Método para ser chamado por botão e executar mudança para tela de configurações
    public void SwitchToConfig()
    {
        SwitchScreen(GameScreens.config);
    }
    
    // Método para ser chamado por botão e executar mudança para tela de seleção de mapa
    public void SwitchToLevelSelect()
    {
        SwitchScreen(GameScreens.levelSelect);
    }
}
