
using System;
using System.Collections.Generic;
using Environment.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class StationDialogueTaskBinding {
    public string actionId;
    public Task task;
}

public class StationAIDialogueUI : MonoBehaviour {
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;

    [Header("Dialogue Data")]
    [SerializeField]
    private string jsonResourcePath =
        "Dialogue/helper_dialogue";

    [Header("Optional Task Actions")]
    [SerializeField] private StationDialogueTaskBinding[] taskBindings;

    [Header("Player Controls")]
    [SerializeField] private Behaviour[] playerBehavioursToDisable;

    private StationDialogueFile dialogueFile;
    private readonly Dictionary<string, StationDialogueNode> nodesById =
        new Dictionary<string, StationDialogueNode>();

    private readonly List<GameObject> spawnedButtons =
        new List<GameObject>();

    private bool dialogueOpen;
    private bool[] previousEnabledStates;

    private void Awake() {
        LoadDialogue();

        if(dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update() {
        if(!dialogueOpen)
            return;

        if(Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame) {
            CloseDialogue();
        }
    }

    private void LoadDialogue() {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonResourcePath);

        if(jsonFile == null) {
            Debug.LogError(
                "Station AI dialogue JSON not found at Resources/" +
                jsonResourcePath + ".json"
            );
            return;
        }

        try {
            dialogueFile =
                JsonUtility.FromJson<StationDialogueFile>(jsonFile.text);

            if(dialogueFile == null || dialogueFile.nodes == null) {
                Debug.LogError("Station AI dialogue JSON is invalid.");
                return;
            }

            nodesById.Clear();

            foreach(StationDialogueNode node in dialogueFile.nodes) {
                if(node == null || string.IsNullOrWhiteSpace(node.id))
                    continue;

                if(nodesById.ContainsKey(node.id)) {
                    Debug.LogWarning(
                        "Duplicate dialogue node ID: " + node.id
                    );
                    continue;
                }

                nodesById.Add(node.id, node);
            }

            if(!nodesById.ContainsKey(dialogueFile.startNodeId)) {
                Debug.LogError(
                    "The dialogue start node does not exist: " +
                    dialogueFile.startNodeId
                );
            }
        }
        catch(Exception exception) {
            Debug.LogError("Could not load Station AI dialogue JSON.");
            Debug.LogException(exception);
        }
    }

    public void OpenDialogue() {
        if(dialogueOpen)
            return;

        if(dialogueFile == null ||
            !nodesById.ContainsKey(dialogueFile.startNodeId)) {
            Debug.LogError(
                "Station AI cannot open because dialogue data is missing."
            );
            return;
        }

        dialogueOpen = true;

        if(dialoguePanel != null)
            dialoguePanel.SetActive(true);

        SetPlayerControls(false);
        ShowNode(dialogueFile.startNodeId);
    }

    public void CloseDialogue() {
        if(!dialogueOpen)
            return;

        dialogueOpen = false;
        ClearChoiceButtons();

        if(dialoguePanel != null)
            dialoguePanel.SetActive(false);

        SetPlayerControls(true);
    }

    private void ShowNode(string nodeId) {
        if(!nodesById.TryGetValue(nodeId, out StationDialogueNode node)) {
            Debug.LogWarning(
                "Station AI dialogue node not found: " + nodeId
            );
            CloseDialogue();
            return;
        }

        if(speakerText != null)
            speakerText.text = node.speaker;

        if(dialogueText != null)
            dialogueText.text = node.text;

        ClearChoiceButtons();

        if(node.choices == null || node.choices.Length == 0) {
            CreateChoiceButton("Close terminal", CloseDialogue);
            return;
        }

        foreach(StationDialogueChoice choice in node.choices) {
            if(choice == null)
                continue;

            StationDialogueChoice selectedChoice = choice;

            CreateChoiceButton(
                selectedChoice.text,
                () => SelectChoice(selectedChoice)
            );
        }
    }

    private void SelectChoice(StationDialogueChoice choice) {
        if(!string.IsNullOrWhiteSpace(choice.actionId))
            ExecuteTaskAction(choice.actionId);

        if(string.IsNullOrWhiteSpace(choice.nextNodeId)) {
            CloseDialogue();
            return;
        }

        ShowNode(choice.nextNodeId);
    }

    private void CreateChoiceButton(string label, Action onClick) {
        if(choicesContainer == null || choiceButtonPrefab == null) {
            Debug.LogError(
                "Assign Choices Container and Choice Button Prefab."
            );
            return;
        }

        Button newButton = Instantiate(
            choiceButtonPrefab,
            choicesContainer
        );

        newButton.gameObject.SetActive(true);

        TMP_Text buttonText =
            newButton.GetComponentInChildren<TMP_Text>(true);

        if(buttonText != null)
            buttonText.text = label;

        newButton.onClick.AddListener(() => onClick.Invoke());

        spawnedButtons.Add(newButton.gameObject);
    }

    private void ClearChoiceButtons() {
        foreach(GameObject button in spawnedButtons) {
            if(button != null)
                Destroy(button);
        }

        spawnedButtons.Clear();
    }

    private void ExecuteTaskAction(string actionId) {
        if(taskBindings == null)
            return;

        foreach(StationDialogueTaskBinding binding in taskBindings) {
            if(binding == null ||
                binding.actionId != actionId ||
                binding.task == null) {
                continue;
            }

            try {
                object result = binding.task.Execute();

                Debug.Log(
                    "Station AI action '" + actionId +
                    "' executed. Result: " +
                    (result != null ? result.ToString() : "null")
                );
            }
            catch(Exception exception) {
                Debug.LogError(
                    "Station AI action failed: " + actionId
                );
                Debug.LogException(exception);
            }

            return;
        }

        Debug.LogWarning(
            "No Task asset is assigned for action ID: " + actionId
        );
    }

    private void SetPlayerControls(bool enableControls) {
        if(playerBehavioursToDisable == null)
            return;

        if(!enableControls) {
            previousEnabledStates =
                new bool[playerBehavioursToDisable.Length];

            for(int i = 0;
                 i < playerBehavioursToDisable.Length;
                 i++) {
                Behaviour behaviour = playerBehavioursToDisable[i];

                if(behaviour == null)
                    continue;

                previousEnabledStates[i] = behaviour.enabled;
                behaviour.enabled = false;
            }
        }
        else {
            for(int i = 0;
                 i < playerBehavioursToDisable.Length;
                 i++) {
                Behaviour behaviour = playerBehavioursToDisable[i];

                if(behaviour == null ||
                    previousEnabledStates == null ||
                    i >= previousEnabledStates.Length) {
                    continue;
                }

                behaviour.enabled = previousEnabledStates[i];
            }

            previousEnabledStates = null;
        }
    }

    private void OnDisable() {
        if(dialogueOpen) {
            dialogueOpen = false;
            SetPlayerControls(true);
        }
    }
}
