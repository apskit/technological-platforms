package org.example;

import java.util.*;

public class Mage  implements Comparable<Mage>{
    private final String name;
    private int level;
    private double power;
    private Set<Mage> apprentices;

    public Mage(String name, int level, double power, String sortingMode, PowerComparator comparator) {
        this.name = name;
        this.level = level;
        this.power = power;

        if ("natural".equals(sortingMode)) {
            this.apprentices  = new TreeSet<>(); // sortowanie z naturalnym porządkiem
        }
        else if ("alternative".equals(sortingMode)) {
            this.apprentices  = new TreeSet<>(comparator); // Sortowanie z alternatywnym kryterium
        }
        else {
            this.apprentices  = new HashSet<>(); // brak sortowania
        }
    }

    public void addApprentice(Mage apprentice) {
        this.apprentices.add(apprentice);
    }

    @Override
    public boolean equals(Object mage) {
        if (mage.hashCode() == this.hashCode()) {
            return true;
        }
        return false;
    }

    @Override
    public int hashCode() {
        return Objects.hash(name, level, power);
    }

    // Comparable interface
    @Override
    public int compareTo(Mage toCompare) {
        // name, level, power
        int result = this.name.compareTo(toCompare.name);
        if (result == 0) {
            result = Integer.compare(this.level, toCompare.level);
            if (result == 0) {
                result = Double.compare(this.power, toCompare.power);
            }
        }
        return result;
    }

    public String toString() {
        return "Mage{name='" + name + "', level=" + level + ", power=" + power + "}";
    }

    public void print(int structureLevel) {
        StringBuilder structureElement= new StringBuilder("-");

        //for(int i = 0; i < structureLevel; i++) {
        //    structureElement.append("--");
        //}
        structureElement.append("-".repeat(Math.max(0, structureLevel)));

        System.out.println(structureElement.toString() + this);

        for(Mage nextApprentice : apprentices) {
            nextApprentice.print(structureLevel + 1);
        }
    }

    public Integer getApprenticesStats(Map<Mage, Integer> apprenticesMap) {
        Integer apprenticesCount = 0;

        for (Mage apprentice : apprentices) {
            apprenticesCount++;
            apprenticesCount += apprentice.getApprenticesStats(apprenticesMap);
        }

        apprenticesMap.put(this, apprenticesCount);

        return apprenticesCount;
    }

    public void printApprenticesStats(Map<Mage, Integer> apprenticesMap) {
        this.getApprenticesStats(apprenticesMap); // budowa mapy statystyki potomków
        System.out.println();
        for (Map.Entry<Mage, Integer> entry : apprenticesMap.entrySet()) {
            System.out.println(entry.getKey() + " " + entry.getValue());
        }
    }

    public static class PowerComparator implements Comparator<Mage> {
        @Override
        public int compare(Mage mage1, Mage mage2) {
            //
            int result = Double.compare(mage1.power, mage2.power);
            return result;
        }
    }
}
