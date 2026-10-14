
using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "InsertEmployeesIntoArrayTask",
        menuName = "Surveillance Tasks/Insert Employees Into Array"
    )]
    public class InsertEmployeesIntoArrayTask : Task {
        private EmployeeData[] employees;

        public override object Execute(object input = null) {
            EmployeeData[] incomingEmployees =
                input as EmployeeData[];

            if(incomingEmployees == null) {
                Debug.LogError(
                    "InsertEmployeesIntoArrayTask received no employee array."
                );
                return null;
            }

            employees = new EmployeeData[incomingEmployees.Length];

            for(int i = 0; i < incomingEmployees.Length; i++) {
                employees[i] = incomingEmployees[i];

                Debug.Log(
                    $"Array[{i}]: {employees[i].name}, " +
                    $"ID: {employees[i].employeeId}, " +
                    $"Engine clearance: " +
                    $"{employees[i].hasEngineRoomClearance}"
                );
            }

            Debug.Log($"Inserted {employees.Length} employees into the array.");

            return employees;
        }
    }
}
