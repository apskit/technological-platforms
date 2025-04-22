package org.example;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.Scanner;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Client {
    private static final Logger logger = Logger.getLogger(Client.class.getName());

    public static void main(String[] args) {
        String serverAddress = "localhost";
        int port = 12345;

        try (Socket socket = new Socket(serverAddress, port);
             ObjectOutputStream out = new ObjectOutputStream(socket.getOutputStream());
             ObjectInputStream in = new ObjectInputStream(socket.getInputStream());
             Scanner scanner = new Scanner(System.in)) {

            logger.info("Connected to server.");

            String response = (String) in.readObject();
            logger.info("Server: " + response);

            logger.info("Enter the number of messages to send:");
            int numOfMessages = Integer.parseInt(scanner.nextLine());

            out.writeObject(numOfMessages);

            response = (String) in.readObject();
            logger.info("Server: " + response);

            for (int i = 1; i <= numOfMessages; i++) {
                logger.info("Enter message " + i + ":");
                String userInput = scanner.nextLine();
                Message message = new Message(i, userInput);
                out.writeObject(message);
            }
        } catch (IOException | ClassNotFoundException e) {
            logger.log(Level.SEVERE, "Client exception: " + e.getMessage(), e);
        }
    }
}
