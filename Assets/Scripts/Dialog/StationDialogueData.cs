using System;

[Serializable]
public class StationDialogueFile {
    public string startNodeId;
    public StationDialogueNode[] nodes;
}

[Serializable]
public class StationDialogueNode {
    public string id;
    public string speaker;
    public string text;
    public StationDialogueChoice[] choices;
}

[Serializable]
public class StationDialogueChoice {
    public string text;
    public string nextNodeId;
    public string actionId;
}
