using UnityEngine;

public class InvestigationPanelController : MonoBehaviour {
    [SerializeField] private GameObject investigationPanel;

    private void Awake() {
        if(investigationPanel != null)
            investigationPanel.SetActive(false);
    }

    public void TogglePanel() {
        if(investigationPanel == null)
            return;

        investigationPanel.SetActive(
            !investigationPanel.activeSelf
        );
    }

    public void ClosePanel() {
        if(investigationPanel != null)
            investigationPanel.SetActive(false);
    }
}
