using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace Lab5
{
    public class Student
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public List<int> Oceny { get; set; }

        public Student()
        {
            Oceny = new List<int>();
        }
    }

    internal class Program
    {
        private const string LinesFile = "lines.txt";
        private const string StudentsJsonFile = "students.json";
        private const string StudentsXmlFile = "students.xml";
        private const string IrisFile = "iris.csv";
        private const string IrisFilteredFile = "iris_filtered.csv";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Wybierz opcję:");
                Console.WriteLine("1 - Zapisz linie do pliku (zastąp)");
                Console.WriteLine("2 - Odczytaj linie z pliku");
                Console.WriteLine("3 - Dopisz linie do istniejącego pliku");
                Console.WriteLine("4 - Utwórz przykładowych studentów i zapisz do JSON");
                Console.WriteLine("5 - Odczytaj studentów z JSON i wypisz");
                Console.WriteLine("6 - Utwórz przykładowych studentów i zapisz do XML");
                Console.WriteLine("7 - Odczytaj studentów z XML i wypisz");
                Console.WriteLine("8 - Odczytaj iris.csv i wypisz wiersze");
                Console.WriteLine("9 - Policz średnie kolumn numerycznych w iris.csv");
                Console.WriteLine("10 - Filtruj iris (sepal length < 5) i zapisz wybrane kolumny");
                Console.WriteLine("0 - Wyjście");
                Console.Write("Wybór: ");
                var key = Console.ReadLine();
                Console.WriteLine();

                switch (key)
                {
                    case "1": AskAndWriteLines(); break;
                    case "2": ReadLinesFromFile(); break;
                    case "3": AppendLinesToFile(); break;
                    case "4": CreateAndSaveStudentsJson(); break;
                    case "5": ReadStudentsFromJson(); break;
                    case "6": CreateAndSaveStudentsXml(); break;
                    case "7": ReadStudentsFromXml(); break;
                    case "8": ReadCsv(); break;
                    case "9": CsvComputeColumnAverages(); break;
                    case "10": FilterCsv(); break;
                    case "0": return;
                    default: Console.WriteLine("Nieznana opcja."); break;
                }
            }
        }

        // Zadanie 2: zapyta użytkownika kilka razy o tekst i zapisze (po jednej informacji w linii)
        public static void AskAndWriteLines()
        {
            Console.Write("Ile linii chcesz wprowadzić? ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
            {
                Console.WriteLine("Niepoprawna liczba.");
                return;
            }

            var lines = new List<string>(n);
            for (int i = 0; i < n; i++)
            {
                Console.Write($"Linia {i + 1}: ");
                lines.Add(Console.ReadLine() ?? string.Empty);
            }

            File.WriteAllLines(LinesFile, lines);
            Console.WriteLine($"Zapisano {n} linii do pliku '{LinesFile}'.");
        }

        // Zadanie 3: odczytaj dane z pliku przygotowanego powyżej i wypisz linia po linii
        public static void ReadLinesFromFile()
        {
            if (!File.Exists(LinesFile))
            {
                Console.WriteLine($"Plik '{LinesFile}' nie istnieje.");
                return;
            }

            var lines = File.ReadAllLines(LinesFile);
            Console.WriteLine($"Zawartość pliku '{LinesFile}':");
            for (int i = 0; i < lines.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {lines[i]}");
            }
        }

        // Zadanie 4: dopisywanie nowych linii do istniejącego pliku
        public static void AppendLinesToFile()
        {
            Console.Write("Ile linii chcesz dopisać? ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
            {
                Console.WriteLine("Niepoprawna liczba.");
                return;
            }

            using var sw = new StreamWriter(LinesFile, append: true);
            for (int i = 0; i < n; i++)
            {
                Console.Write($"Dopisywana linia {i + 1}: ");
                sw.WriteLine(Console.ReadLine() ?? string.Empty);
            }

            Console.WriteLine($"Dopisano {n} linii do pliku '{LinesFile}'.");
        }

        // Zadanie 6: utwórz przykładowych studentów i zapisz listę do JSON
        public static void CreateAndSaveStudentsJson()
        {
            var students = CreateSampleStudents();
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(students, options);
            File.WriteAllText(StudentsJsonFile, json);
            Console.WriteLine($"Zapisano {students.Count} studentów do pliku '{StudentsJsonFile}'.");
        }

        // Zadanie 7: odczytaj plik JSON i wypisz studentów
        public static void ReadStudentsFromJson()
        {
            if (!File.Exists(StudentsJsonFile))
            {
                Console.WriteLine($"Plik '{StudentsJsonFile}' nie istnieje.");
                return;
            }

            var json = File.ReadAllText(StudentsJsonFile);
            var students = JsonSerializer.Deserialize<List<Student>>(json);
            if (students == null)
            {
                Console.WriteLine("Brak danych w pliku JSON.");
                return;
            }

            Console.WriteLine("Studenci z pliku JSON:");
            foreach (var s in students)
            {
                Console.WriteLine($"{s.Imie} {s.Nazwisko} - Oceny: {string.Join(", ", s.Oceny)}");
            }
        }

        // Zadanie 8: utwórz przykładowych studentów i zapisz listę do XML
        public static void CreateAndSaveStudentsXml()
        {
            var students = CreateSampleStudents();
            var serializer = new XmlSerializer(typeof(List<Student>));
            using var fs = new FileStream(StudentsXmlFile, FileMode.Create);
            serializer.Serialize(fs, students);
            Console.WriteLine($"Zapisano {students.Count} studentów do pliku '{StudentsXmlFile}'.");
        }

        // Zadanie 9: odczytaj plik XML i wypisz studentów
        public static void ReadStudentsFromXml()
        {
            if (!File.Exists(StudentsXmlFile))
            {
                Console.WriteLine($"Plik '{StudentsXmlFile}' nie istnieje.");
                return;
            }

            var serializer = new XmlSerializer(typeof(List<Student>));
            using var fs = new FileStream(StudentsXmlFile, FileMode.Open);
            var students = (List<Student>)serializer.Deserialize(fs)!;
            Console.WriteLine("Studenci z pliku XML:");
            foreach (var s in students)
            {
                Console.WriteLine($"{s.Imie} {s.Nazwisko} - Oceny: {string.Join(", ", s.Oceny)}");
            }
        }

        // Zadanie 10: odczytaj iris.csv i wypisz wartości wiersz po wierszu
        public static void ReadCsv()
        {
            if (!File.Exists(IrisFile))
            {
                Console.WriteLine($"Plik '{IrisFile}' nie istnieje. Włóż plik do katalogu aplikacji.");
                return;
            }

            using var sr = new StreamReader(IrisFile);
            string? header = sr.ReadLine();
            if (header == null)
            {
                Console.WriteLine("Plik CSV pusty.");
                return;
            }

            Console.WriteLine("Nagłówek: " + header);
            int row = 0;
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                row++;
                Console.WriteLine($"Wiersz {row}: {line}");
            }
        }

        // Zadanie 11: policz średnią każdej numerycznej kolumny pliku CSV (zakładamy format Iris)
        public static void CsvComputeColumnAverages()
        {
            if (!File.Exists(IrisFile))
            {
                Console.WriteLine($"Plik '{IrisFile}' nie istnieje.");
                return;
            }

            var sums = new double[4];
            var counts = new int[4];
            using var sr = new StreamReader(IrisFile);
            var header = sr.ReadLine();
            if (header == null)
            {
                Console.WriteLine("Plik CSV pusty.");
                return;
            }

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < 5) continue; // oczekujemy 5 kolumn: 4 numeryczne + class

                for (int i = 0; i < 4; i++)
                {
                    if (double.TryParse(parts[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                    {
                        sums[i] += val;
                        counts[i]++;
                    }
                }
            }

            string[] colNames = { "sepal length", "sepal width", "petal length", "petal width" };
            for (int i = 0; i < 4; i++)
            {
                if (counts[i] > 0)
                {
                    Console.WriteLine($"{colNames[i]} - średnia = {sums[i] / counts[i]:F3}");
                }
                else
                {
                    Console.WriteLine($"{colNames[i]} - brak danych do obliczenia średniej");
                }
            }
        }

        // Zadanie 12: wczytaj iris.csv, wybierz kolumny ['sepal length','sepal width','class'], filtruj sepal length < 5 i zapisz do iris_filtered.csv
        public static void FilterCsv()
        {
            if (!File.Exists(IrisFile))
            {
                Console.WriteLine($"Plik '{IrisFile}' nie istnieje.");
                return;
            }

            var outputLines = new List<string>();
            using var sr = new StreamReader(IrisFile);
            var header = sr.ReadLine();
            if (header == null)
            {
                Console.WriteLine("Plik CSV pusty.");
                return;
            }

            // Przyjmujemy, że nagłówek zawiera nazwy w standardowej kolejności
            outputLines.Add("sepal length,sepal width,class");
            int kept = 0;
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < 5) continue;
                if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double sepalLength))
                {
                    if (sepalLength < 5.0)
                    {
                        var newLine = string.Join(',', parts[0], parts[1], parts[4]);
                        outputLines.Add(newLine);
                        kept++;
                    }
                }
            }

            File.WriteAllLines(IrisFilteredFile, outputLines);
            Console.WriteLine($"Zapisano {kept} rekordów do pliku '{IrisFilteredFile}'.");
        }

        // Pomocnicza: tworzy przykładową listę studentów
        private static List<Student> CreateSampleStudents()
        {
            return new List<Student>
            {
                new Student { Imie = "Jan", Nazwisko = "Kowalski", Oceny = new List<int> {5,4,3} },
                new Student { Imie = "Anna", Nazwisko = "Nowak", Oceny = new List<int> {4,4,5} },
                new Student { Imie = "Piotr", Nazwisko = "Zieliński", Oceny = new List<int> {3,2,4} }
            };
        }
    }
}