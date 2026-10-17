
using System;
using System.Collections;
using System.Text;
using Environment.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerTerminalUI : MonoBehaviour {
    [Header("Terminal Panel")]
    [SerializeField] private GameObject terminalPanel;

    [Header("Terminal Text")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text employeeCountText;
    [SerializeField] private TMP_Text employeeDataText;
    [SerializeField] private TMP_Text footerText;

    [Header("Loading Bar")]
    [SerializeField] private RectTransform progressBarFill;

    [Header("Animation Settings")]
    [SerializeField] private float lineDelay = 0.45f;
    [SerializeField] private float loginDelay = 1f;

    private Task taskToExecute;
    private ComputerTerminal activeTerminal;

    private Coroutine bootCoroutine;
    private bool terminalOpen;

    private void Awake() {
        CloseImmediately();
    }

    private void Update() {
        if(!terminalOpen)
            return;

        if(Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame) {
            if(activeTerminal != null) {
                activeTerminal.CloseTerminal();
            }
            else {
                CloseImmediately();
            }
        }
    }

    public void OpenTerminal(
        Task task,
        ComputerTerminal computerTerminal) {
        if(terminalOpen || task == null)
            return;

        taskToExecute = task;
        activeTerminal = computerTerminal;
        terminalOpen = true;

        if(terminalPanel != null)
            terminalPanel.SetActive(true);

        if(headerText != null)
            headerText.text = "VAULT-TEC // PERSONNEL TERMINAL";

        if(statusText != null)
            statusText.text = "";

        if(employeeCountText != null)
            employeeCountText.text = "";

        if(employeeDataText != null)
            employeeDataText.text = "";

        if(footerText != null)
            footerText.text = "INITIALIZING SYSTEM...";

        SetProgress(0f);

        if(bootCoroutine != null)
            StopCoroutine(bootCoroutine);

        bootCoroutine = StartCoroutine(BootSequence());
    }

    public void CloseImmediately() {
        if(bootCoroutine != null) {
            StopCoroutine(bootCoroutine);
            bootCoroutine = null;
        }

        terminalOpen = false;
        taskToExecute = null;
        activeTerminal = null;

        if(terminalPanel != null)
            terminalPanel.SetActive(false);
    }

    private IEnumerator BootSequence() {
        yield return TypeLine("BIOS CHECK ................. OK");

        SetProgress(0.15f);
        yield return TypeLine("MEMORY TEST ................ OK");

        SetProgress(0.30f);
        yield return TypeLine("PERSONNEL MODULE ........... LOADING");

        SetProgress(0.45f);
        yield return new WaitForSecondsRealtime(lineDelay);

        yield return TypeLine("CONNECTING TO DATABASE ..... OK");

        SetProgress(0.65f);
        yield return TypeLine("VERIFYING USER ............. ADMIN");

        SetProgress(0.80f);
        yield return new WaitForSecondsRealtime(loginDelay);

        yield return TypeLine("ACCESS GRANTED");

        SetProgress(1f);

        if(footerText != null)
            footerText.text = "LOADING PERSONNEL RECORDS...";

        yield return new WaitForSecondsRealtime(lineDelay);

        LoadEmployeeData();

        if(footerText != null)
            footerText.text = "PRESS ESC TO LOG OUT";

        bootCoroutine = null;
    }

    private IEnumerator TypeLine(string line) {
        if(statusText != null)
            statusText.text += line + "\n";

        yield return new WaitForSecondsRealtime(lineDelay);
    }

    private void SetProgress(float progress) {
        if(progressBarFill == null)
            return;

        Vector3 scale = progressBarFill.localScale;
        scale.x = Mathf.Clamp01(progress);
        progressBarFill.localScale = scale;
    }

    private void LoadEmployeeData() {
        if(taskToExecute == null) {
            ShowError("ERROR: NO TASK ASSIGNED");
            return;
        }

        try {
            // Execute the existing employee task pipeline once.
            object result = taskToExecute.Execute();

            EmployeeData[] employees = result as EmployeeData[];

            if(employees == null) {
                ShowError(
                    "ERROR: EMPLOYEE ARRAY NOT RETURNED"
                );

                Debug.LogWarning(
                    "ComputerTerminalUI: The task did not return " +
                    "an EmployeeData array. Check the return value " +
                    "of the final task in the pipeline."
                );

                return;
            }

            DisplayEmployees(employees);

            Debug.Log(
                "Terminal loaded " + employees.Length +
                " employee records."
            );
        }
        catch(Exception exception) {
            ShowError("DATABASE ERROR");
            Debug.LogException(exception);
        }
    }

    private void DisplayEmployees(EmployeeData[] employees) {
        StringBuilder output = new StringBuilder();

        output.AppendLine("========================================");
        output.AppendLine("          PERSONNEL DATABASE");
        output.AppendLine("========================================");
        output.AppendLine();

        int displayedCount = 0;

        for(int i = 0; i < employees.Length; i++) {
            EmployeeData employee = employees[i];

            if(employee == null)
                continue;

            displayedCount++;

            output.AppendLine(
                "RECORD: " + displayedCount.ToString("D4")
            );

            output.AppendLine(
                "EMPLOYEE ID: " + employee.employeeId
            );

            output.AppendLine(
                "NAME: " + employee.name
            );

            output.AppendLine(
                "ENGINE ROOM CLEARANCE: " +
                (employee.hasEngineRoomClearance
                    ? "AUTHORIZED"
                    : "DENIED")
            );

            output.AppendLine(
                "LAST ENGINE ROOM ENTRY: " +
                employee.lastEntranceInEngineRoom
            );

            output.AppendLine(
                "LAST ENGINE ROOM EXIT: " +
                employee.lastExitFromEngineRoom
            );

            output.AppendLine();
            output.AppendLine("----------------------------------------");
            output.AppendLine();
        }

        if(employeeCountText != null) {
            employeeCountText.text =
                "RECORDS FOUND: " + displayedCount;
        }

        if(employeeDataText != null) {
            employeeDataText.text = output.ToString();
        }

        if(statusText != null) {
            statusText.text += "\nDATABASE ACCESS COMPLETE";
        }
    }

    private void ShowError(string message) {
        if(statusText != null)
            statusText.text += "\n" + message;

        if(employeeCountText != null)
            employeeCountText.text = "DATABASE ERROR";
    }
}
