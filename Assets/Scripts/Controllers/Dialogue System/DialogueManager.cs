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
    GameObject dialoguePanel;
    Button nextDialogBtn;
    Image background;
    GameObject textBackground;
    TextMeshProUGUI characterName;
    TextMeshProUGUI characterScript;
    Image characterImage;
    AudioManager audioManager;
    GameManager gameManager;

    // Variáveis para manutenção e desenrolar do diálogo
    [Header("Dialogues variables manager")]
    public bool autoStart;
    public DialogueData[] dialogueData;
    public UnityEvent[] lastScriptAction;
    DialogueStates dialogueStates;
    public int scriptIndex;
    public int dialogueIndex;

    // Variáveis referentes às animações de escrita e da caixa de diálogo
    [Header("Animation Variables")]
    [SerializeField] float typeDelay;
    string fullText;
    [SerializeField] float imageSpeed;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
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
    }

    private void Start()
    {
        //nextDialogBtn.onClick.AddListener(OnFinishedScript);
        if (autoStart) StartDialogue();
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
    }

    // Rotina de ativação do diálogo
    public IEnumerator ActiveDialoguePanel()
    {
        for (float time = 0; time < imageSpeed - .03; time += .01f)
        {
            background.fillAmount = time/imageSpeed;
            yield return new WaitForSeconds(.01f);
        }
        background.fillAmount = 1f;
        GameManager.instance.SetDialogueBtn(true, nextDialogBtn.gameObject);
        textBackground.SetActive(true);
        characterImage.gameObject.SetActive(true);
        NextText();
    }

    // Método para ativar o dialogo
    public void StartDialogue()
    {
        if (dialogueData.Length == dialogueIndex || GameManager.instance.gameIsPaused) return;

        nextDialogBtn.onClick.AddListener(OnFinishedScript);
        audioManager.ChangeMusic(2);
        GameManager.instance.isInGame = false;
        PlayerController.instance.PausePlayerMovement(true);
        dialoguePanel.SetActive(true);

        if (autoStart) 
        {
            background.fillAmount = 1f;
            textBackground.SetActive(true);
            characterImage.gameObject.SetActive(true);
            GameManager.instance.SetDialogueBtn(true, nextDialogBtn.gameObject);
            NextText();
        }
        else
        {
            StartCoroutine(ActiveDialoguePanel());
        }
    }

    // Método para chamar o próximo texto, terminar escrita do texto atual ou finalizar diálogo
    public void OnFinishedScript()
    {
        switch (dialogueStates)
        {
            case DialogueStates.waiting:
                if (dialogueData[dialogueIndex].talkScript.Count <= scriptIndex) FinishDialogue();
                else NextText(); 
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
        GameManager.instance.isInGame = true;
        for (float time = imageSpeed; time > .03; time -= .01f)
        {
            background.fillAmount = time/imageSpeed;
            yield return new WaitForSeconds(.01f);
        }
        background.fillAmount = 0f;
        dialoguePanel.SetActive(false);
        PlayerController.instance.PausePlayerMovement(false);
    }

    // Método destinado a finalizar o diálogo
    public void FinishDialogue()
    {
        dialogueStates = DialogueStates.finished;
        scriptIndex = 0;
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
