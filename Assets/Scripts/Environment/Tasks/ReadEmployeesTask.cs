
using System.IO;
using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "ReadEmployeesTask",
        menuName = "Surveillance Tasks/Read Employees"
    )]
    public class ReadEmployeesTask : Task {
        [SerializeField] private string fileName = "Employees.json";
        [SerializeField] private Task nextTask;

        public override object Execute(object input = null) {
            string filePath = Path.Combine(
                Application.streamingAssetsPath,
                fileName
            );
            string json = File.ReadAllText(filePath);

            EmployeeList data =
                JsonUtility.FromJson<EmployeeList>(json);

            if(nextTask != null) {
                return nextTask.Execute(data.employees);
            }

            return data.employees;
        }
    }
}
