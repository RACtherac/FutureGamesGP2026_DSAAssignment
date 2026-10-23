
using Environment;
using Environment.Tasks;
using UnityEngine;

public class ComputerTerminal : MonoBehaviour, IInteractable {
    [Header("Task")]
    [SerializeField] private Task testTask;

    [Header("Terminal UI")]
    [SerializeField] private ComputerTerminalUI terminalUI;

    [Header("Scripts To Disable While Using Terminal")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private bool terminalOpen;
    private bool[] previousEnabledStates;

    private void Awake() {
        if(terminalUI == null) {
            Debug.LogError(
                "ComputerTerminal: Terminal UI reference is missing.",
                this
            );
        }
    }

    public void OnInteract() {
        if(terminalOpen)
            return;

        if(testTask == null) {
            Debug.LogError(
                "ComputerTerminal: No task assigned.",
                this
            );
            return;
        }

        OpenTerminal(testTask);
    }

    public void OpenTerminal(Task task) {
        if(terminalOpen)
            return;

        if(task == null) {
            Debug.LogError(
                "ComputerTerminal: Cannot open terminal without a task.",
                this
            );
            return;
        }

        if(terminalUI == null) {
            Debug.LogError(
                "ComputerTerminal: Terminal UI is missing.",
                this
            );
            return;
        }

        terminalOpen = true;

        // Remember the enabled state of each script.
        if(scriptsToDisable != null) {
            previousEnabledStates =
                new bool[scriptsToDisable.Length];

            for(int i = 0; i < scriptsToDisable.Length; i++) {
                if(scriptsToDisable[i] == null)
                    continue;

                previousEnabledStates[i] =
                    scriptsToDisable[i].enabled;

                scriptsToDisable[i].enabled = false;
            }
        }

        // The UI handles booting and executing the task.
        terminalUI.OpenTerminal(task, this);
    }

    public void CloseTerminal() {
        if(!terminalOpen)
            return;

        terminalOpen = false;

        // Restore each script to its previous state.
        if(scriptsToDisable != null &&
            previousEnabledStates != null) {
            for(int i = 0; i < scriptsToDisable.Length; i++) {
                if(scriptsToDisable[i] == null)
                    continue;

                scriptsToDisable[i].enabled =
                    previousEnabledStates[i];
            }
        }

        previousEnabledStates = null;

        if(terminalUI != null) {
            terminalUI.CloseImmediately();
        }
    }
}
