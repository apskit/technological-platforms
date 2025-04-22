package org.example;

import java.util.*;

public class Main {

    public static void main(String[] args) {

        String sortingMode = args[0];
        Mage.PowerComparator comparatorInstance;
        Map<Mage, Integer> apprenticesMap = new HashMap<>();

        if ("alternative".equals(sortingMode)) {
            comparatorInstance = new Mage.PowerComparator();  // sortowanie z alternatywnym kryterium
        }
        else {
            comparatorInstance = null; // sortowanie z naturalnym porządkiem / brak sortowania
        }

        Mage mage1 = new Mage("Merlin", 5, 20.0, sortingMode, comparatorInstance);
        Mage mage2 = new Mage("Altar", 10, 12.0, sortingMode, comparatorInstance);
        Mage mage3 = new Mage("Ravenna", 4, 22.0, sortingMode, comparatorInstance);
        Mage mage4 = new Mage("Thaddeus", 6, 21.0, sortingMode, comparatorInstance);
        Mage mage5 = new Mage("Seraphina", 3, 2.0, sortingMode, comparatorInstance);
        Mage mage6 = new Mage("Azriel", 10, 14.0, sortingMode, comparatorInstance);
        Mage mage7 = new Mage("Sylas", 3, 13.0, sortingMode, comparatorInstance);
        Mage mage8 = new Mage("Isolde", 1, 7.0, sortingMode, comparatorInstance);
        Mage mage9 = new Mage("Morgana", 2, 9.0, sortingMode, comparatorInstance);
        Mage mage10 = new Mage("Celestine", 8, 5.0, sortingMode, comparatorInstance);

        mage1.addApprentice(mage3); // Ravenna
        mage1.addApprentice(mage2); // Altar
        mage2.addApprentice(mage6); // Azriel
        mage3.addApprentice(mage4); // Thaddeus
        mage3.addApprentice(mage5); // Seraphina
        mage7.addApprentice(mage9); // Morgana
        mage7.addApprentice(mage10); // Celestine
        mage9.addApprentice(mage8); // Isolde

        mage1.print(0);
        mage7.print(0);

        mage1.getApprenticesStats(apprenticesMap); // zbuduj mapę dla mage1
        mage7.printApprenticesStats(apprenticesMap); // uzupelnij i drukuj
    }
}