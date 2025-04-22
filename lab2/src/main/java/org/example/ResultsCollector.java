package org.example;

import java.util.LinkedList;
import java.util.Queue;

public class ResultsCollector {
    // zasób do zbierania wyników
    private final Queue<String> results = new LinkedList<>();

    public synchronized void addResult(String result) {
        results.add(result);
    }

    public synchronized Queue<String> getResults() {
        return new LinkedList<>(results);
    }

}
