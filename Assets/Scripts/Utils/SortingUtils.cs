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

        public static void MergeSort(int[] arrayToSort) {
            if(arrayToSort == null || arrayToSort.Length < 2)
                return;

            int[] temp = new int[arrayToSort.Length];

            MergeSort(arrayToSort, temp, 0, arrayToSort.Length - 1);
        }

        private static void MergeSort(
            int[] array,
            int[] temp,
            int left,
            int right) {
            if(left >= right)
                return;

            int middle = left + (right - left) / 2;

            MergeSort(array, temp, left, middle);
            MergeSort(array, temp, middle + 1, right);

            Merge(array, temp, left, middle, right);
        }

        private static void Merge(
            int[] array,
            int[] temp,
            int left,
            int middle,
            int right) {
            int i = left;
            int j = middle + 1;
            int k = left;

            while(i <= middle && j <= right) {
                if(array[i] <= array[j])
                    temp[k++] = array[i++];
                else
                    temp[k++] = array[j++];
            }

            while(i <= middle)
                temp[k++] = array[i++];

            while(j <= right)
                temp[k++] = array[j++];

            for(int index = left; index <= right; index++)
                array[index] = temp[index];
        }

        public static void SortEmployees(
            EmployeeData[] employees,
            EmployeeSortOption sortOption) {
            if(employees == null || employees.Length < 2)
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

            EmployeeData[] temp = new EmployeeData[employees.Length];

            MergeSortEmployees(
                employees,
                temp,
                0,
                employees.Length - 1,
                baseOption);

            if(descending)
                Array.Reverse(employees);
        }

        private static void MergeSortEmployees(
            EmployeeData[] employees,
            EmployeeData[] temp,
            int left,
            int right,
            EmployeeSortOption sortOption) {
            if(left >= right)
                return;

            int middle = left + (right - left) / 2;

            MergeSortEmployees(
                employees, temp, left, middle, sortOption);

            MergeSortEmployees(
                employees, temp, middle + 1, right, sortOption);

            MergeEmployees(
                employees, temp, left, middle, right, sortOption);
        }

        private static void MergeEmployees(
            EmployeeData[] employees,
            EmployeeData[] temp,
            int left,
            int middle,
            int right,
            EmployeeSortOption sortOption) {
            int i = left;
            int j = middle + 1;
            int k = left;

            while(i <= middle && j <= right) {
                if(CompareEmployees(
                    employees[i],
                    employees[j],
                    sortOption) <= 0) {
                    temp[k++] = employees[i++];
                }
                else {
                    temp[k++] = employees[j++];
                }
            }

            while(i <= middle)
                temp[k++] = employees[i++];

            while(j <= right)
                temp[k++] = employees[j++];

            for(int index = left; index <= right; index++)
                employees[index] = temp[index];
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
                        first.name,
                        second.name,
                        StringComparison.OrdinalIgnoreCase);

                case EmployeeSortOption.EmployeeIDAscending:
                    return first.employeeId.CompareTo(
                        second.employeeId);

                case EmployeeSortOption.LastEntranceAscending:

                    bool firstValid = DateTime.TryParse(
                        first.lastEntranceInEngineRoom,
                        out DateTime firstDate);

                    bool secondValid = DateTime.TryParse(
                        second.lastEntranceInEngineRoom,
                        out DateTime secondDate);

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
//quicsort
//no dot sort
//happpen through a task 
