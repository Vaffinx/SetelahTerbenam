using UnityEngine;

public class InteractionSensor : MonoBehaviour
{
    private IInteractable interactable;

    void Update()
    {
        // Pastikan interactable tidak null sebelum memanggil Interact()
        if (Input.GetKeyDown(KeyCode.E) && interactable != null)
        {
            interactable.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable foundInteractable = collision.GetComponent<IInteractable>();
        if (foundInteractable != null)
        {
            interactable = foundInteractable;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable foundInteractable = collision.GetComponent<IInteractable>();
        if (foundInteractable != null && interactable == foundInteractable)
        {
            interactable = null;
        }
    }
}
