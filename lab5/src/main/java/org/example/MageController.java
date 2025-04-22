package org.example;

import java.util.Optional;
public class MageController {
    private MageRepository repository;
    public MageController(MageRepository repository) {
        this.repository = repository;
    }
    public String find(String name) {
        Optional<Mage> mageOptional = repository.find(name);
        return mageOptional.map(mage -> mage.getName() + " " + mage.getLevel()).orElse("not found");
    }
    public String delete(String name) {
        try {
            repository.delete(name);
            return "done";
        } catch (IllegalArgumentException e) {
            return "not found";
        }
    }
    public String save(String name, Integer level) {
        try {
            repository.save(new Mage(name, level));
            return "done";
        } catch (IllegalArgumentException e) {
            return "bad request";
        }
    }

}
