package org.example;

import java.util.LinkedList;
import java.util.Queue;

public class TaskManager {
    // zasób do zgłaszania zadań
    private final Queue<Integer> tasks = new LinkedList<>(); // primes to check

    public synchronized void addTask(int number) {
        tasks.add(number);
        notify();
    }

    public synchronized int getTask() throws InterruptedException {
        while (tasks.isEmpty()) {
            wait();
        }
        return tasks.remove();
    }

    public synchronized void endAllTasks() { // wake?
        notifyAll();
    }
}
