using System;
using System.Collections.Generic;

public class GenericQueue<T> {
    private readonly List<T> queue = new List<T>();

    public int Count => queue.Count;

    public bool IsEmpty => queue.Count == 0;

    public void Enqueue(T newElement) {
        queue.Add(newElement);
    }
    public T Peek() {
        if(IsEmpty)
            throw new InvalidOperationException(
                "Queue is empty no peek."
            );

        return queue[0];
    }
    public T Dequeue() {
        if(IsEmpty)
            throw new InvalidOperationException(
                "Queue is empty"
            );

        T firstElement = queue[0];
        queue.RemoveAt(0);

        return firstElement;
    }
    public void Clear() {
        queue.Clear();
    }
}
