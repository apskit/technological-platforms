package org.example;

public class PrimalityTestThread implements Runnable {
    private final TaskManager taskManager;
    private final ResultsCollector collector;
    private boolean interrupt = false;

    public PrimalityTestThread(TaskManager taskManager, ResultsCollector resultCollector) {
        this.taskManager = taskManager;
        this.collector = resultCollector;
    }

    @Override
    public void run() {
        while (!interrupt) {
            try {
                int numberToCheck = taskManager.getTask();
                boolean isPrime = isPrime(numberToCheck);
                Thread.sleep(1000);
                String result = buildResult(numberToCheck, isPrime);
                collector.addResult(result);
            } catch(InterruptedException e){
                interrupt = true;
            }
        }
    }

    private boolean isPrime(int number) {
        if (number <= 1) {
            return false;
        }
        for (int i = 2; i < number; i++) {
            if (number % i == 0) {
                return false;
            }
        }
        return true;
    }

    private String buildResult(int number, boolean isPrime) {
        String result;
        if (isPrime) {
            result = "prime: " + number;
        }
        else {
            result = "NOT prime: " + number;
        }
        return result;
    }
}
