using System.Collections;
using System.Collections.Generic;
using Environment.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

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

    [Header("Sorting")]
    [SerializeField] private TMP_Dropdown sortDropdown;

    [Header("Footer")]
    [SerializeField] private TMP_Text footerText;

    [Header("Terminal Logo")]
    [SerializeField] private GameObject terminalLogo;

    private Task taskToExecute;
    private ComputerTerminal activeTerminal;

    private EmployeeData[] allEmployees = new EmployeeData[0];

    private Coroutine bootCoroutine;

    private EmployeeSortOption currentSort =
        EmployeeSortOption.NameAscending;

    private void Awake() {
        if(terminalPanel != null)
            terminalPanel.SetActive(false);

        if(employeeDatabasePanel != null)
            employeeDatabasePanel.SetActive(false);

        if(terminalLogo != null)
            terminalLogo.SetActive(false);

        if(loadingBarContainer != null)
            loadingBarContainer.SetActive(true);

        if(statusText != null)
            statusText.gameObject.SetActive(true);

        SetupSortDropdown();

        if(searchInput != null) {
            searchInput.onValueChanged.AddListener(SearchEmployees);
        }
    }

    private void Update() {
        if(terminalPanel == null || !terminalPanel.activeSelf)
            return;

        if(Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame) {
            if(activeTerminal != null)
                activeTerminal.CloseTerminal();
            else
                CloseImmediately();
        }
    }

    public void OpenTerminal(
        Task task,
        ComputerTerminal computerTerminal) {
        if(task == null) {
            Debug.LogError(
                "ComputerTerminalUI: No task received.",
                this
            );
            return;
        }

        taskToExecute = task;
        activeTerminal = computerTerminal;

        if(bootCoroutine != null) {
            StopCoroutine(bootCoroutine);
            bootCoroutine = null;
        }

        if(terminalPanel != null)
            terminalPanel.SetActive(true);

        if(employeeDatabasePanel != null)
            employeeDatabasePanel.SetActive(false);

        if(loadingBarContainer != null)
            loadingBarContainer.SetActive(true);

        if(statusText != null) {
            statusText.gameObject.SetActive(true);
            statusText.text = "";
        }

        if(headerText != null)
            headerText.text = "YPSILON 13 // SECURITY TERMINAL";

        if(footerText != null)
            footerText.text = "INITIALIZING...";

        if(progressBarFill != null) {
            Vector3 scale = progressBarFill.localScale;
            scale.x = 0f;
            progressBarFill.localScale = scale;
        }

        if(terminalLogo != null)
            terminalLogo.SetActive(true);

        bootCoroutine = StartCoroutine(BootSequence());
    }

    private IEnumerator BootSequence() {
        string[] bootLines =
        {
            "BIOS CHECK ................. OK",
            "MEMORY TEST ................ OK",
            "PERSONNEL MODULE ........... OK",
            "CONNECTING TO DATABASE .....",
            "VERIFYING USER .............",
            "ACCESS GRANTED"
        };

        if(statusText != null)
            statusText.text = "";

        for(int i = 0; i < bootLines.Length; i++) {
            if(statusText != null) {
                if(i > 0)
                    statusText.text += "\n";

                statusText.text += bootLines[i];
            }

            SetProgress((float)(i + 1) / bootLines.Length);

            yield return new WaitForSecondsRealtime(lineDelay);
        }

        yield return new WaitForSecondsRealtime(loginDelay);

        LoadEmployeeData();

        bootCoroutine = null;
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
            Debug.LogError(
                "ComputerTerminalUI: No task to execute.",
                this
            );
            return;
        }

        object result = taskToExecute.Execute();

        allEmployees = result as EmployeeData[];

        if(allEmployees == null) {
            Debug.LogError(
                "ComputerTerminalUI: The task did not return EmployeeData[]. " +
                "Check the task's Execute() return value.",
                this
            );

            allEmployees = new EmployeeData[0];
        }

        if(statusText != null)
            statusText.gameObject.SetActive(false);

        if(loadingBarContainer != null)
            loadingBarContainer.SetActive(false);

        if(terminalLogo != null)
            terminalLogo.SetActive(false);

        if(employeeDatabasePanel != null)
            employeeDatabasePanel.SetActive(true);

        if(footerText != null)
            footerText.text = "PERSONNEL DATABASE // CONNECTED";

        if(searchInput != null)
            searchInput.SetTextWithoutNotify("");

        currentSort = GetSortOption();

        SearchEmployees("");
    }

    private void SetupSortDropdown() {
        if(sortDropdown == null)
            return;

        sortDropdown.ClearOptions();

        sortDropdown.AddOptions(new List<string>
        {
            "Name (A-Z)",
            "Name (Z-A)",
            "Employee ID (Low-High)",
            "Employee ID (High-Low)",
            "Last Entrance (Oldest-Newest)",
            "Last Entrance (Newest-Oldest)"
        });

        sortDropdown.SetValueWithoutNotify(0);
        sortDropdown.RefreshShownValue();

        sortDropdown.onValueChanged.AddListener(OnSortChanged);
    }

    private void OnSortChanged(int selectedIndex) {
        currentSort = GetSortOption();

        SearchEmployees(
            searchInput != null ? searchInput.text : ""
        );
    }

    private EmployeeSortOption GetSortOption() {
        if(sortDropdown == null)
            return EmployeeSortOption.NameAscending;

        switch(sortDropdown.value) {
            case 1:
                return EmployeeSortOption.NameDescending;

            case 2:
                return EmployeeSortOption.EmployeeIDAscending;

            case 3:
                return EmployeeSortOption.EmployeeIDDescending;

            case 4:
                return EmployeeSortOption.LastEntranceAscending;

            case 5:
                return EmployeeSortOption.LastEntranceDescending;

            default:
                return EmployeeSortOption.NameAscending;
        }
    }

    private void SearchEmployees(string searchText) {
        if(employeeDataText == null)
            return;

        List<EmployeeData> filteredEmployees =
            new List<EmployeeData>();

        string query = string.IsNullOrWhiteSpace(searchText)
            ? ""
            : searchText.Trim().ToLowerInvariant();

        foreach(EmployeeData employee in allEmployees) {
            if(employee == null)
                continue;

            string employeeName = employee.name ?? "";
            string employeeID = employee.employeeId.ToString();

            if(query == "" ||
                employeeName.ToLowerInvariant().Contains(query) ||
                employeeID.Contains(query)) {
                filteredEmployees.Add(employee);
            }
        }

        EmployeeData[] sortedEmployees =
            filteredEmployees.ToArray();

        SortingUtils.SortEmployees(
            sortedEmployees,
            currentSort
        );

        DisplayEmployees(sortedEmployees);
    }

    private void DisplayEmployees(EmployeeData[] employees) {
        System.Text.StringBuilder output =
            new System.Text.StringBuilder();

        for(int i = 0; i < employees.Length; i++) {
            EmployeeData employee = employees[i];

            output.AppendLine("----------------------------------------");
            output.AppendLine($"NAME: {employee.name}");
            output.AppendLine($"EMPLOYEE ID: {employee.employeeId}");
            output.AppendLine(
                $"ENGINE ROOM CLEARANCE: {employee.hasEngineRoomClearance}"
            );
            output.AppendLine(
                $"LAST ENTRANCE: {employee.lastEntranceInEngineRoom}"
            );
            output.AppendLine(
                $"LAST EXIT: {employee.lastExitFromEngineRoom}"
            );
            output.AppendLine();
        }

        if(employees.Length == 0)
            output.AppendLine("NO MATCHING EMPLOYEES FOUND.");

        employeeDataText.text = output.ToString();

        if(employeeCountText != null) {
            employeeCountText.text =
                $"RECORDS FOUND: {employees.Length}";
        }

        if(employeeScrollRect != null) {
            Canvas.ForceUpdateCanvases();
            employeeScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    public void CloseImmediately() {
        if(bootCoroutine != null) {
            StopCoroutine(bootCoroutine);
            bootCoroutine = null;
        }

        if(terminalPanel != null)
            terminalPanel.SetActive(false);

        if(employeeDatabasePanel != null)
            employeeDatabasePanel.SetActive(false);

        if(terminalLogo != null)
            terminalLogo.SetActive(false);

        if(loadingBarContainer != null)
            loadingBarContainer.SetActive(false);

        if(statusText != null)
            statusText.gameObject.SetActive(false);

        taskToExecute = null;
        activeTerminal = null;
        allEmployees = new EmployeeData[0];
    }

    private void OnDestroy() {
        if(searchInput != null)
            searchInput.onValueChanged.RemoveListener(SearchEmployees);

        if(sortDropdown != null)
            sortDropdown.onValueChanged.RemoveListener(OnSortChanged);
    }
}
