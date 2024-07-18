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
    // Vari�veis externas para visual da UI
    [Header("UI Elements")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Button nextDialogBtn;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterScript;
    [SerializeField] Image characterImage;

    // Vari�veis para manuten��o e desenrolar do di�logo
    [Header("Dialogues variables manager")]
    public bool autoStart;
    [SerializeField] DialogueData[] dialogueData;
    public UnityEvent[] lastScriptAction;
    DialogueStates dialogueStates;
    int scriptIndex;
    int dialogueIndex;

    // Vari�veis referentes �s anima��es de escrita e da caixa de di�logo
    [Header("Animation Variables")]
    [SerializeField] float typeDelay;
    string fullText;
    [SerializeField] float imageSpeed;
    bool open;

    private void Awake()
    {
        nextDialogBtn.onClick.AddListener(OnFinishedScript);
    }

    private void Start()
    {
        background.fillAmount = 0f;
        //if (autoStart) StartDialogue();
    }

    private void Update()
    {
        if (dialoguePanel == null) return;

        if (open)
            background.fillAmount = Mathf.Lerp(background.fillAmount, 1f, imageSpeed * Time.deltaTime);
        else
        {
            background.fillAmount = Mathf.Lerp(background.fillAmount, 0f, imageSpeed * Time.deltaTime);
            if (background.fillAmount < 0.05f)
            {
                background.fillAmount = 0f;
                dialoguePanel.SetActive(false);
            }
        }
    }

    // M�todo para ativar o dialogo
    public void StartDialogue()
    {
        if (dialogueData.Length == dialogueIndex || GameManager.instance.gameIsPaused) return;

        GameManager.instance.isInGame = false;
        GameManager.instance.SetDialogueBtn(true, nextDialogBtn.gameObject);
        PlayerController.instance.PausePlayerMovement(true);
        dialoguePanel.SetActive(true);
        open = true;
        NextText();
    }

    // M�todo para chamar o pr�ximo texto, terminar escrita do texto atual ou finalizar di�logo
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

    // M�todo destinado a finalizar o di�logo
    public void FinishDialogue()
    {
        dialogueStates = DialogueStates.finished;
        GameManager.instance.isInGame = true;
        if (lastScriptAction.Length == 0)
        {
            if (lastScriptAction[dialogueIndex] != null) lastScriptAction[dialogueIndex].Invoke();
        }
        open = false;
        characterName.text = "";
        characterScript.text = "";
        GameManager.instance.SetDialogueBtn(false, null);
        PlayerController.instance.PausePlayerMovement(false);
    }

    // M�todo que ativa di�logo
    public void NextText()
    {
        dialogueStates = DialogueStates.typing;
        characterName.text = dialogueData[dialogueIndex].talkScript[scriptIndex].characterName;
        characterImage.sprite = dialogueData[dialogueIndex].talkScript[scriptIndex].characterImage;
        fullText = dialogueData[dialogueIndex].talkScript[scriptIndex++].dialogueTxt;

        StartCoroutine(TypeText());
    }

    //M�todo que ativa di�logo espec�fico
    public void SelectedText(int _dialogueIndex, int _scriptIndex)
    {
        characterName.text = dialogueData[_dialogueIndex].talkScript[_scriptIndex].characterName;
        characterImage.sprite = dialogueData[_dialogueIndex].talkScript[_scriptIndex].characterImage;
        fullText = dialogueData[_dialogueIndex].talkScript[_scriptIndex++].dialogueTxt;
        scriptIndex = _scriptIndex;

        StartCoroutine(TypeText());
    }

    // M�todo que efetua anima��o de digita��o
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

    // M�todo destinado � pular anima��o de digita��o
    public void SkipAnimation()
    {
        StopAllCoroutines();
        characterScript.maxVisibleCharacters = characterScript.text.Length;
        dialogueStates = DialogueStates.waiting;
    }

    // M�todo destinado a mudar o di�logo
    public void NextDialogue()
    {
        dialogueIndex++;
    }
}
