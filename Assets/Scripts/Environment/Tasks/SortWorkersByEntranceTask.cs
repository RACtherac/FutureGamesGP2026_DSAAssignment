using UnityEngine;
using Utils;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "SortWorkersByEntranceTask",
        menuName = "Surveillance Tasks/Sort Workers By Entrance"
    )]
    public class SortWorkersByEntranceTask : Task {
        public override object Execute(object input = null) {
            EmployeeData[] workers = input as EmployeeData[];

            if(workers == null) {
                Debug.LogError(
                    "SortWorkersByEntranceTask: No workers array received."
                );

                return null;
            }

            SortingUtils.SortEmployees(
                workers,
                EmployeeSortOption.LastEntranceAscending
            );

            for(int i = 0; i < workers.Length; i++) {
                Debug.Log(
                    $"Sorted Array[{i}]: {workers[i].name}, " +
                    $"Last entrance: {workers[i].lastEntranceInEngineRoom}"
                );
            }


            return workers;
        }
    }
}
