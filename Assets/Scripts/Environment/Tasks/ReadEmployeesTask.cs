
using System.IO;
using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "ReadEmployeesTask",
        menuName = "Surveillance Tasks/Read Employees"
    )]
    public class ReadEmployeesTask : Task {
        [SerializeField] private string fileName = "employees.json";
        [SerializeField] private Task nextTask;

        public override object Execute(object input = null) {
            string filePath = Path.Combine(
                Application.streamingAssetsPath,
                fileName
            );

            if(!File.Exists(filePath)) {
                Debug.LogError($"Employee file not found: {filePath}");
                return null;
            }

            string json = File.ReadAllText(filePath);

            EmployeeList data =
                JsonUtility.FromJson<EmployeeList>(json);

            if(data == null || data.employees == null) {
                Debug.LogError("Could not parse employee JSON.");
                return null;
            }

            Debug.Log($"Loaded {data.employees.Length} employees.");

            if(nextTask != null) {
                return nextTask.Execute(data.employees);
            }

            return data.employees;
        }
    }
}
