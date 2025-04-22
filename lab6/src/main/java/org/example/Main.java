package org.example;

import org.apache.commons.lang3.tuple.Pair;

import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.Objects;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ForkJoinPool;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class Main {
    public static void main(String[] args) {
        if (args.length != 3) {
            System.out.println("Niepoprawna liczba argumentow");
            return;
        }

        String inputDirectoryPath = args[0];
        String outputDirectoryPath = args[1];
        int threadPoll = Integer.parseInt(args[2]);

        List<Path> files;
        Path source = Path.of(inputDirectoryPath);
        try {
            Stream<Path> stream = Files.list(source);
            files = stream.collect(Collectors.toList());
        } catch (IOException e) {
            throw new RuntimeException(e);
        }

        long time = System.currentTimeMillis();

        ForkJoinPool forkJoinPool = new ForkJoinPool(threadPoll);

        try {
            forkJoinPool.submit(() ->
                    files.parallelStream()
                            .map(path -> {
                                // para: nazwa, plik
                                try {
                                    BufferedImage image = ImageIO.read(path.toFile());
                                    return Pair.of(path.getFileName().toString(), image);
                                } catch (IOException e) {
                                    e.printStackTrace();
                                    return null;
                                }
                            })
                            .filter(Objects::nonNull)
                            .map(pair -> {
                                BufferedImage original = pair.getRight();
                                BufferedImage newImage = new BufferedImage(original.getWidth(), original.getHeight(), original.getType());

                                // modyfikacja obrazka
                                for (int i = 0; i < original.getWidth(); i++) {
                                    for (int j = 0; j < original.getHeight(); j++) {
                                        int rgb = original.getRGB(i, j);
                                        Color color = new Color(rgb);

                                        double red = (double)color.getRed()*0.299;
                                        double green = (double)color.getGreen()*0.587;
                                        double blue = (double)color.getBlue()*0.114;

                                        int grey = (int) (red + green + blue);

                                        Color newColor = new Color(grey, grey, grey);
                                        newImage.setRGB(i, j, newColor.getRGB());
                                    }
                                }
                                return Pair.of(pair.getLeft(), newImage);
                            })
                            .forEach(pair -> {
                                // zapis
                                try {
                                    Path outputPath = Path.of(outputDirectoryPath, pair.getLeft());
                                    ImageIO.write(pair.getRight(), "png", outputPath.toFile());
                                } catch (IOException e) {
                                    e.printStackTrace();
                                }
                            })
            ).get();
        } catch (InterruptedException | ExecutionException e) {
            throw new RuntimeException(e);
        }

        System.out.println(System.currentTimeMillis() - time + " ms");

        forkJoinPool.shutdown();
    }
}
