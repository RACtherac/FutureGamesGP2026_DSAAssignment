
using System;
using System.Collections.Generic;

public class GenericQueue<T> {
    private readonly List<T> queue = new List<T>();

    public int Count => queue.Count;

    public bool IsEmpty => queue.Count == 0;

    // Add an element to the end of the queue.
    public void Enqueue(T newElement) {
        queue.Add(newElement);
    }

    // View the first element without removing it.
    public T Peek() {
        if(IsEmpty)
            throw new InvalidOperationException(
                "Queue is empty, cannot peek."
            );

        return queue[0];
    }

    // Remove and return the first element.
    public T Dequeue() {
        if(IsEmpty)
            throw new InvalidOperationException(
                "Queue is empty, cannot dequeue."
            );

        T firstElement = queue[0];
        queue.RemoveAt(0);

        return firstElement;
    }

    // Remove all elements.
    public void Clear() {
        queue.Clear();
    }
}
