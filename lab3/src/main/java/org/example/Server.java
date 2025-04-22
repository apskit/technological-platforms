package org.example;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Server {
    private static final Logger logger = Logger.getLogger(Server.class.getName());

    public static void main(String[] args) {
        int port = 12345;
        try (ServerSocket serverSocket = new ServerSocket(port)) {
            logger.info("Server is waiting for clients...");

            while (true) {
                Socket clientSocket = serverSocket.accept(); // wait for client
                logger.info("Client connected: " + ((Socket) clientSocket).getInetAddress());

                Thread clientThread = new Thread(new MessageHandler(clientSocket));
                clientThread.start();
            }
        } catch (IOException e) {
            logger.log(Level.SEVERE, "Server exception: " + e.getMessage(), e);
        }
    }

    private static class MessageHandler implements Runnable {
        private final Socket clientSocket;
        private ObjectOutputStream out;
        private ObjectInputStream in;

        public MessageHandler(Socket socket) {
            this.clientSocket = socket;
        }

        @Override
        public void run() {
            try {
                out = new ObjectOutputStream(clientSocket.getOutputStream());
                in = new ObjectInputStream(clientSocket.getInputStream());

                out.writeObject("ready");

                int numOfMessages = (int) in.readObject();

                out.writeObject("ready for messages");

                for (int i = 1; i <= numOfMessages; i++) {
                    Message message = (Message) in.readObject();
                    logger.info("Message " + i + " received from client: " + message.getContent());
                }

                out.writeObject("finished");

            } catch (IOException | ClassNotFoundException e) {
                logger.log(Level.SEVERE, "ClientHandler exception: " + e.getMessage(), e);
            } finally {
                try {
                    clientSocket.close();
                } catch (IOException e) {
                    logger.log(Level.SEVERE, "Socket closing error: " + e.getMessage(), e);
                }
            }
        }
    }
}
