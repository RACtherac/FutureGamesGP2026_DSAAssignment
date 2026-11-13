using UnityEngine;

[CreateAssetMenu(
    fileName = "OpenInvestigationTask",
    menuName = "Surveillance Tasks/Open Investigation"
)]
public class OpenInvestigationTask : Environment.Tasks.Task {

    public override object Execute(object input = null) {

        if(InvestigationManager.Instance == null) {
            Debug.LogError(
                "[Investigation] InvestigationManager not found."
            );
            return null;
        }

        InvestigationManager.Instance.OpenCaseFile();

        return null;
    }
}
