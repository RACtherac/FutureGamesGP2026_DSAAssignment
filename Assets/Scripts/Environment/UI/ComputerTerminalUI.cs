//To long ;(
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Environment.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

public class ComputerTerminalUI : MonoBehaviour {

    [Header("Terminal")]
    [SerializeField] private GameObject terminalPanel;
    [SerializeField] private GameObject terminalLogo;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text footerText;
    [SerializeField] private GameObject loadingBarContainer;
    [SerializeField] private RectTransform progressBarFill;
    [SerializeField] private float lineDelay = 0.45f;
    [SerializeField] private float loginDelay = 1f;

    [Header("Employee Database")]
    [SerializeField] private GameObject employeeDatabasePanel;
    [SerializeField] private TMP_Text employeeCountText;
    [SerializeField] private TMP_Text employeeDataText;
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private ScrollRect employeeScrollRect;
    [SerializeField] private int employeesPerTextBlock = 40;

    [Header("Sorting")]
    [SerializeField] private TMP_Dropdown sortDropdown;

    private Task taskToExecute;
    private ComputerTerminal activeTerminal;
    private EmployeeData[] allEmployees = new EmployeeData[0];

    private Coroutine bootCoroutine;
    private EmployeeSortOption currentSort = EmployeeSortOption.NameAscending;

    private readonly List<TMP_Text> employeeTextBlocks = new();

    private void Awake() {

        SetActive(terminalPanel, false);
        SetActive(employeeDatabasePanel, false);
        SetActive(terminalLogo, false);

        SetActive(loadingBarContainer, true);

        if(statusText != null)
            statusText.gameObject.SetActive(true);

        if(employeeDataText != null)
            employeeTextBlocks.Add(employeeDataText);

        SetupSortDropdown();

        if(searchInput != null)
            searchInput.onValueChanged.AddListener(SearchEmployees);
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

    public void OpenTerminal(Task task, ComputerTerminal computerTerminal) {

        if(task == null) {
            Debug.LogError("No task received.", this);
            return;
        }

        taskToExecute = task;
        activeTerminal = computerTerminal;

        if(bootCoroutine != null)
            StopCoroutine(bootCoroutine);

        SetActive(terminalPanel, true);
        SetActive(employeeDatabasePanel, false);
        SetActive(loadingBarContainer, true);
        SetActive(terminalLogo, true);

        if(statusText != null) {
            statusText.gameObject.SetActive(true);
            statusText.text = "";
        }

        if(headerText != null)
            headerText.text = "YPSILON 13 // SECURITY TERMINAL";

        if(footerText != null)
            footerText.text = "INITIALIZING...";

        SetProgress(0);

        bootCoroutine = StartCoroutine(BootSequence());
    }

    private IEnumerator BootSequence() {

        string[] lines = {
            "BIOS CHECK ................. OK",
            "MEMORY TEST ................ OK",
            "PERSONNEL MODULE ........... OK",
            "CONNECTING TO DATABASE .....",
            "VERIFYING USER .............",
            "ACCESS GRANTED"
        };

        for(int i = 0; i < lines.Length; i++) {

            if(statusText != null)
                statusText.text += (i > 0 ? "\n" : "") + lines[i];

            SetProgress((float)(i + 1) / lines.Length);

            yield return new WaitForSecondsRealtime(lineDelay);
        }

        yield return new WaitForSecondsRealtime(loginDelay);

        LoadEmployeeData();
        bootCoroutine = null;
    }

    private void SetProgress(float value) {

        if(progressBarFill == null)
            return;

        Vector3 scale = progressBarFill.localScale;
        scale.x = Mathf.Clamp01(value);
        progressBarFill.localScale = scale;
    }

    private void LoadEmployeeData() {

        if(taskToExecute == null) {
            Debug.LogError("No task to execute.", this);
            return;
        }

        allEmployees = taskToExecute.Execute() as EmployeeData[]
                      ?? new EmployeeData[0];

        Debug.Log($"[Terminal] Loaded {allEmployees.Length} employees.");

        if(statusText != null)
            statusText.gameObject.SetActive(false);

        SetActive(loadingBarContainer, false);
        SetActive(terminalLogo, false);
        SetActive(employeeDatabasePanel, true);

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

        sortDropdown.AddOptions(new List<string> {
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

    private void OnSortChanged(int index) {

        currentSort = GetSortOption();

        SearchEmployees(searchInput != null ? searchInput.text : "");
    }

    private EmployeeSortOption GetSortOption() {

        if(sortDropdown == null)
            return EmployeeSortOption.NameAscending;

        switch(sortDropdown.value) {
            case 1: return EmployeeSortOption.NameDescending;
            case 2: return EmployeeSortOption.EmployeeIDAscending;
            case 3: return EmployeeSortOption.EmployeeIDDescending;
            case 4: return EmployeeSortOption.LastEntranceAscending;
            case 5: return EmployeeSortOption.LastEntranceDescending;
            default: return EmployeeSortOption.NameAscending;
        }
    }

    private void SearchEmployees(string query) {

        List<EmployeeData> filtered = new();

        foreach(EmployeeData employee in allEmployees) {

            if(employee == null)
                continue;

            if(string.IsNullOrWhiteSpace(query) ||
               employee.name.IndexOf(query.Trim(), System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               employee.employeeId.ToString().Contains(query.Trim())) {

                filtered.Add(employee);
            }
        }

        EmployeeData[] results = filtered.ToArray();

        SortingUtils.SortEmployees(results, currentSort);
        DisplayEmployees(results);
    }

    private void DisplayEmployees(EmployeeData[] employees) {

        int blockSize = Mathf.Max(1, employeesPerTextBlock);

        int requiredBlocks = Mathf.Max(1,
            Mathf.CeilToInt((float)employees.Length / blockSize));

        EnsureEmployeeTextBlocks(requiredBlocks);

        for(int b = 0; b < employeeTextBlocks.Count; b++) {

            TMP_Text text = employeeTextBlocks[b];

            if(text == null)
                continue;

            text.gameObject.SetActive(b < requiredBlocks);

            if(b >= requiredBlocks)
                continue;

            StringBuilder output = new();

            int start = b * blockSize;
            int end = Mathf.Min(start + blockSize, employees.Length);

            for(int i = start; i < end; i++) {

                EmployeeData e = employees[i];

                output.AppendLine("----------------------------------------");
                output.AppendLine($"NAME: {e.name}");
                output.AppendLine($"EMPLOYEE ID: {e.employeeId}");
                output.AppendLine($"ENGINE ROOM CLEARANCE: {e.hasEngineRoomClearance}");
                output.AppendLine($"LAST ENTRANCE: {e.lastEntranceInEngineRoom}");
                output.AppendLine($"LAST EXIT: {e.lastExitFromEngineRoom}");
            }

            if(employees.Length == 0)
                output.AppendLine("NO MATCHING EMPLOYEES FOUND.");

            text.text = output.ToString();
        }

        if(employeeCountText != null)
            employeeCountText.text = $"RECORDS FOUND: {employees.Length}";

        if(employeeScrollRect != null) {

            Canvas.ForceUpdateCanvases();

            if(employeeScrollRect.content != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(employeeScrollRect.content);

            Canvas.ForceUpdateCanvases();
            employeeScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void EnsureEmployeeTextBlocks(int required) {

        while(employeeTextBlocks.Count < required) {

            GameObject obj = Instantiate(
                employeeDataText.gameObject,
                employeeDataText.transform.parent,
                false
            );

            obj.name = $"EmployeeDataText_{employeeTextBlocks.Count + 1}";

            TMP_Text text = obj.GetComponent<TMP_Text>();

            RectTransform rect = obj.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            text.text = "";
            employeeTextBlocks.Add(text);
        }
    }

    public void CloseImmediately() {

        if(bootCoroutine != null) {
            StopCoroutine(bootCoroutine);
            bootCoroutine = null;
        }

        SetActive(terminalPanel, false);
        SetActive(employeeDatabasePanel, false);
        SetActive(terminalLogo, false);
        SetActive(loadingBarContainer, false);

        if(statusText != null)
            statusText.gameObject.SetActive(false);

        taskToExecute = null;
        activeTerminal = null;
        allEmployees = new EmployeeData[0];
    }

    private void SetActive(GameObject obj, bool active) {

        if(obj != null)
            obj.SetActive(active);
    }

    private void OnDestroy() {

        if(searchInput != null)
            searchInput.onValueChanged.RemoveListener(SearchEmployees);

        if(sortDropdown != null)
            sortDropdown.onValueChanged.RemoveListener(OnSortChanged);
    }
}
