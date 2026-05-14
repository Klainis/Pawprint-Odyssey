using TMPro;
using UnityEngine;

public class ArtefactCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;
    private int artefactCount;

    private void OnEnable()
    {
        artefactCount = PlayerView.Instance.PlayerModel.ArtefactCollected;
        counterText.text = $"{artefactCount}/3";
    }
}
