
using Environment.Tasks;
using UnityEngine;

public class ComputerTerminal : MonoBehaviour {
    [Header("Terminal UI")]
    [SerializeField] private ComputerTerminalUI terminalUI;

    [Header("Player Scripts To Disable")]
    [Tooltip("Assign movement and interaction scripts that should stop while using the computer.")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private bool terminalOpen;
    private bool[] previousEnabledStates;

    private void Awake() {
        previousEnabledStates =
            new bool[scriptsToDisable.Length];

        if(terminalUI != null) {
            terminalUI.CloseImmediately();
        }
    }

    public void OpenTerminal(Task task) {
        if(terminalOpen)
            return;

        if(terminalUI == null) {
            Debug.LogError(
                "ComputerTerminal: Terminal UI is not assigned!",
                this
            );
            return;
        }

        if(task == null) {
            Debug.LogError(
                "ComputerTerminal: Task is missing!",
                this
            );
            return;
        }

        terminalOpen = true;

        for(int i = 0; i < scriptsToDisable.Length; i++) {
            MonoBehaviour script = scriptsToDisable[i];

            if(script == null)
                continue;

            previousEnabledStates[i] = script.enabled;
            script.enabled = false;
        }

        terminalUI.OpenTerminal(task, this);
    }

    public void CloseTerminal() {
        if(!terminalOpen)
            return;

        terminalOpen = false;

        if(terminalUI != null) {
            terminalUI.CloseImmediately();
        }

        for(int i = 0; i < scriptsToDisable.Length; i++) {
            MonoBehaviour script = scriptsToDisable[i];

            if(script == null)
                continue;

            script.enabled = previousEnabledStates[i];
        }
    }
}
