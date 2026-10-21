
using System;
using Environment.Tasks;

namespace Utils {
    public enum EmployeeSortOption {
        Name,
        EmployeeID,
        LastEntranceInEngineRoom
    }

    public static class SortingUtils {
        public static void SelectionSort(int[] arrayToSort) {
            if(arrayToSort == null)
                return;

            for(int i = 0; i < arrayToSort.Length - 1; i++) {
                int currentSmallestIndex = i;

                for(int j = i + 1; j < arrayToSort.Length; j++) {
                    if(arrayToSort[j] < arrayToSort[currentSmallestIndex]) {
                        currentSmallestIndex = j;
                    }
                }

                int temp = arrayToSort[i];
                arrayToSort[i] = arrayToSort[currentSmallestIndex];
                arrayToSort[currentSmallestIndex] = temp;
            }
        }

        public static void SortEmployees(
            EmployeeData[] employees,
            EmployeeSortOption sortOption) {
            if(employees == null)
                return;

            for(int i = 0; i < employees.Length - 1; i++) {
                int selectedIndex = i;

                for(int j = i + 1; j < employees.Length; j++) {
                    if(CompareEmployees(
                        employees[j],
                        employees[selectedIndex],
                        sortOption) < 0) {
                        selectedIndex = j;
                    }
                }

                EmployeeData temp = employees[i];
                employees[i] = employees[selectedIndex];
                employees[selectedIndex] = temp;
            }
        }

        private static int CompareEmployees(
            EmployeeData first,
            EmployeeData second,
            EmployeeSortOption sortOption) {
            if(first == null && second == null)
                return 0;

            if(first == null)
                return 1;

            if(second == null)
                return -1;

            switch(sortOption) {
                case EmployeeSortOption.Name:
                    return string.Compare(
                        first.name,
                        second.name,
                        StringComparison.OrdinalIgnoreCase);

                case EmployeeSortOption.EmployeeID:
                    return first.employeeId.CompareTo(second.employeeId);

                case EmployeeSortOption.LastEntranceInEngineRoom:
                    DateTime firstDate;
                    DateTime secondDate;

                    bool firstValid = DateTime.TryParse(
                        first.lastEntranceInEngineRoom, out firstDate);

                    bool secondValid = DateTime.TryParse(
                        second.lastEntranceInEngineRoom, out secondDate);

                    if(!firstValid && !secondValid)
                        return 0;

                    if(!firstValid)
                        return 1;

                    if(!secondValid)
                        return -1;

                    return firstDate.CompareTo(secondDate);

                default:
                    return 0;
            }
        }
    }
}
