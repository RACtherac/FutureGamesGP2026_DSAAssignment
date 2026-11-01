
using Environment.Diagnostic;
using System;
using System.Globalization;
using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "SearchWorkersByEntranceRangeTask",
        menuName = "Surveillance Tasks/Search Workers By Entrance Range"
    )]
    public class SearchWorkersByEntranceRangeTask : Task {
        [Header("Search Range")]
        [SerializeField] private string startDate;
        [SerializeField] private string endDate;

        public override object Execute(object input = null) {
            EmployeeData[] workers = input as EmployeeData[];

            if(workers == null) {
                Debug.LogError("No employee array received.");
                return null;
            }

            if(!DateTime.TryParse(
                startDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTime start)) {
                Debug.LogError("Invalid start date.");
                return null;
            }

            if(!DateTime.TryParse(
                endDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTime end)) {
                Debug.LogError("Invalid end date.");
                return null;
            }

            if(start > end) {
                Debug.LogError("Start date cannot be after end date.");
                return null;
            }

            EmployeeData[] results = TaskBenchmark.Measure(
                "Binary Search",
                () => FindWorkersInRange(workers, start, end)
            );

            Debug.Log($"Workers found: {results.Length}");

            for(int i = 0; i < results.Length; i++) {
                Debug.Log(
                    $"Result [{i}]: {results[i].name}, " +
                    $"Entrance: {results[i].lastEntranceInEngineRoom}"
                );
            }

            return results;
        }

        private EmployeeData[] FindWorkersInRange(
            EmployeeData[] workers,
            DateTime start,
            DateTime end) {
            // Find the first worker whose entrance is >= start.
            int first = LowerBound(workers, start);

            // Find the first worker whose entrance is > end.
            int last = UpperBound(workers, end);

            int count = last - first;

            if(count <= 0)
                return new EmployeeData[0];

            EmployeeData[] results = new EmployeeData[count];

            Array.Copy(
                workers,
                first,
                results,
                0,
                count
            );

            return results;
        }

        private int LowerBound(
            EmployeeData[] workers,
            DateTime target) {
            int low = 0;
            int high = workers.Length;

            while(low < high) {
                int middle = low + (high - low) / 2;

                int comparison = CompareEntrance(
                    workers[middle],
                    target
                );

                if(comparison < 0)
                    low = middle + 1;
                else
                    high = middle;
            }

            return low;
        }

        private int UpperBound(
            EmployeeData[] workers,
            DateTime target) {
            int low = 0;
            int high = workers.Length;

            while(low < high) {
                int middle = low + (high - low) / 2;

                int comparison = CompareEntrance(
                    workers[middle],
                    target
                );

                if(comparison <= 0)
                    low = middle + 1;
                else
                    high = middle;
            }

            return low;
        }

        private int CompareEntrance(
            EmployeeData worker,
            DateTime target) {
            if(worker == null)
                return 1;

            bool valid = DateTime.TryParse(
                worker.lastEntranceInEngineRoom,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTime entrance
            );

            if(!valid)
                return 1;

            return entrance.CompareTo(target);
        }
    }
}
