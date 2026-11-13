using Environment;
using UnityEngine;

public class StationAIComputer : MonoBehaviour, IInteractable {
    [SerializeField] private StationAIDialogueUI dialogueUI;

    public void OnInteract() {
        dialogueUI.OpenDialogue();
    }
}
