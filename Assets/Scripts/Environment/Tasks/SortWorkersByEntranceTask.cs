
using UnityEngine;
using Utils;
using Environment.Diagnostic;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "SortWorkersByEntranceTask",
        menuName = "Surveillance Tasks/Sort Workers By Entrance"
    )]
    public class SortWorkersByEntranceTask : Task {
        [Header("Next Task")]
        [SerializeField] private Task nextTask;

        public override object Execute(object input = null) {
            EmployeeData[] workers = input as EmployeeData[];

            if(workers == null) {
                Debug.LogError(
                    "SortWorkersByEntranceTask: No workers array received."
                );

                return null;
            }

            // Benchmark Merge Sort
            TaskBenchmark.Measure(
                "Merge Sort - Entrance Time",
                () => {
                    SortingUtils.SortEmployees(
                        workers,
                        EmployeeSortOption.LastEntranceAscending
                    );

                    return workers;
                }
            );

            // Display sorted employees
            for(int i = 0; i < workers.Length; i++) {
                Debug.Log(
                    $"Sorted Array[{i}]: {workers[i].name}, " +
                    $"Last entrance: {workers[i].lastEntranceInEngineRoom}"
                );
            }

            // Pass sorted employees to the next task
            if(nextTask != null) {
                return nextTask.Execute(workers);
            }

            return workers;
        }
    }
}
