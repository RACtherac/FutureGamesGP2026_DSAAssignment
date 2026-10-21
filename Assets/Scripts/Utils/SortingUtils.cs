using System.Runtime.ExceptionServices;

namespace Utils {
    public static class SortingUtils {
        public static void SelectionSort(int[] arrayToSort)
        {
            for(int i = 0; i < arrayToSort.Length - 1; i++){
                int currentsmallestIndex = i;
                for(int j = i + 1; j < arrayToSort.Length; j++) {
                    if(arrayToSort[j] < arrayToSort[i]) {
                        currentsmallestIndex = j;
                        
                    }

                }
                int temp = arrayToSort[i];
                arrayToSort[i] = arrayToSort[currentsmallestIndex];
                arrayToSort[currentsmallestIndex] = temp;
            }
        }

        }
    }


