
using System;
using System.Collections;
using System.Text;
using Environment.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ComputerTerminalUI : MonoBehaviour {
    [Header("Terminal Panel")]
    [SerializeField] private GameObject terminalPanel;

    [Header("Boot UI")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject loadingBarContainer;
    [SerializeField] private RectTransform progressBarFill;
    [SerializeField] private float lineDelay = 0.45f;
    [SerializeField] private float loginDelay = 1f;

    [Header("Employee Database UI")]
    [SerializeField] private GameObject employeeDatabasePanel;
    [SerializeField] private TMP_Text employeeCountText;
    [SerializeField] private TMP_Text employeeDataText;
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private ScrollRect employeeScrollRect;

    [Header("Footer")]
    [SerializeField] private TMP_Text footerText;

    private Task taskToExecute;
    private ComputerTerminal activeTerminal;
    private Coroutine bootCoroutine;
    private bool terminalOpen;

    private EmployeeData[] allEmployees = Array.Empty<EmployeeData>();

    private void Awake() {
        if(searchInput != null)
            searchInput.onValueChanged.AddListener(SearchEmployees);

        CloseImmediately();
    }

    private void OnDestroy() {
        if(searchInput != null)
            searchInput.onValueChanged.RemoveListener(SearchEmployees);
    }

    private void Update() {
        if(!terminalOpen)
            return;

        if(Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame) {
            if(activeTerminal != null)
                activeTerminal.CloseTerminal();
            else
                CloseImmediately();
        }
    }

    public void OpenTerminal(Task task, ComputerTerminal computerTerminal) {
        if(terminalOpen || task == null)
            return;

        taskToExecute = task;
        activeTerminal = computerTerminal;
        terminalOpen = true;
        allEmployees = Array.Empty<EmployeeData>();

        if(terminalPanel != null)
            terminalPanel.SetActive(true);

        if(headerText != null)
            headerText.text = "YPSILON 13 // SURVEILLACE TERMINAL";

        if(statusText != null) {
            statusText.gameObject.SetActive(true);
            statusText.text = "";
        }

        if(loadingBarContainer != null)
            loadingBarContainer.SetActive(true);

        if(employeeDatabasePanel != null)
            employeeDatabasePanel.SetActive(false);

        if(employeeCountText != null)
            employeeCountText.text = "";

        if(employeeDataText != null)
            employeeDataText.text = "";

        if(searchInput != null)
            searchInput.SetTextWithoutNotify("");

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
        allEmployees = Array.Empty<EmployeeData>();

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

        bool loaded = LoadEmployeeData();

        if(loaded) {
            // Hide boot UI after loading has finished.
            if(statusText != null)
                statusText.gameObject.SetActive(false);

            if(loadingBarContainer != null)
                loadingBarContainer.SetActive(false);

            // Show the database only after the boot UI is gone.
            if(employeeDatabasePanel != null)
                employeeDatabasePanel.SetActive(true);

            SearchEmployees(
                searchInput != null ? searchInput.text : ""
            );

            if(footerText != null)
                footerText.text = "PRESS ESC TO LOG OUT";
        }
        else {
            if(footerText != null)
                footerText.text = "DATABASE LOAD FAILED - PRESS ESC";
        }

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

    private bool LoadEmployeeData() {
        if(taskToExecute == null) {
            ShowError("ERROR: NO TASK ASSIGNED");
            return false;
        }

        try {
            object result = taskToExecute.Execute();
            EmployeeData[] employees = result as EmployeeData[];

            if(employees == null) {
                ShowError("ERROR: EMPLOYEE ARRAY NOT RETURNED");
                Debug.LogWarning(
                    "The task did not return an EmployeeData array. " +
                    "Check the final task in your pipeline."
                );
                return false;
            }

            allEmployees = employees;

            Debug.Log(
                "Terminal loaded " + allEmployees.Length +
                " employee records."
            );

            return true;
        }
        catch(Exception exception) {
            ShowError("DATABASE ERROR");
            Debug.LogException(exception);
            return false;
        }
    }

    private void SearchEmployees(string searchText) {
        if(employeeDataText == null)
            return;

        StringBuilder output = new StringBuilder();
        int displayedCount = 0;

        string query = string.IsNullOrWhiteSpace(searchText)
            ? ""
            : searchText.Trim();

        for(int i = 0; i < allEmployees.Length; i++) {
            EmployeeData employee = allEmployees[i];

            if(employee == null)
                continue;

            bool matches =
    string.IsNullOrWhiteSpace(query) ||
    (employee.name != null &&
     employee.name.IndexOf(
         query, StringComparison.OrdinalIgnoreCase
     ) >= 0) ||
    employee.employeeId.ToString().IndexOf(
        query, StringComparison.OrdinalIgnoreCase
    ) >= 0;

            if(!matches)
                continue;

            displayedCount++;

            output.AppendLine("========================================");
            output.AppendLine("RECORD: " + displayedCount.ToString("D4"));
            output.AppendLine("EMPLOYEE ID: " + employee.employeeId);
            output.AppendLine("NAME: " + employee.name);
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
            output.AppendLine("========================================");
            output.AppendLine();
        }

        employeeDataText.text = output.ToString();

        if(employeeCountText != null) {
            employeeCountText.text =
                "RECORDS FOUND: " + displayedCount;
        }

        // Refresh layout and return the scroll view to the top.
        Canvas.ForceUpdateCanvases();

        if(employeeScrollRect != null)
            employeeScrollRect.verticalNormalizedPosition = 1f;
    }

    private void ShowError(string message) {
        if(statusText != null) {
            statusText.gameObject.SetActive(true);
            statusText.text += "\n" + message;
        }

        if(employeeCountText != null)
            employeeCountText.text = "DATABASE ERROR";
    }
}
