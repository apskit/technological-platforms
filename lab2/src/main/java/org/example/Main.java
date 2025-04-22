package org.example;

import java.util.LinkedList;
import java.util.List;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        TaskManager taskManager = new TaskManager();
        ResultsCollector resultCollector = new ResultsCollector();
        List<Thread> threads = new LinkedList<>();

        int threadsNumber = 1;
        if (args.length == 1) {
            threadsNumber = Integer.parseInt(args[0]);
        }

        for (int i = 0; i < threadsNumber; i++) {
            Thread thread = new Thread(new PrimalityTestThread(taskManager, resultCollector));
            thread.start();
            threads.add(thread);
        }

        Scanner scanner = new Scanner(System.in);
        while(true)
        {
            System.out.println("Podaj liczbę do sprawdzenia lub 'q' aby zakończyć:");
            String input = scanner.nextLine();
            if (input.equals("q")) {
                System.out.println(resultCollector.getResults());

                for (Thread thread : threads) {
                    thread.interrupt();
                }
                taskManager.endAllTasks(); // wybudzenie wszystkich wątków
                break;
            }
            try {
                int number = Integer.parseInt(input);
                taskManager.addTask(number);
            } catch (NumberFormatException e) {
                System.out.println("Błąd: Wprowadź liczbę lub 'q' aby zakończyć.");
            }
        }

    }
}