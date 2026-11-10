using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

[Serializable]
public class InvestigationObjective {
    public string id;
    public string title;
    public bool completed;
}

[Serializable]
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
    [SerializeField] private TMP_Text objectiveText;
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

        if(investigationPanel != null)
            investigationPanel.SetActive(false);

        UpdateUI();
    }

    public void OpenCaseFile() {
        if(investigationPanel == null)
            return;

        UpdateUI();
        investigationPanel.SetActive(true);
    }

    public void CloseCaseFile() {
        if(investigationPanel != null)
            investigationPanel.SetActive(false);
    }

    public void CompleteObjective(string objectiveId) {
        InvestigationObjective objective =
            objectives.Find(o => o.id == objectiveId);

        if(objective == null || objective.completed)
            return;

        objective.completed = true;

        Debug.Log("[Investigation] Completed: " + objective.title);

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
                string status = objective.completed ? "[X]" : "[ ]";

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
