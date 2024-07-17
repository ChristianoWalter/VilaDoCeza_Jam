using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
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

    [Header("Game items manager")]
    public bool isInGame;
    [SerializeField] int levelsUnlocked;
    [SerializeField] GameObject playerRef;
    bool gameIsPaused;

    [Header("Level select manager")]
    [SerializeField] List<GameObject> levels;
    [SerializeField] List<Button> levelButton;
    [SerializeField] GameObject pauseBtnSelected;
    [SerializeField] GameObject mainMenuBtnSelected;
    [SerializeField] GameObject configBtnSelected;
    [SerializeField] GameObject dialogBtn;
    [SerializeField] GameObject[] levelBtnSelected;
    int currentLevelActive;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        if (SaveManager.LoadFiles() != null)
        {
            levelsUnlocked = SaveManager.LoadFiles().levelsUnlocked;
        }
    }

    private void Start()
    {
        MainMenuStart();
        //LoadProgress();
    }

    // Método direcionado à salvar progresso do jogo
    public void SaveProgress()
    {
        SaveManager.SaveGame(new SaveFiles(levelsUnlocked));
    }

    // Método de entrada de input para pausar jogo
    public void PauseAction(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            PauseUnpauseGame();
        }
    }

    // Método para execução do pause do jogo
    public void PauseUnpauseGame()
    {
        if (!isInGame) return;
        if (!gameIsPaused)
        {
            SwitchScreen(GameScreens.pauseMenu);
            gameIsPaused = true;
            PlayerController.instance.PausePlayerMovement(true);
        }
        else
        {
            SwitchScreen(GameScreens.gameUI);
            gameIsPaused = false;
            PlayerController.instance.PausePlayerMovement(false);
        }
    }

    // Região destinada à métodos de navegação de tela
    #region Screens Navigation
    // Método destinado à mudança geral de telas
    public void SwitchScreen(GameScreens _screen)
    {
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        configPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        loadingPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);


        switch (_screen)
        {
            case GameScreens.gameUI:
                gamePanel.SetActive(true);
                break;
            case GameScreens.mainMenu: 
                mainMenuPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(mainMenuBtnSelected);
                break;
            case GameScreens.pauseMenu:
                pauseMenuPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(pauseBtnSelected);
                break;
            case GameScreens.config:
                configPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(configBtnSelected);
                break;
            case GameScreens.levelSelect:
                levelSelectPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(levelBtnSelected[currentLevelActive]);
                break;
            case GameScreens.loading:
                loadingPanel.SetActive(true);
                break;
        }
    }

    // Método para ser chamado por botão e executar mudança de jogo para tela de menu principal
    public void MainMenuStart()
    {
        isInGame = false;
        PlayerController.instance.PausePlayerMovement(true);
        SwitchScreen(GameScreens.mainMenu);
        playerRef.transform.position = Vector2.zero;
        playerRef.SetActive(false);
    }
    
    // Método para ser chamado por botão e executar mudança de jogo para tela de menu principal
    public void SwitchGameToMainMenu()
    {
        isInGame = false;
        SceneManager.LoadScene("MainMenu");
        SwitchScreen(GameScreens.mainMenu);
        playerRef.transform.position = Vector2.zero;
        playerRef.SetActive(false);
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

    // Método para ser chamado por botão e executar mudança para tela de loading
    public void SwitchToLoading()
    {
        SwitchScreen(GameScreens.loading);
    }
    
    // Método para ser chamado por botão e executar mudança para tela de seleção de mapa
    public void SwitchToLevelSelect()
    {
        SwitchScreen(GameScreens.levelSelect);
    }

    public void SetDialogueBtn(bool _active)
    {
        if (_active)
        {
            EventSystem.current.SetSelectedGameObject(dialogBtn);
            isInGame = false;
        }
        else 
        { 
            EventSystem.current.SetSelectedGameObject(null);
            isInGame = true;
        }
    }
    #endregion

    // Região destinada à navegação de niveis
    #region Levels Navigation
    // Método direcionado à carregar progresso do jogo
    public void LoadProgress()
    {
        switch (levelsUnlocked)
        {
            case 1:
                levelButton[0].interactable = true;
                break;
            case 2:
                levelButton[0].interactable = true;
                levelButton[1].interactable = true;
                break;
            case 3:
                levelButton[0].interactable = true;
                levelButton[1].interactable = true;
                levelButton[2].interactable = true;
                break;
            case 4:
                levelButton[0].interactable = true;
                levelButton[1].interactable = true;
                levelButton[2].interactable = true;
                levelButton[3].interactable = true;
                break;
        }
    }

    public void PlayLevel(string _levelName)
    {
        SceneManager.LoadScene(_levelName);
        SwitchScreen(GameScreens.gameUI);

        playerRef.transform.position = Vector2.zero;
        playerRef.SetActive(true);
        PlayerController.instance.PausePlayerMovement(false);
        isInGame = true;
    }

    // Método para mudança de level no level select
    public void NextLevel(int _levelNumber)
    {
        if (levels == null) return;
        levels[_levelNumber - 1].SetActive(false);
        levels[_levelNumber].SetActive(true);
        currentLevelActive = _levelNumber;
        EventSystem.current.SetSelectedGameObject(levelBtnSelected[_levelNumber]);
    }
    public void LastLevel(int _levelNumber)
    {
        if (levels == null) return;
        levels[_levelNumber + 1].SetActive(false);
        levels[_levelNumber].SetActive(true);
        currentLevelActive = _levelNumber;
        EventSystem.current.SetSelectedGameObject(levelBtnSelected[_levelNumber]);
    }
    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }
}
