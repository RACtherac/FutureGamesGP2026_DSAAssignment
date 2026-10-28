using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "InsertEmployeesIntoArrayTask",
        menuName = "Surveillance Tasks/Insert Employees Into Array"
    )]
    public class InsertEmployeesIntoArrayTask : Task {
        [Header("Next Task")]
        [SerializeField] private Task nextTask;

        private EmployeeData[] employees;

        public override object Execute(object input = null) {
            EmployeeData[] incomingEmployees = input as EmployeeData[];

            if(incomingEmployees == null) {
                Debug.LogError(
                    "InsertEmployeesIntoArrayTask: No employee array received."
                );
                return null;
            }

            employees = new EmployeeData[incomingEmployees.Length];

            for(int i = 0; i < incomingEmployees.Length; i++) {
                employees[i] = incomingEmployees[i];
            }

            Debug.Log(
                $"InsertEmployeesIntoArrayTask: Copied {employees.Length} employees."
            );

            if(nextTask != null) {
                Debug.Log(
                    $"Passing employees to next task: {nextTask.name}"
                );

                return nextTask.Execute(employees);
            }

            Debug.LogWarning(
                "InsertEmployeesIntoArrayTask: No next task assigned."
            );

            return employees;
        }
    }
}
