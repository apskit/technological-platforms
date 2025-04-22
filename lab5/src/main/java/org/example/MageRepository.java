package org.example;

import java.util.Collection;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;

public class MageRepository {
    private Map<String, Mage> collection;
    public MageRepository() {
        this.collection = new HashMap<>();
    }
    public Optional<Mage> find(String name) {
        return Optional.ofNullable(collection.get(name));
    }
    public void delete(String name) {
        if (!collection.containsKey(name)) {
            throw new IllegalArgumentException("Mage with name " + name + " not exist");
        }
        collection.remove(name);
    }
    public void save(Mage mage) {
        if (collection.containsKey(mage.getName())) {
            throw new IllegalArgumentException("Mage with name " + mage.getName() + " already exists");
        }
        collection.put(mage.getName(), mage);
    }
}
