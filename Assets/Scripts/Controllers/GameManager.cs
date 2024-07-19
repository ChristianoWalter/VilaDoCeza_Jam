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
    [SerializeField] GameObject transitionCanvas;

    [Header("Game items manager")]
    public bool isInGame;
    [SerializeField] int levelsUnlocked;
    [SerializeField] GameObject playerRef;
    public GameObject interactBtn;
    public int levelobjectsCollected;
    [HideInInspector] public bool levelStarted;
    [HideInInspector] public bool gameIsPaused;
    GameObject currentSpawnVFX;
    RespawnManager currentRespawn;

    [Header("Level select manager")]
    [SerializeField] List<GameObject> levels;
    [SerializeField] List<Button> levelButton;
    [SerializeField] GameObject pauseBtnSelected;
    [SerializeField] GameObject mainMenuBtnSelected;
    [SerializeField] GameObject configBtnSelected;
    [SerializeField] GameObject[] levelBtnSelected;
    int currentLevelActive;
    [SerializeField] Animator transitionAnim;

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
        //MainMenuStart();
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
            PlayerController.instance.PausePlayerMovement(true);
            gameIsPaused = true;
        }
        else
        {
            SwitchScreen(GameScreens.gameUI);
            PlayerController.instance.PausePlayerMovement(false);
            gameIsPaused = false;
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
        }
    }

    // Método para ser chamado por botão e executar mudança de jogo para tela de menu principal
    public void MainMenuStart()
    {
        isInGame = false;
        SetPlayer();
        playerRef.transform.position = Vector2.zero;
        SwitchScreen(GameScreens.mainMenu);
    }
    
    // Método para ser chamado por botão e executar mudança de jogo para tela de menu principal
    public void SwitchGameToMainMenu()
    {
        StartCoroutine(GameBackMainMenu());
    }

    public IEnumerator GameBackMainMenu()
    {
        FadeIn();
        isInGame = false;
        yield return new WaitForSeconds(1f);
        SetPlayer();
        gameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
        SwitchScreen(GameScreens.mainMenu);
        yield return new WaitForSeconds(1f);
        FadeOut();
        playerRef.transform.position = Vector2.zero;
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

    public void SetDialogueBtn(bool _active, GameObject _btn)
    {
        if (_active)
        {
            EventSystem.current.SetSelectedGameObject(_btn);
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

    // Método para carregar telas de cena do jogo
    public IEnumerator GameToNextLevel()
    {
        yield return new WaitForSeconds(1f);
        FadeIn();
        isInGame = false;
        yield return new WaitForSeconds(1f);
        SetPlayer();
        SwitchScreen(GameScreens.levelSelect);
        SceneManager.LoadScene("MainMenu");
        levels[currentLevelActive].SetActive(false);
        levels[currentLevelActive + 1].SetActive(true);
        currentLevelActive++;
        EventSystem.current.SetSelectedGameObject(levelBtnSelected[currentLevelActive]);
        SaveProgress();
        FadeOut();
        playerRef.transform.position = Vector2.zero;
    }

    public void PlayLevel(string _levelName)
    {
        StartCoroutine(LoadLevel(_levelName));
    }

    // Rotina para carregar cena
    IEnumerator LoadLevel(string _levelName)
    {
        FadeIn();
        levelStarted = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_levelName);
        playerRef.transform.position = Vector2.zero;
        SwitchScreen(GameScreens.gameUI);
        isInGame = true;
        yield return new WaitForSeconds(1f);
        FadeOut();
        SetPlayer();
        playerRef.SetActive(true);
    }

    // Método utilizado para mudar controle do player de acordo com o modo de jogo
    void SetPlayer()
    {
        playerRef.SetActive(isInGame);
        PlayerController.instance.PausePlayerMovement(!isInGame);
    }

    // Método responsável por iniciar transição
    public void FadeIn()
    {
        transitionCanvas.SetActive(true);
        transitionAnim.SetTrigger("FadeIn");
    }

    // Método responsável por finalizar transição
    public void FadeOut()
    {
        transitionAnim.SetTrigger("FadeOut");
        StartCoroutine(FadeOutRoutine());
    }

    // Finaliza transição levando em conta o tempo de animação
    IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        transitionCanvas.SetActive(false);
        if (!levelStarted && FindObjectOfType<DialogueManager>())
        {
            levelStarted = true;
            DialogueManager dRef = FindObjectOfType<DialogueManager>();
            if(dRef.autoStart) dRef.StartDialogue();
        }
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

    // Método para alterar ponto de spawn ativo
    public void ChangeCurrentSpawn(GameObject _currentVFX, RespawnManager _respawnRef)
    {
        if (currentRespawn != null) currentRespawn.activeSpawn = false;
        if (currentSpawnVFX != null) Destroy(currentSpawnVFX);
        currentRespawn = _respawnRef;
        currentSpawnVFX = _currentVFX;
    }
    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }
}
