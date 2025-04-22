using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace lab10_2
{
    public partial class MainWindow : Window
    {
        private static List<Car> myCars = new List<Car>
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

        private static List<Car> myCarsCopy = new List<Car>
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

        SortableBindingList<Car> myCarsBindingList = new SortableBindingList<Car>(myCars);

        public MainWindow()
        {
            InitializeComponent();
            myCarsBindingList = new SortableBindingList<Car>(myCars);

            dataGridView.ItemsSource = myCarsBindingList;

            ICollectionView dataView = CollectionViewSource.GetDefaultView(myCarsBindingList);
            //dataView.SortDescriptions.Add(new SortDescription("motor.horsePower", ListSortDirection.Ascending));

            // zad1
            query_expression();
            method_based_expression();

            // zad2
            args();
        }

        // zad 1.1
        void query_expression()
        {
            var elements = from car in myCars
                           where car.model == "A6"
                           let engineType = car.motor.model.Equals("TDI") ? "diesel" : "petrol"
                           let hppl = car.motor.horsePower / car.motor.displacement
                           group hppl by engineType
                           into carGroups
                           orderby carGroups.Average() descending
                           select new
                           {
                               engineType = carGroups.Key,
                               avgHPPL = carGroups.Average()
                           };

            foreach (var e in elements)
            {
                Debug.WriteLine($"{e.engineType}: {e.avgHPPL}");
            }
        }

        // zad 1.2
        void method_based_expression()
        {
            var elements2 = myCars
               .Where(car => car.model.Equals("A6"))
               .Select(car => new
               {
                   engineType = car.motor.model.Equals("TDI") ? "diesel" : "petrol",
                   hppl = car.motor.horsePower / car.motor.displacement
               })
               .GroupBy(car => car.engineType)
               .Select(group => new
               {
                   engineType = group.Key,
                   avgHPPL = group.Average(c => c.hppl)
               })
               .OrderByDescending(group => group.avgHPPL);

            foreach (var e in elements2) Debug.WriteLine(e.engineType + ": " + e.avgHPPL);
        }


        // zad 2
        void args()
        {
            Func<Car, Car, int> arg1 = (car1, car2) => car2.motor.horsePower.CompareTo(car1.motor.horsePower);
            Predicate<Car> arg2 = car => car.motor.model == "TDI";
            Action<Car> arg3 = car => MessageBox.Show($"Model: {car.model}, Engine Model: {car.motor.model}, Horsepower: {car.motor.horsePower}");

            myCars.Sort(new Comparison<Car>(arg1));
            myCars.FindAll(arg2).ForEach(arg3);
        }



        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            myCarsBindingList.Add(new Car("New Model", new Engine(2.0, 200, "New Engine"), 2024));
        }

        private void RemoveCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                myCarsBindingList.Remove((Car)dataGridView.SelectedItem);
            }
        }

        private void ResetBindingList()
            {
                myCarsBindingList.Clear();
                foreach (var car in myCarsCopy)
                {
                    myCarsBindingList.Add(car);
                }
            }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResetBindingList();
            string searchedText = SearchValueTextBox.Text;
            if (PropertyComboBox.SelectedItem == null) return;
            string searchedField = PropertyComboBox.SelectedItem.ToString();

            List<Car> foundCars = myCars
                .Where(car =>
                {
                    var prop = car.GetType().GetProperty(searchedField);
                    if (prop != null)
                    {
                        var value = prop.GetValue(car)?.ToString();
                        return value != null && value.Contains(searchedText, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                })
                .ToList();

            myCarsBindingList.Clear();
            foreach (var car in foundCars)
            {
                myCarsBindingList.Add(car);
            }

            PropertyComboBox.SelectedIndex = -1;
            SearchValueTextBox.Clear();
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            string sortField = (SortComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            ListSortDirection sortDirection = AscRadioButton.IsChecked == true ? ListSortDirection.Ascending : ListSortDirection.Descending;
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Car)).Find(sortField, true);
            // myCarsBindingList.ApplySortCore(prop, sortDirection);
            // myCarsBindingList.ApplySortCore(new PropertyDescriptorCollection(TypeDescriptor.GetProperties(typeof(Car)).Find(sortField, true)), sortDirection);
        }

        private void PropertyComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PropertyComboBox.Items.Count > 0) return;

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(typeof(Car)))
            {
                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int))
                {
                    PropertyComboBox.Items.Add(prop.Name);
                }
            }
        }

        // obsługa kończenia edycji komórki
        private void dataGridView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            myCarsBindingList.ResetBindings();
        }

        // obsługa zmiany zaznaczenia
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                // obsługa zdarzenia zmiany zaznaczenia
            }
        }


    }


    // MODEL DANYCH
    public class Car
    {
        public string model { get; set; }
        public int year { get; set; }
        public Engine motor { get; set; }

        public Car(string model, Engine motor, int year)
        {
            this.model = model;
            this.motor = motor;
            this.year = year;
        }
    }

    public class Engine : IComparable<Engine>
    {
        public double displacement { get; set; }
        public double horsePower { get; set; }
        public string model { get; set; }

        public Engine(double displacement, double horsePower, string model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }

        public int CompareTo(Engine other)
        {
            return this.horsePower.CompareTo(other.horsePower);
        }
    }

}

