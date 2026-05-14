using UnityEngine;

public class InteractWithArtefact : MonoBehaviour
{
    void Update()
    {
        if (PlayerInput.Instance.InteractPressed)
        {
            TakeArtefact();
        }
    }

    private void TakeArtefact()
    {
        PlayerView.Instance.PlayerModel.AddArtefact();
        Destroy(gameObject);
    }
}
