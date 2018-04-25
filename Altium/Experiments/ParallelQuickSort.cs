using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Experiments
{
    public class ParallelSort
    {
        public static void QuicksortSequential<T>(IList<T> arr, IComparer<T> comparer)
        {
            QuicksortSequential(arr, 0, arr.Count - 1, comparer);
        }

        public static void QuicksortParallel<T>(IList<T> arr, IComparer<T> comparer)
        {
            QuicksortParallel(arr, 0, arr.Count - 1, comparer);
        }

        private static void QuicksortSequential<T>(IList<T> arr, int left, int right, IComparer<T> comparer)
        {
            if (right > left)
            {
                int pivot = Partition(arr, left, right, comparer);
                QuicksortSequential(arr, left, pivot - 1, comparer);
                QuicksortSequential(arr, pivot + 1, right, comparer);
            }
        }

        private static void QuicksortParallel<T>(IList<T> arr, int left, int right, IComparer<T> comparer)
        {
            const int SEQUENTIAL_THRESHOLD = 2048;
            if (right > left)
            {
                if (right - left < SEQUENTIAL_THRESHOLD)
                {
                    QuicksortSequential(arr, left, right, comparer);
                }
                else
                {
                    int pivot = Partition(arr, left, right, comparer);
                    Parallel.Invoke(new Action[]
                    {
                        delegate { QuicksortParallel(arr, left, pivot - 1, comparer); },
                        delegate { QuicksortParallel(arr, pivot + 1, right, comparer); }
                    });
                }
            }
        }

        private static void Swap<T>(IList<T> arr, int i, int j)
        {
            T tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }

        private static int Partition<T>(IList<T> arr, int low, int high, IComparer<T> comparer)
        {
            // Simple partitioning implementation
            int pivotPos = (high + low) / 2;
            T pivot = arr[pivotPos];
            Swap(arr, low, pivotPos);

            int left = low;
            for (int i = low + 1; i <= high; i++)
            {
                if (comparer.Compare(arr[i], pivot) < 0)
                {
                    left++;
                    Swap(arr, i, left);
                }
            }

            Swap(arr, low, left);
            return left;
        }
    }
}