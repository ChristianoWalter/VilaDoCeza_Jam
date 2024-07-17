using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
    [SerializeField] GameObject DialogPanel;
    [SerializeField] Button nextDialogBtn;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterScript;
    [SerializeField] Image characterImage;

    // Variáveis para manutenção e desenrolar do diálogo
    [Header("Dialogues variables manager")]
    [SerializeField] bool autoStart;
    [SerializeField] DialogueData[] dialogueData;
    public UnityEvent[] lastScriptAction;
    DialogueStates dialogueStates;
    int scriptIndex;
    int dialogueIndex;

    // Variáveis referentes às animações de escrita e da caixa de diálogo
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
        if (autoStart) StartDialogue();
    }

    private void Update()
    {
        if (!DialogPanel.activeSelf) return;

        if (open)
            background.fillAmount = Mathf.Lerp(background.fillAmount, 1f, imageSpeed * Time.deltaTime);
        else
        {
            background.fillAmount = Mathf.Lerp(background.fillAmount, 0f, imageSpeed * Time.deltaTime);
            if (background.fillAmount < 0.05f)
            {
                background.fillAmount = 0f;
                DialogPanel.SetActive(false);
            }
        }
    }

    // Método para ativar o dialogo
    public void StartDialogue()
    {
        if (dialogueData.Length == dialogueIndex) return;

        GameManager.instance.SetDialogueBtn(true);
        PlayerController.instance.PausePlayerMovement(true);
        DialogPanel.SetActive(true);
        open = true;
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
                break;
            case DialogueStates.typing:
                SkipAnimation();
                break;
            case DialogueStates.finished:
                break;
        }
    }

    // Método destinado a finalizar o diálogo
    public void FinishDialogue()
    {
        dialogueStates = DialogueStates.finished;
        if (lastScriptAction.Length != 0)
        {
            if (lastScriptAction[dialogueIndex] != null) lastScriptAction[dialogueIndex].Invoke();
        }
        open = false;
        characterName.text = "";
        characterScript.text = "";
        GameManager.instance.SetDialogueBtn(false);
        PlayerController.instance.PausePlayerMovement(false);
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
