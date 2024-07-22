using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DialogueStates
{
    typing,
    waiting,
    finished
}

public class DialogueManager : MonoBehaviour
{
    // Variáveis externas para visual da UI
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public Button nextDialogBtn;
    public Image background;
    public GameObject textBackground;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterScript;
    public Image characterImage;
    public AudioManager audioManager;
    public GameManager gameManager;

    // Variáveis para manutenção e desenrolar do diálogo
    [Header("Dialogues variables manager")]
    public bool autoStart;
    public DialogueData[] dialogueData;
    public UnityEvent[] lastScriptAction;
    DialogueStates dialogueStates;
    public int scriptIndex;
    public int dialogueIndex;
    bool isInDialogue;

    // Variáveis referentes às animações de escrita e da caixa de diálogo
    [Header("Animation Variables")]
    [SerializeField] float typeDelay;
    string fullText;
    [SerializeField] float imageSpeed;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        
        gameManager = FindObjectOfType<GameManager>();
        dialoguePanel = gameManager.dialoguePanel;
        characterImage = gameManager.characterImage;
        textBackground = gameManager.textBackground;
        nextDialogBtn = gameManager.nextDialogBtn;
        background = gameManager.background;
        characterName = gameManager.characterName;
        characterScript = gameManager.characterScript;
    }

    private void Start()
    {
        //nextDialogBtn.onClick.AddListener(OnFinishedScript);
        if (autoStart && gameManager != null) StartDialogue();
    }

    private void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            dialoguePanel = gameManager.dialoguePanel;
            nextDialogBtn = gameManager.nextDialogBtn;
            background = gameManager.background;
            textBackground = gameManager.textBackground;
            characterName = gameManager.characterName;
            characterScript = gameManager.characterScript;
            characterImage = gameManager.characterImage;
        }

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        /*if (isInDialogue && !dialoguePanel.activeSelf)
        {
            background.fillAmount = 1f;
            dialoguePanel.SetActive(true);
            textBackground.SetActive(true);
            characterImage.gameObject.SetActive(true);
            nextDialogBtn.onClick.AddListener(OnFinishedScript);
            GameManager.instance.SetDialogueBtn(true, nextDialogBtn.gameObject);
            characterScript.maxVisibleCharacters = characterScript.text.Length;
            isInDialogue = false;
        }*/
    }


    // Método para ativar o dialogo
    public void StartDialogue()
    {
        if (dialogueData.Length == dialogueIndex || GameManager.instance.gameIsPaused) return;
        while (dialoguePanel == null) { dialoguePanel = gameManager.dialoguePanel; }

        audioManager.ChangeMusic(2);
        dialoguePanel.SetActive(true);
        GameManager.instance.isInGame = false;
        PlayerController.instance.PausePlayerMovement(true);
        nextDialogBtn.onClick.AddListener(OnFinishedScript);

        if (autoStart) 
        {
            background.fillAmount = 1f;
            textBackground.SetActive(true);
            characterImage.gameObject.SetActive(true);
            GameManager.instance.SetDialogueBtn(true, nextDialogBtn.gameObject);
            //isInDialogue = true;
            NextText();
        }
        else
        {
            StartCoroutine(ActiveDialoguePanel());
        }
    }

    // Rotina de ativação do diálogo
    public IEnumerator ActiveDialoguePanel()
    {
        for (float time = 0; time < imageSpeed - .03; time += .01f)
        {
            background.fillAmount = time/imageSpeed;
            yield return new WaitForSeconds(.01f);
        }
        //isInDialogue = true;
        background.fillAmount = 1f;
        GameManager.instance.SetDialogueBtn(true, nextDialogBtn.gameObject);
        textBackground.SetActive(true);
        characterImage.gameObject.SetActive(true);
        NextText();
    }

    // Método para chamar o próximo texto, terminar escrita do texto atual ou finalizar diálogo
    public void OnFinishedScript()
    {
        switch (dialogueStates)
        {
            case DialogueStates.waiting:
                if (dialogueData[dialogueIndex].talkScript.Count <= scriptIndex) FinishDialogue();
                else NextText();
                if (background.fillAmount != 1f)
                {
                    background.fillAmount = 1f;
                    GameManager.instance.SetDialogueBtn(true, nextDialogBtn.gameObject);
                    textBackground.SetActive(true);
                    characterImage.gameObject.SetActive(true);
                }
                break;
            case DialogueStates.typing:
                SkipAnimation();
                break;
            case DialogueStates.finished:
                break;
        }
    }

    // Rotina para finalizar diálogo
    public IEnumerator CloseDialoguePanel()
    {
        for (float time = imageSpeed; time > .03; time -= .01f)
        {
            background.fillAmount = time/imageSpeed;
            yield return new WaitForSeconds(.01f);
        }
        background.fillAmount = 0f;
        Debug.Log("Fechou");
        dialoguePanel.SetActive(false);
        PlayerController.instance.PausePlayerMovement(false);
    }

    // Método destinado a finalizar o diálogo
    public void FinishDialogue()
    {
        //isInDialogue = false;
        dialogueStates = DialogueStates.finished;
        scriptIndex = 0;
        GameManager.instance.isInGame = true;
        StartCoroutine(CloseDialoguePanel());
        characterName.text = "";
        characterScript.text = "";
        textBackground.SetActive(false);
        characterImage.gameObject.SetActive(false);
        GameManager.instance.SetDialogueBtn(false, null);
        audioManager.ChangeMusic(1);

        if (lastScriptAction.Length != 0)
        {
            if (lastScriptAction[dialogueIndex] != null) lastScriptAction[dialogueIndex].Invoke();
        }
    }

    // Método que ativa diálogo
    public void NextText()
    {
        dialogueStates = DialogueStates.typing;
        characterName.text = dialogueData[dialogueIndex].talkScript[scriptIndex].characterName;
        characterImage.sprite = dialogueData[dialogueIndex].talkScript[scriptIndex].characterImage;
        fullText = dialogueData[dialogueIndex].talkScript[scriptIndex++].dialogueTxt;

        StartCoroutine(TypeText());
    }

    //Método que ativa diálogo específico
    public void SelectedText(int _dialogueIndex, int _scriptIndex)
    {
        characterName.text = dialogueData[_dialogueIndex].talkScript[_scriptIndex].characterName;
        characterImage.sprite = dialogueData[_dialogueIndex].talkScript[_scriptIndex].characterImage;
        fullText = dialogueData[_dialogueIndex].talkScript[_scriptIndex++].dialogueTxt;
        scriptIndex = _scriptIndex;

        StartCoroutine(TypeText());
    }

    // Método que efetua animação de digitação
    public IEnumerator TypeText()
    {
        characterScript.text = fullText;
        characterScript.maxVisibleCharacters = 0;
        for (int i = 0; i <= characterScript.text.Length; i++)
        {
            characterScript.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }
        if (characterScript.maxVisibleCharacters == characterScript.text.Length) dialogueStates = DialogueStates.waiting;
    }

    // Método destinado à pular animação de digitação
    public void SkipAnimation()
    {
        StopAllCoroutines();
        characterScript.maxVisibleCharacters = characterScript.text.Length;
        dialogueStates = DialogueStates.waiting;
    }

    // Método destinado a mudar o diálogo
    public void NextDialogue()
    {
        dialogueIndex++;
    }
}
