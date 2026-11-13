using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InvestigationObjective {
    public string id;
    public string title;
    public bool completed;
}

[System.Serializable]
public class InvestigationEvidence {
    public string id;
    public string title;

    [TextArea(2, 5)]
    public string description;
}

public class InvestigationManager : MonoBehaviour {

    public static InvestigationManager Instance { get; private set; }

    [Header("Case File Panel")]
    [SerializeField] private GameObject investigationPanel;

    [Header("Close")]
    [SerializeField] private Button closeCaseFileButton;

    [Header("Tab Buttons")]
    [SerializeField] private Button overviewButton;
    [SerializeField] private Button evidenceButton;
    [SerializeField] private Button personnelButton;
    [SerializeField] private Button timelineButton;

    [Header("Tab Panels")]
    [SerializeField] private GameObject overviewTab;
    [SerializeField] private GameObject evidenceTab;
    [SerializeField] private GameObject personnelTab;
    [SerializeField] private GameObject timelineTab;

    [Header("Overview")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("Evidence")]
    [SerializeField] private TMP_Text evidenceText;

    [Header("Investigation Objectives")]
    [SerializeField]
    private List<InvestigationObjective> objectives =
        new List<InvestigationObjective>();

    private readonly List<InvestigationEvidence> evidence =
        new List<InvestigationEvidence>();

    private void Awake() {

        if(Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if(closeCaseFileButton != null)
            closeCaseFileButton.onClick.AddListener(CloseCaseFile);
        else
            Debug.LogError("[Investigation] Close button is not assigned!");

        if(overviewButton != null)
            overviewButton.onClick.AddListener(ShowOverviewTab);

        if(evidenceButton != null)
            evidenceButton.onClick.AddListener(ShowEvidenceTab);

        if(personnelButton != null)
            personnelButton.onClick.AddListener(ShowPersonnelTab);

        if(timelineButton != null)
            timelineButton.onClick.AddListener(ShowTimelineTab);

        if(investigationPanel != null)
            investigationPanel.SetActive(false);

        UpdateUI();

        SetActiveTab(overviewTab);
    }

    public void OpenCaseFile() {

        if(investigationPanel == null) {
            Debug.LogError(
                "[Investigation] Panel is not assigned."
            );
            return;
        }

        UpdateUI();

        investigationPanel.SetActive(true);

        ShowOverviewTab();

        Debug.Log("[Investigation] Case File opened.");
    }
    public void CloseCaseFile() {

        if(investigationPanel != null)
            investigationPanel.SetActive(false);

        Debug.Log("[Investigation] Case File closed.");
    }
    public void ShowOverviewTab() {
        Debug.Log("[Investigation] Overview button clicked!");
        SetActiveTab(overviewTab);
    }

    public void ShowEvidenceTab() {
        Debug.Log("[Investigation] Evidence button clicked!");
        UpdateUI();
        SetActiveTab(evidenceTab);
    }

    public void ShowPersonnelTab() {
        Debug.Log("[Investigation] Personnel button clicked!");
        SetActiveTab(personnelTab);
    }

    public void ShowTimelineTab() {
        Debug.Log("[Investigation] Timeline button clicked!");
        SetActiveTab(timelineTab);
    }
    private void SetActiveTab(GameObject selectedTab) {

        if(overviewTab != null)
            overviewTab.SetActive(selectedTab == overviewTab);

        if(evidenceTab != null)
            evidenceTab.SetActive(selectedTab == evidenceTab);

        if(personnelTab != null)
            personnelTab.SetActive(selectedTab == personnelTab);

        if(timelineTab != null)
            timelineTab.SetActive(selectedTab == timelineTab);
    }
    public void CompleteObjective(string objectiveId) {

        InvestigationObjective objective =
            objectives.Find(o => o.id == objectiveId);

        if(objective == null || objective.completed)
            return;

        objective.completed = true;

        Debug.Log(
            "[Investigation] Completed: " + objective.title
        );

        UpdateUI();
    }
    public void AddEvidence(
        string evidenceId,
        string title,
        string description
    ) {

        if(string.IsNullOrWhiteSpace(evidenceId))
            return;

        if(evidence.Exists(e => e.id == evidenceId))
            return;

        evidence.Add(new InvestigationEvidence {
            id = evidenceId,
            title = title,
            description = description
        });

        Debug.Log("[Investigation] New evidence: " + title);

        UpdateUI();
    }
    public bool HasEvidence(string evidenceId) {
        return evidence.Exists(e => e.id == evidenceId);
    }
    public bool IsObjectiveComplete(string objectiveId) {

        InvestigationObjective objective =
            objectives.Find(o => o.id == objectiveId);

        return objective != null && objective.completed;
    }
    private void UpdateUI() {

        if(objectiveText != null) {

            StringBuilder output = new StringBuilder();

            foreach(InvestigationObjective objective in objectives) {

                string status =
                    objective.completed ? "[X]" : "[ ]";

                output.AppendLine(
                    status + " " + objective.title
                );
            }

            objectiveText.text = output.ToString();
        }

        if(evidenceText != null) {

            StringBuilder output = new StringBuilder();

            foreach(InvestigationEvidence item in evidence) {

                output.AppendLine("> " + item.title);
                output.AppendLine(item.description);
                output.AppendLine();
            }

            if(evidence.Count == 0)
                output.AppendLine("NO EVIDENCE COLLECTED.");

            evidenceText.text = output.ToString();
        }
    }
    private void OnDestroy() {

        if(Instance == this)
            Instance = null;
    }
}
