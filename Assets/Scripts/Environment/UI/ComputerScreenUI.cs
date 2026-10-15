
using System.Text;
using TMPro;
using UnityEngine;

namespace Environment.Tasks {
    public class ComputerScreenUI : MonoBehaviour {
        [SerializeField] private GameObject employeePanel;
        [SerializeField] private TMP_Text employeeListText;

        public void ShowEmployees(EmployeeData[] employees) {
            if(employees == null) {
                Debug.LogWarning("No employee data to display.");
                return;
            }

            StringBuilder output = new StringBuilder();
            output.AppendLine("YPSILON 13 - EMPLOYEE DATABASE");
            output.AppendLine("--------------------------------");

            foreach(EmployeeData employee in employees) {
                output.AppendLine($"Name: {employee.name}");
                output.AppendLine($"Employee ID: {employee.employeeId}");
                output.AppendLine(
                    $"Engine Room Clearance: " +
                    $"{(employee.hasEngineRoomClearance ? "YES" : "NO")}"
                );
                output.AppendLine(
                    $"Last Entrance: {employee.lastEntranceInEngineRoom}"
                );
                output.AppendLine(
                    $"Last Exit: {employee.lastExitFromEngineRoom}"
                );
                output.AppendLine("--------------------------------");
            }

            employeeListText.text = output.ToString();

            if(employeePanel != null)
                employeePanel.SetActive(true);
        }
    }
}
