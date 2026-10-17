using System;
using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "SortWorkersByEntranceTask",
        menuName = "Surveillance Tasks/Sort Workers By Entrance"
    )]
    public class SortWorkersByEntranceTask : Task {
        public override object Execute(object input = null) {
            EmployeeData[] workers = input as EmployeeData[];

            if(workers == null) {
                Debug.LogError("SortWorkersByEntranceTask: No workers array received.");
                return null;
            }

            // Bubble Sort: oldest entrance first.
            for(int i = 0; i < workers.Length - 1; i++) {
                for(int j = 0; j < workers.Length - 1 - i; j++) {
                    DateTime firstDate;
                    DateTime secondDate;

                    bool firstValid = DateTime.TryParse(
                        workers[j].lastEntranceInEngineRoom,
                        out firstDate
                    );

                    bool secondValid = DateTime.TryParse(
                        workers[j + 1].lastEntranceInEngineRoom,
                        out secondDate
                    );

                    if(firstValid && secondValid && firstDate > secondDate) {
                        EmployeeData temp = workers[j];
                        workers[j] = workers[j + 1];
                        workers[j + 1] = temp;
                    }
                }
            }

            return workers;
        }
    }
}
