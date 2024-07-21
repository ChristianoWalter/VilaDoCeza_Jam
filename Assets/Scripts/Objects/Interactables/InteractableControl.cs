using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableControl : MonoBehaviour
{
    // Área para controle dos interágiveis no geral
    [Header("Interact Variables")]
    [SerializeField] protected string objectName;
    [SerializeField] protected bool canInteract;
    [SerializeField] protected UnityEvent action;
    [SerializeField] protected DialogueManager dialogue;
    [SerializeField] protected Animator anim;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] clip;
    AudioManager audioManager;
    protected bool playerIsColliding;

    [Header("Power Up Variables")]
    [SerializeField] PlayerForms playerForms;

    [Header("Level changes Variables")]
    [SerializeField] int objectsToNextLevel;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();  
    }

    private void Update()
    {
        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.instance.interactBtn.SetActive(canInteract);
            playerIsColliding = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (GameManager.instance.interactBtn.activeSelf) GameManager.instance.interactBtn.SetActive(false);
            playerIsColliding = false;
        }
    }

    // Método de recepção de input para ativar evento do objeto
    public void InteractAction(InputAction.CallbackContext value)
    {
        if (value.performed && GameManager.instance.interactBtn.activeInHierarchy && playerIsColliding)
        {
            action.Invoke();
        }
    }

    // Método para coleta do objeto de fase
    public void CollectLevelObject()
    {
        audioManager.PlaySFX(clip[0]);
        canInteract = false;
        GameManager.instance.interactBtn.SetActive(canInteract);
        GameManager.instance.levelobjectsCollected++;
        Destroy(gameObject);
    }

    // Método para coleta de power up
    public void CollectPowerUp()
    {
        canInteract = false;
        GameManager.instance.interactBtn.SetActive(canInteract);
        PlayerController.instance.SwitchPlayerForm(playerForms);
        Destroy(gameObject);
    }

    // Método para mudança de level
    public void LevelFinished()
    {
        canInteract = false;
        GameManager.instance.interactBtn.SetActive(canInteract);
        if (GameManager.instance.levelobjectsCollected == objectsToNextLevel)
        {
            anim.SetTrigger("ChangeDoor");
            PlaySFX(0);
        }
        else
        {
            dialogue.StartDialogue();
        }
    }

    public void FinishDoorOpenAnim()
    {
        if (dialogue.dialogueIndex > 0) dialogue.NextDialogue();
        dialogue.StartDialogue();
    }

    // Ativa próxima fase
    public void NextLevel()
    {
        StartCoroutine(GameManager.instance.GameToNextLevel());
    }

    public void PlaySFX(int index)
    {
        audioSource.clip = clip[index];
        audioSource.Play();
    }

    // Ativa interação com objeto novamente
    public void ActiveInteraction()
    {
        canInteract = true;
    }
}
