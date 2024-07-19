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
    [SerializeField]bool playerIsColliding;

    [Header("Power Up Variables")]
    [SerializeField] PlayerForms playerForms;

    [Header("Level changes Variables")]
    [SerializeField] int objectsToNextLevel;

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
            GameManager.instance.interactBtn.SetActive(false);
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
        canInteract = false;
        GameManager.instance.interactBtn.SetActive(canInteract);
        if (PlayerPrefs.HasKey(objectName))
        {
            GameManager.instance.levelobjectsCollected++;
            Destroy(gameObject);
        }
        else
        {
            PlayerPrefs.SetString(objectName, objectName);
            if (dialogue != null) dialogue.StartDialogue();
        }
    }

    // Método para coleta de power up
    public void CollectPowerUp()
    {
        canInteract = false;
        GameManager.instance.interactBtn.SetActive(canInteract);
        if (PlayerPrefs.HasKey(objectName))
        {
            PlayerController.instance.SwitchPlayerForm(playerForms);
            Destroy(gameObject);
        }
        else
        {
            PlayerPrefs.SetString(objectName, objectName);
            if (dialogue != null) dialogue.StartDialogue();
        }
    }

    // Método para mudança de level
    public void LevelFinished()
    {
        canInteract = false;
        GameManager.instance.interactBtn.SetActive(canInteract);
        if (GameManager.instance.levelobjectsCollected == objectsToNextLevel)
        {
            dialogue.NextDialogue();
            dialogue.StartDialogue();
        }
        else
        {
            dialogue.StartDialogue();
        }
    }

    // Ativa próxima fase
    public void NextLevel()
    {
        StartCoroutine(GameManager.instance.GameToNextLevel());
    }

    // Ativa interação com objeto novamente
    public void ActiveInteraction()
    {
        canInteract = true;
    }
}
