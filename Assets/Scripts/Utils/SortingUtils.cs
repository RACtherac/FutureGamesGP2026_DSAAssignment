
using System;
using Environment.Tasks;

namespace Utils {
    public enum EmployeeSortOption {
        NameAscending,
        NameDescending,
        EmployeeIDAscending,
        EmployeeIDDescending,
        LastEntranceAscending,
        LastEntranceDescending
    }

    public static class SortingUtils {
        public static void SelectionSort(int[] arrayToSort) {
            if(arrayToSort == null)
                return;

            for(int i = 0; i < arrayToSort.Length - 1; i++) {
                int currentSmallestIndex = i;

                for(int j = i + 1; j < arrayToSort.Length; j++) {
                    if(arrayToSort[j] < arrayToSort[currentSmallestIndex])
                        currentSmallestIndex = j;
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

            bool descending =
                sortOption == EmployeeSortOption.NameDescending ||
                sortOption == EmployeeSortOption.EmployeeIDDescending ||
                sortOption == EmployeeSortOption.LastEntranceDescending;

            EmployeeSortOption baseOption = sortOption;

            switch(sortOption) {
                case EmployeeSortOption.NameDescending:
                    baseOption = EmployeeSortOption.NameAscending;
                    break;
                case EmployeeSortOption.EmployeeIDDescending:
                    baseOption = EmployeeSortOption.EmployeeIDAscending;
                    break;
                case EmployeeSortOption.LastEntranceDescending:
                    baseOption = EmployeeSortOption.LastEntranceAscending;
                    break;
            }

            for(int i = 0; i < employees.Length - 1; i++) {
                int selectedIndex = i;

                for(int j = i + 1; j < employees.Length; j++) {
                    if(CompareEmployees(
                        employees[j], employees[selectedIndex], baseOption) < 0) {
                        selectedIndex = j;
                    }
                }

                EmployeeData temp = employees[i];
                employees[i] = employees[selectedIndex];
                employees[selectedIndex] = temp;
            }

            if(descending)
                Array.Reverse(employees);
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
                case EmployeeSortOption.NameAscending:
                    return string.Compare(
                        first.name, second.name,
                        StringComparison.OrdinalIgnoreCase);

                case EmployeeSortOption.EmployeeIDAscending:
                    return first.employeeId.CompareTo(second.employeeId);

                case EmployeeSortOption.LastEntranceAscending:
                    bool firstValid = DateTime.TryParse(
                        first.lastEntranceInEngineRoom, out DateTime firstDate);

                    bool secondValid = DateTime.TryParse(
                        second.lastEntranceInEngineRoom, out DateTime secondDate);

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
