using UnityEngine;
using GlobalEnums;

public abstract class NPC : MonoBehaviour, IInteractable
{
    

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.DIALOGUE)
        {
            if (PlayerInput.Instance.InteractPressed)
            {
                Interact();
            }
        }
        else if (GameManager.Instance.GameState == GameState.DIALOGUE)
        {
            if (PlayerInput.Instance.AgreeButtonPressed)
            {
                Interact();
            }
        }
    }

    public abstract void Interact();

}
