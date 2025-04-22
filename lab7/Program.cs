using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

// serializacja:
// https://learn.microsoft.com/en-us/dotnet/core/compatibility/serialization/7.0/binaryformatter-apis-produce-errors#enableunsafebinaryformatterserialization-property
// MyProject.csproj

static class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("nie podano ścieżki do katalogu");
            return;
        }

        string directoryPath = args[0];

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("podana ścieżka jest niepoprawna");
            return;
        }

        DisplayDirectoryContent(directoryPath, 0);

        DirectoryInfo directory = new DirectoryInfo(directoryPath);
        DateTime oldestElementDate = directory.GetOldestElementDate();
        Console.WriteLine($"\nData najstarszego elementu w katalogu: {oldestElementDate}");

        string dosAttributes = directory.GetDosAttributes();
        Console.WriteLine($"Atrybuty DOS'owe: {dosAttributes}");

        SortedDictionary<string, long> directoryContent = LoadContentToCollection(directory);
        SerializeCollection(directoryContent, "collection.bin");
        SortedDictionary<string, long> deserializedCollection = DeserializeCollection("collection.bin");
    }

    // zawartość katalogu
    static void DisplayDirectoryContent(string directoryPath, int level)
    {
        string[] files = Directory.GetFiles(directoryPath);
        string[] directories = Directory.GetDirectories(directoryPath);

        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            Console.WriteLine(new string(' ', level * 4) + Path.GetFileName(file) + " " + fileInfo.Length + " bajtów " + GetDosAttributes(fileInfo));
        }

        foreach (string directory in directories)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            FileSystemInfo[] directoryElements = directoryInfo.GetFileSystemInfos(); // tablica elementów
            
            Console.WriteLine(new string(' ', level * 4) + Path.GetFileName(directory) + " (" + directoryElements.Length + ") " + GetDosAttributes(directoryInfo));
            DisplayDirectoryContent(directory, level + 1); // wywołanie rekurencyjne
        }
    }

    // najstarszy element
    static DateTime GetOldestElementDate(this DirectoryInfo directory)
    {
        return directory.GetDirectories("*", SearchOption.AllDirectories) // pobranie wszystkich katalogów
                        .Select(dir => dir.CreationTimeUtc) // pobranie dat utworzenia katalogów
                        .Concat(directory.GetFiles().Select(file => file.CreationTimeUtc)) // ... i plików
                        .Min(); // wybranie najmniejszej wartości
    }

    // atrybuty dosowe
    public static string GetDosAttributes(this FileSystemInfo fileSystemInfo)
    {
        string attributes = "";
        if ((fileSystemInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            attributes += "r";
        else
            attributes += "-";
        if ((fileSystemInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
            attributes += "a";
        else
            attributes += "-";
        if ((fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            attributes += "h";
        else
            attributes += "-";
        if ((fileSystemInfo.Attributes & FileAttributes.System) == FileAttributes.System)
            attributes += "s";
        else
            attributes += "-";
        return attributes;
    }

    // kolekcja
    static SortedDictionary<string, long> LoadContentToCollection(DirectoryInfo directory)
    {
        SortedDictionary<string, long> sortedCollection = new SortedDictionary<string, long>(new CustomComparer());

        foreach (var file in directory.GetFiles())
        {
            sortedCollection.Add(file.Name, file.Length);
        }

        foreach (var dir in directory.GetDirectories())
        {
            sortedCollection.Add(dir.Name, dir.GetFileSystemInfos().Length);
        }

        return sortedCollection;
    }

    [Serializable]
    class CustomComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int lengthComparison = x.Length.CompareTo(y.Length);
            if (lengthComparison != 0)
            {
                return lengthComparison;
            }
            return string.Compare(x, y);
        }
    }

    // serializacja
    static void SerializeCollection(SortedDictionary<string, long> collection, string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, collection);
        }
        Console.WriteLine("\nkolekcja została zserializowana");
    }

    // deserializacja
    static SortedDictionary<string, long> DeserializeCollection(string filePath)
    {
        SortedDictionary<string, long> deserializedCollection = null;
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            deserializedCollection = (SortedDictionary<string, long>)binaryFormatter.Deserialize(fileStream);
        }
        Console.WriteLine("kolekcja została zdeserializowana");

        foreach (var element in deserializedCollection)
        {
            Console.WriteLine($"{element.Key} -> {element.Value}");
        }

        return deserializedCollection;
    }
}
