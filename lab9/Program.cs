using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

public class Program
{
    public static void Main()
    {
        List<Car> myCars = new List<Car>
        {
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };


        // 1. zapytania LINQ
        var query1 = myCars
            .Where(car => car.model == "A6")
            .Select(car => new
            {
                engineType = car.motor.model == "TDI" ? "diesel" : "petrol",
                hppl = car.motor.horsePower / car.motor.displacement
            });

        var query2 = query1
            .GroupBy(car => car.engineType)
            .Select(group => new
            {
                EngineType = group.Key,
                AverageHppl = group.Average(car => car.hppl)
            });

        foreach (var group in query2)
        {
            Console.WriteLine($"{group.EngineType}: {group.AverageHppl}");
        }


        // 2. serializacja i deserializacja
        SerializeCarsToXml(myCars, "CarsCollection.xml");


        // 3. XPath
        XElement rootNode = XElement.Load("CarsCollection.xml");
        var avgHP = rootNode.XPathSelectElements("//car[engine/@model != 'TDI']/engine/horsePower").Select(hp => (double)hp).Average();

        Console.WriteLine($"Przeciętna moc samochodów o silnikach innych niż TDI: {avgHP}");

        // wyrażenie 2
        IEnumerable<XElement> models = rootNode.XPathSelectElements("//car/model");
        var distinctModels = models.Select(m => m.Value).Distinct();

        Console.WriteLine("modele samochodów:");
        foreach (var model in distinctModels)
        {
            Console.WriteLine(model);
        }


        // 4. zapytanie LINQ
        CreateXmlFromLinq(myCars);


        // 5. XHTML
        CreateXhtmlFromLinq(myCars);


        // 6. modyfikacja XML-a
        ModifyXml();
    }


    // MODEL DANYCH

    [XmlType("car")]
    public class Car
    {
        public string model { get; set; }
        public int year { get; set; }
        [XmlElement("engine")]
        public Engine motor { get; set; }

        public Car(string model, Engine motor, int year)
        {
            this.model = model;
            this.motor = motor;
            this.year = year;
        }

        public Car() { }
    }

    public class Engine
    {
        public double displacement { get; set; }
        public double horsePower { get; set; }
        [XmlAttribute("model")]
        public string model { get; set; }

        public Engine(double displacement, double horsePower, string model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }

        public Engine() { }
    }


    // serializacja
    static void SerializeCarsToXml(List<Car> cars, string fileName)
    {
        XmlSerializer serializer = new(typeof(List<Car>), new XmlRootAttribute("cars"));
        using TextWriter writer = new StreamWriter(fileName);
        serializer.Serialize(writer, cars);
    }

    // deserializacja
    static List<Car> DeserializeCarsFromXml(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
        using TextReader reader = new StreamReader(fileName);
        return (List<Car>)serializer.Deserialize(reader);
    }


    // zapytanie LINQ
    private static void CreateXmlFromLinq(List<Car> myCars)
    {
        // tworzenie węzłów XML
        IEnumerable<XElement> nodes = myCars.Select(car =>
            new XElement("car",
                new XElement("model", car.model),
                new XElement("year", car.year),
                new XElement("engine",
                    new XAttribute("model", car.motor.model),
                    new XElement("displacement", car.motor.displacement),
                    new XElement("horsePower", car.motor.horsePower)
                )
            )
        );

        // węzeł zawierający wyniki zapytania
        XElement rootNode = new XElement("cars", nodes);
        rootNode.Save("CarsFromLinq.xml");
    }


    // LINQ to HTML
    private static void CreateXhtmlFromLinq(List<Car> myCars)
    {
        XDocument xhtmlDoc = XDocument.Load("template.html");

        XElement table = new XElement("table",
            new XElement("tr",
                new XElement("th", "Model"),
                new XElement("th", "Year"),
                new XElement("th", "Engine Model"),
                new XElement("th", "Displacement"),
                new XElement("th", "Horse Power")
            ),
            myCars.Select(car =>
                new XElement("tr",
                    new XElement("td", car.model),
                    new XElement("td", car.year),
                    new XElement("td", car.motor.model),
                    new XElement("td", car.motor.displacement),
                    new XElement("td", car.motor.horsePower)
                )
            )
        );

        XElement tableContainer = xhtmlDoc.Descendants().FirstOrDefault(e => e.Attribute("id")?.Value == "tableContainer");
        if (tableContainer != null)
        {
            tableContainer.Add(table);
        }

        xhtmlDoc.Save("CarsFromLinq.xhtml");
    }


    // modyfikacja XML-a
    public static void ModifyXml()
    {
        XElement rootNode = XElement.Load("CarsCollection.xml");

        // zmiana nazwy elementu horsePower na hp
        foreach (XElement car in rootNode.Elements("car"))
        {
            XElement engine = car.Element("engine");
            if (engine != null)
            {
                XElement horsePowerElement = engine.Element("horsePower");
                if (horsePowerElement != null)
                {
                    horsePowerElement.Name = "hp";
                }
            }

            // zamiana elementu year na atrybut w elemencie car
            XElement yearElement = car.Element("year");
            if (yearElement != null)
            {
                XAttribute yearAttribute = new XAttribute("year", yearElement.Value);
                car.Add(yearAttribute);
                yearElement.Remove();
            }
        }

        rootNode.Save("ModifiedCarsCollection.xml");
    }
}
