
using Environment;
using UnityEngine;

public class StationAIComputer : MonoBehaviour, IInteractable {
    [SerializeField] private StationAIDialogueUI dialogueUI;

    public void OnInteract() {
        if(dialogueUI == null) {
            Debug.LogWarning(
                "Station AI computer has no dialogue UI assigned."
            );
            return;
        }

        dialogueUI.OpenDialogue();
    }
}
