using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;
public enum GameScreens
{
    gameUI,
    mainMenu,
    pauseMenu,
    config,
    levelSelect,
    controls
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Vari�veis para controle de naveg��o de telas
    [Header("Screens Manager")]
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject configPanel;
    [SerializeField] GameObject levelSelectPanel;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject transitionCanvas;

    // Vari�veis para controle de jogo no geral
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

    // Vari�veis para controle e sele��o de n�veis
    [Header("Level select manager")]
    [SerializeField] List<GameObject> levels;
    [SerializeField] List<Button> levelButton;
    [SerializeField] GameObject pauseBtnSelected;
    [SerializeField] GameObject mainMenuBtnSelected;
    [SerializeField] GameObject configBtnSelected;
    [SerializeField] GameObject controlsBtnSelected;
    [SerializeField] GameObject[] levelBtnSelected;
    int currentLevelActive;
    [SerializeField] Animator transitionAnim;

    // Vari�veis para controle de di�logo
    [Header ("Dialogue UI Variables")]
    public GameObject dialoguePanel;
    public Button nextDialogBtn;
    public Image background;
    public GameObject textBackground;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterScript;
    public Image characterImage;

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

    // M�todo direcionado � salvar progresso do jogo
    public void SaveProgress()
    {
        SaveManager.SaveGame(new SaveFiles(levelsUnlocked));
    }

    // M�todo de entrada de input para pausar jogo
    public void PauseAction(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            PauseUnpauseGame();
        }
    }

    // M�todo para execu��o do pause do jogo
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

    // Regi�o destinada � m�todos de navega��o de tela
    #region Screens Navigation
    // M�todo destinado � mudan�a geral de telas
    public void SwitchScreen(GameScreens _screen)
    {
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        configPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        controlsPanel.SetActive(false);
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
            case GameScreens.controls:
                controlsPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(controlsBtnSelected);
                break;
        }
    }

    // M�todo para ser chamado por bot�o e executar mudan�a de jogo para tela de menu principal
    public void MainMenuStart()
    {
        isInGame = false;
        SetPlayer();
        playerRef.transform.position = Vector2.zero;
        SwitchScreen(GameScreens.mainMenu);
    }
    
    // M�todo para ser chamado por bot�o e executar mudan�a de jogo para tela de menu principal
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

    // Regi�o destinada � navega��o de niveis
    #region Levels Navigation
    // M�todo direcionado � carregar progresso do jogo
    public void LoadProgress()
    {
        for (int i = 0; i < levelsUnlocked; i++)
        {
            levelButton[i].interactable = true;
        }
    }

    // M�todo para carregar telas de cena do jogo
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
        LoadProgress();
        playerRef.transform.position = Vector2.zero;
    }

    public void PlayLevel(string _levelName)
    {
        StartCoroutine(LoadLevel(_levelName));
        PlayerController.instance.SetPlayerToNewLvl();
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

    // M�todo utilizado para mudar controle do player de acordo com o modo de jogo
    void SetPlayer()
    {
        playerRef.SetActive(isInGame);
        PlayerController.instance.PausePlayerMovement(!isInGame);
    }

    // M�todo respons�vel por iniciar transi��o
    public void FadeIn()
    {
        transitionCanvas.SetActive(true);
        transitionAnim.SetTrigger("FadeIn");
    }

    // M�todo respons�vel por finalizar transi��o
    public void FadeOut()
    {
        transitionAnim.SetTrigger("FadeOut");
        StartCoroutine(FadeOutRoutine());
    }

    // Finaliza transi��o levando em conta o tempo de anima��o
    IEnumerator FadeOutRoutine()
    {
       /* if (!levelStarted && GameObject.FindGameObjectWithTag("LevelDialogue"))
        {
            levelStarted = true;
            DialogueManager dRef = GameObject.FindGameObjectWithTag("LevelDialogue").GetComponent<DialogueManager>();
            if(dRef.autoStart) dRef.StartDialogue();
        }*/
        yield return new WaitForSeconds(1f);
        transitionCanvas.SetActive(false);
    }

    // M�todo para mudan�a de level no level select
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

    // M�todo para alterar ponto de spawn ativo
    public void ChangeCurrentSpawn(GameObject _currentVFX, RespawnManager _respawnRef)
    {
        if (currentRespawn != null) currentRespawn.activeSpawn = false;
        if (currentSpawnVFX != null) Destroy(currentSpawnVFX);
        currentRespawn = _respawnRef;
        currentSpawnVFX = _currentVFX;
    }

    // M�todo para desbloquear todos os n�veis de jogo
    public void UnlockAllLevels()
    {
        levelsUnlocked = levels.Count - 1;
        LoadProgress();
        SaveProgress();
    }
    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }
}
