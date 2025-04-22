package org.example;

import org.hibernate.sql.Select;

import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;
import javax.persistence.Persistence;
import java.util.List;
import java.util.Objects;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        EntityManagerFactory entityManagerFactory = Persistence.createEntityManagerFactory("myPersistenceUnit");
        EntityManager entityManager = entityManagerFactory.createEntityManager();

        // dodawanie 1 //
        entityManager.getTransaction().begin();

        Tower tower1 = new Tower();
        tower1.setName("Tower 1");
        tower1.setHeight(100);

        Mage mage1 = new Mage();
        mage1.setName("Gandalf");
        mage1.setLevel(22);
        mage1.setTower(tower1);

        Mage mage2 = new Mage();
        mage2.setName("Merlin");
        mage2.setLevel(18);
        mage2.setTower(tower1);

        tower1.getMages().add(mage1);
        tower1.getMages().add(mage2);

        // zapis obiektów w bazie
        entityManager.persist(tower1);
        entityManager.persist(mage1);
        entityManager.persist(mage2);

        entityManager.getTransaction().commit();

        // wyświetlanie wpisow
        printAllEntries(entityManager);


        // dodawanie 2 //
        entityManager.getTransaction().begin();
        Tower tower2 = new Tower();
        tower2.setName("Tower 2");
        tower2.setHeight(50);

        Mage mage3 = new Mage();
        mage3.setName("mag 3");
        mage3.setLevel(7);
        mage3.setTower(tower2);

        entityManager.persist(tower2);
        entityManager.persist(mage3);
        entityManager.getTransaction().commit();
        printAllEntries(entityManager);


        // usuwanie //
        String towerToDelete = "Tower 1";
        deleteTower(entityManager, towerToDelete);
        printAllEntries(entityManager);



        while(true) {
            String userInput = scanner.nextLine();

            if(Objects.equals(userInput, "exit")) {
                break;
            }
            else if(Objects.equals(userInput, "add tower")) {
                String name = scanner.nextLine();
                Integer height = Integer.valueOf(scanner.nextLine());
                addTower(entityManager, name, height);
            }
            else if(Objects.equals(userInput, "add mage")) {
                String name = scanner.nextLine();
                Integer height = Integer.valueOf(scanner.nextLine());
                String towerName = scanner.nextLine();
                addMage(entityManager, name, height, towerName);
            }
            else if(Objects.equals(userInput, "print")) {
                printAllEntries(entityManager);
            }
            else if(Objects.equals(userInput, "delete tower")) {
                towerToDelete = scanner.nextLine();
                deleteTower(entityManager, towerToDelete);
            }
            else if(Objects.equals(userInput, "delete mage")) {
                String mageToDelete = scanner.nextLine();
                deleteMage(entityManager, mageToDelete);
            }
            else if(Objects.equals(userInput, "level >")) {
                Integer level = Integer.valueOf(scanner.nextLine());
                printAllMages(entityManager, level);
            }
        }

        entityManager.close();
        entityManagerFactory.close();
    }

    public static void printAllEntries(EntityManager entityManager) {
        List<Mage> mages = entityManager.createQuery("FROM Mage", Mage.class).getResultList();
        List<Tower> towers = entityManager.createQuery("FROM Tower", Tower.class).getResultList();

        System.out.println("\nwieże:");
        for (Tower tower : towers) {
            System.out.println(tower.getName() + ", " + tower.getHeight());
        }

        System.out.println("\nmagowie:");
        for (Mage mage : mages) {
            System.out.println(mage.getName() + ", " + mage.getLevel() + ", wieża: " + mage.getTower().getName());
        }
    }

    public static void printAllMages(EntityManager entityManager, Integer level) {
        List<Mage> mages = entityManager.createQuery("SELECT m FROM Mage m  WHERE m.level > :level", Mage.class).setParameter("level", level).getResultList();
        System.out.println("\nmagowie:");
        for (Mage mage : mages) {
            System.out.println(mage.getName() + ", " + mage.getLevel() + ", wieża: " + mage.getTower().getName());
        }
    }

    public static void deleteTower(EntityManager entityManager, String name) {
        entityManager.getTransaction().begin();
        Tower towerToDelete = (Tower) entityManager.createQuery("SELECT t FROM Tower t  WHERE t.name = :name", Tower.class).setParameter("name", name).getSingleResult();
        List<Mage> mages = entityManager.createQuery("SELECT m FROM Mage m  WHERE m.tower = :tower", Mage.class).setParameter("tower", towerToDelete).getResultList();
        for (Mage mage : mages) {
            entityManager.remove(mage);
        }
        entityManager.remove(towerToDelete);
        entityManager.getTransaction().commit();
    }

    public static void deleteMage(EntityManager entityManager, String name) {
        entityManager.getTransaction().begin();
        Mage mageToDelete = (Mage) entityManager.createQuery("SELECT m FROM Mage m WHERE m.name = :name", Mage.class).setParameter("name", name).getSingleResult();
        entityManager.remove(mageToDelete);
        entityManager.getTransaction().commit();
    }

    public static void addTower(EntityManager entityManager, String name, Integer height) {
        entityManager.getTransaction().begin();
        Tower tower = new Tower();
        tower.setName(name);
        tower.setHeight(height);
        entityManager.persist(tower);
        entityManager.getTransaction().commit();
    }

    public static void addMage(EntityManager entityManager, String name, Integer level, String towerName) {
        entityManager.getTransaction().begin();
        Mage mage = new Mage();
        mage.setName(name);
        mage.setLevel(level);
        Tower tower = (Tower) entityManager.createQuery("SELECT t FROM Tower t  WHERE t.name = :name", Tower.class).setParameter("name", towerName).getSingleResult();
        mage.setTower(tower);
        entityManager.persist(mage);
        entityManager.getTransaction().commit();
    }
}
