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
                //Debug.LogError("No employee array received.");
                return null;
            }

            if(!DateTime.TryParse(startDate, out DateTime start) ||
               !DateTime.TryParse(endDate, out DateTime end) ||
               start > end) {
                //Debug.LogError("Invalid date range.");
                return null;
            }

            EmployeeData[] results = TaskBenchmark.Measure(
                "Binary Search",
                () => {
                    int first = BinarySearch(workers, start, false);
                    int last = BinarySearch(workers, end, true);

                    int count = last - first;

                    if(count <= 0)
                        return new EmployeeData[0];

                    EmployeeData[] found = new EmployeeData[count];
                    Array.Copy(workers, first, found, 0, count);

                    return found;
                }
            );

            //Debug.Log($"Workers found: {results.Length}");

            //foreach(EmployeeData worker in results) {
            //    Debug.Log($"{worker.name} - {worker.lastEntranceInEngineRoom}");
            //}

            return results;
        }
        private int BinarySearch(
            EmployeeData[] workers,
            DateTime target,
            bool findAfter) {

            int low = 0;
            int high = workers.Length;

            while(low < high) {

                int middle = low + (high - low) / 2;

                if(!DateTime.TryParse(
                    workers[middle].lastEntranceInEngineRoom,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTime entrance)) {
                    entrance = DateTime.MaxValue;
                }

                if(findAfter ? entrance <= target : entrance < target)
                    low = middle + 1;
                else
                    high = middle;
            }

            return low;
        }
    }
}
