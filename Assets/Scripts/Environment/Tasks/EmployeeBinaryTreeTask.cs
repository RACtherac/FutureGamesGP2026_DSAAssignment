using System;
using System.Collections.Generic;
using Environment.Diagnostic;
using UnityEngine;

namespace Environment.Tasks {

    [CreateAssetMenu(
        fileName = "EmployeeBinaryTreeTask",
        menuName = "Surveillance Tasks/Employee Binary Tree"
    )]

    public class EmployeeBinaryTreeTask : Task {

        [SerializeField] private int searchEmployeeID;

        private class Node {
            public EmployeeData employee;
            public Node left, right;

            public Node(EmployeeData employee) {
                this.employee = employee;
            }
        }

        public override object Execute(object input = null) {

            EmployeeData[] employees = input as EmployeeData[];

            if(employees == null) {
                Debug.LogError("No employee array received.");
                return null;
            }

            EmployeeData[] sorted = Array.FindAll(
                employees, e => e != null
            );

            Array.Sort(sorted, (a, b) =>
                a.employeeId.CompareTo(b.employeeId)
            );

            Node root = BuildTree(sorted, 0, sorted.Length - 1);

            EmployeeData result = TaskBenchmark.Measure(
                "BFS Employee Search",
                () => BreadthFirstSearch(root, searchEmployeeID)
            );

            Debug.Log(result != null
                ? $"Employee found: {result.name}, ID: {result.employeeId}"
                : $"Employee {searchEmployeeID} not found.");

            return result != null
                ? new EmployeeData[] { result }
                : new EmployeeData[0];
        }

        private Node BuildTree(EmployeeData[] employees, int start, int end) {

            if(start > end)
                return null;

            int middle = start + (end - start) / 2;

            Node node = new Node(employees[middle]);

            node.left = BuildTree(employees, start, middle - 1);
            node.right = BuildTree(employees, middle + 1, end);

            return node;
        }

        private EmployeeData BreadthFirstSearch(Node root, int targetID) {

            if(root == null)
                return null;

            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(root);

            while(queue.Count > 0) {

                Node current = queue.Dequeue();

                if(current.employee.employeeId == targetID)
                    return current.employee;

                if(current.left != null)
                    queue.Enqueue(current.left);

                if(current.right != null)
                    queue.Enqueue(current.right);
            }

            return null;
        }
    }
}
