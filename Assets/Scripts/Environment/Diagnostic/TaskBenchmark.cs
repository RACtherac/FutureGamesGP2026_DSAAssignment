using System;
using System.Diagnostics;
using UnityEngine;

namespace Environment.Diagnostic {
    public static class TaskBenchmark {
        public static T Measure<T>(string operationName, Func<T> operation) {
            Stopwatch stopwatch = Stopwatch.StartNew();

            T result = operation();

            stopwatch.Stop();

            UnityEngine.Debug.Log(
                operationName + " completed in " +
                stopwatch.Elapsed.TotalMilliseconds.ToString("F4") +
                " ms"
            );

            return result;
        }
    }
}
