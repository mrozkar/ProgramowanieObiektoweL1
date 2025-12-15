using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Data.SqlClient;

public class Program
{
    public static void Main()
    {
        string connectionString =
        "Data Source=10.200.2.28;" + //"(LocalDb)\\MSSQLLocalDB;" - dla lokalnej bazy
        "Initial Catalog=studenci_71457;" + //USTAW SWÓJ NUMER!
        "Integrated Security=True;" +
        "Encrypt=True;" +
        "TrustServerCertificate=True";

        try
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Połączono z bazą.");

            while (true)
            {
                ShowMenu();
                string input = Console.ReadLine()?.Trim() ?? "";
                if (input.Equals("q", StringComparison.OrdinalIgnoreCase) || input == "0")
                    break;

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Nieprawidłowy wybór. Wprowadź numer zadania (4-10) lub 0/Q aby zakończyć.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 4:
                            DisplayAllStudents(connection);
                            break;
                        case 5:
                        {
                            int id = ReadInt("Podaj identyfikator studenta:");
                            PrintStudentNameById(connection, id);
                            break;
                        }
                        case 6:
                        {
                            var students = LoadStudentsWithGrades(connection);
                            PrintStudents(students);
                            break;
                        }
                        case 7:
                        {
                            string imie = ReadString("Podaj imię studenta:");
                            string nazwisko = ReadString("Podaj nazwisko studenta:");
                            var newStudent = new Student { Imie = imie, Nazwisko = nazwisko };
                            int newId = AddStudent(connection, newStudent);
                            Console.WriteLine($"Dodano studenta. Nadany identyfikator: {newId}");
                            break;
                        }
                        case 8:
                        {
                            int studentId = ReadInt("Podaj identyfikator studenta (student_id):");
                            string przedmiot = ReadString("Podaj przedmiot:");
                            double wartosc = ReadDouble("Podaj wartość oceny (np. 4.5):");
                            var ocena = new Ocena { StudentId = studentId, Przedmiot = przedmiot, Wartosc = wartosc };
                            try
                            {
                                int ocenaId = AddGrade(connection, ocena);
                                Console.WriteLine($"Dodano ocenę. id: {ocenaId}");
                            }
                            catch (ArgumentException aex)
                            {
                                Console.WriteLine("Błąd walidacji oceny: " + aex.Message);
                            }
                            break;
                        }
                        case 9:
                        {
                            int removed = DeleteGeographyGrades(connection);
                            Console.WriteLine($"Usunięto {removed} ocen z przedmiotu 'Geografia'.");
                            break;
                        }
                        case 10:
                        {
                            int ocenaId = ReadInt("Podaj identyfikator oceny (ocena_id) do aktualizacji:");
                            double newValue = ReadDouble("Podaj nową wartość oceny:");
                            try
                            {
                                bool updated = UpdateGradeValue(connection, ocenaId, newValue);
                                Console.WriteLine(updated ? "Aktualizacja powiodła się." : "Nie znaleziono oceny o podanym id.");
                            }
                            catch (ArgumentException aex)
                            {
                                Console.WriteLine("Błąd walidacji oceny: " + aex.Message);
                            }
                            break;
                        }
                        default:
                            Console.WriteLine("Wybierz zadanie 4-10 lub 0/Q aby zakończyć.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Wystąpił błąd podczas wykonania operacji: " + ex.Message);
                }

                Console.WriteLine();
            }

            Console.WriteLine("Koniec programu.");
        }
        catch (Exception exc)
        {
            Console.WriteLine("Wystąpił błąd: " + exc);
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Wybierz zadanie do wykonania:");
        Console.WriteLine(" 4 - Wyświetl wszystkich studentów (id, imię, nazwisko)");
        Console.WriteLine(" 5 - Wypisz imię i nazwisko studenta po identyfikatorze");
        Console.WriteLine(" 6 - Pobierz i wypisz listę studentów z ocenami");
        Console.WriteLine(" 7 - Dodaj nowego studenta");
        Console.WriteLine(" 8 - Dodaj nową ocenę (walidacja wartości)");
        Console.WriteLine(" 9 - Usuń wszystkie oceny z przedmiotu 'Geografia'");
        Console.WriteLine("10 - Zaktualizuj wartość oceny (walidacja wartości)");
        Console.WriteLine(" 0/Q - Wyjście");
        Console.Write("Twój wybór: ");
    }

    // pomocnicze metody wejścia
    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " ");
            string? s = Console.ReadLine();
            if (int.TryParse(s, out int v))
                return v;
            Console.WriteLine("Nieprawidłowa liczba całkowita, spróbuj ponownie.");
        }
    }

    private static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " ");
            string? s = Console.ReadLine();
            if (s is null) { Console.WriteLine("Brak danych, spróbuj ponownie."); continue; }
            if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out double d) ||
                double.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out d))
                return d;
            Console.WriteLine("Nieprawidłowa liczba zmiennoprzecinkowa, użyj formatu np. 4.5 lub 4,5.");
        }
    }

    private static string ReadString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " ");
            string? s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s))
                return s.Trim();
            Console.WriteLine("Wartość nie może być pusta, spróbuj ponownie.");
        }
    }

    // 4. Wyświetla wszystkie wiersze z tabeli student (id, imię, nazwisko)
    public static void DisplayAllStudents(SqlConnection connection)
    {
        using var cmd = new SqlCommand("SELECT student_id, imie, nazwisko FROM student ORDER BY student_id", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string imie = reader.GetString(1);
            string nazwisko = reader.GetString(2);
            Console.WriteLine($"{id}: {imie} {nazwisko}");
        }
    }

    // 5. Wypisuje imię i nazwisko studenta o podanym identyfikatorze
    public static void PrintStudentNameById(SqlConnection connection, int studentId)
    {
        using var cmd = new SqlCommand("SELECT imie, nazwisko FROM student WHERE student_id = @id", connection);
        cmd.Parameters.AddWithValue("@id", studentId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            string imie = reader.GetString(0);
            string nazwisko = reader.GetString(1);
            Console.WriteLine($"Student {studentId}: {imie} {nazwisko}");
        }
        else
        {
            Console.WriteLine($"Student o id {studentId} nie znaleziony.");
        }
    }

    // 6. Zwraca listę studentów z uwzględnieniem ich ocen (mapowanie z bazy)
    public static List<Student> LoadStudentsWithGrades(SqlConnection connection)
    {
        var students = new Dictionary<int, Student>();

        using (var cmd = new SqlCommand("SELECT student_id, imie, nazwisko FROM student", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                var s = new Student
                {
                    StudentId = id,
                    Imie = reader.GetString(1),
                    Nazwisko = reader.GetString(2)
                };
                students[id] = s;
            }
        }

        using (var cmd = new SqlCommand("SELECT ocena_id, wartosc, przedmiot, student_id FROM ocena", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var o = new Ocena
                {
                    OcenaId = reader.GetInt32(0),
                    Wartosc = reader.GetDouble(1),
                    Przedmiot = reader.GetString(2),
                    StudentId = reader.GetInt32(3)
                };

                if (students.TryGetValue(o.StudentId, out var st))
                {
                    st.Oceny.Add(o);
                }
            }
        }

        return students.Values.OrderBy(s => s.StudentId).ToList();
    }

    // Pomocnicza metoda do wypisywania listy studentów z ocenami (użyte w zad.6)
    public static void PrintStudents(List<Student> students)
    {
        foreach (var s in students)
        {
            Console.WriteLine($"{s.StudentId}: {s.Imie} {s.Nazwisko}");
            if (s.Oceny == null || s.Oceny.Count == 0)
            {
                Console.WriteLine("\tBrak ocen.");
            }
            else
            {
                foreach (var o in s.Oceny)
                {
                    Console.WriteLine($"\t{o.Przedmiot}: {o.Wartosc:F1}");
                }
                Console.WriteLine($"\tŚrednia: {s.Oceny.Average(o => o.Wartosc):F2}");
            }
        }
    }

    // 7. Dodaje nowego studenta do bazy i ustawia StudentId w obiekcie; zwraca nowy identyfikator
    public static int AddStudent(SqlConnection connection, Student student)
    {
        using var cmd = new SqlCommand("INSERT INTO student (imie, nazwisko) VALUES (@imie, @nazwisko); SELECT CAST(SCOPE_IDENTITY() AS int);", connection);
        cmd.Parameters.AddWithValue("@imie", student.Imie);
        cmd.Parameters.AddWithValue("@nazwisko", student.Nazwisko);
        var result = cmd.ExecuteScalar();
        int newId = Convert.ToInt32(result);
        student.StudentId = newId;
        return newId;
    }

    // Walidacja oceny zgodnie z zadaniem 8 (2..5, tylko .0 lub .5, brak 2.5)
    private static bool IsValidGrade(double value)
    {
        if (!(value >= 2.0 && value <= 5.0))
            return false;
        if (Math.Abs(value - 2.5) < 1e-9) // zabronione 2.5
            return false;
        double twice = value * 2.0;
        if (Math.Abs(twice - Math.Round(twice)) > 1e-9)
            return false; // nie jest wielokrotnością 0.5
        return true;
    }

    // 8. Dodaje nową ocenę do bazy (z walidacją). Zwraca id nowej oceny.
    public static int AddGrade(SqlConnection connection, Ocena ocena)
    {
        if (!IsValidGrade(ocena.Wartosc))
            throw new ArgumentException("Nieprawidłowa wartość oceny. Dozwolone: 2.0,3.0,3.5,4.0,4.5,5.0 (bez 2.5).");

        using var cmd = new SqlCommand("INSERT INTO ocena (wartosc, przedmiot, student_id) VALUES (@wartosc, @przedmiot, @studentId); SELECT CAST(SCOPE_IDENTITY() AS int);", connection);
        cmd.Parameters.AddWithValue("@wartosc", ocena.Wartosc);
        cmd.Parameters.AddWithValue("@przedmiot", ocena.Przedmiot);
        cmd.Parameters.AddWithValue("@studentId", ocena.StudentId);
        var result = cmd.ExecuteScalar();
        int newId = Convert.ToInt32(result);
        ocena.OcenaId = newId;
        return newId;
    }

    // 9. Usuwa wszystkie oceny z przedmiotu "Geografia"
    public static int DeleteGeographyGrades(SqlConnection connection)
    {
        using var cmd = new SqlCommand("DELETE FROM ocena WHERE LOWER(przedmiot) = 'geografia'", connection);
        int affected = cmd.ExecuteNonQuery();
        return affected; // ilość usuniętych wierszy
    }

    // 10. Aktualizuje wartość oceny o danym identyfikatorze po walidacji
    public static bool UpdateGradeValue(SqlConnection connection, int ocenaId, double newValue)
    {
        if (!IsValidGrade(newValue))
            throw new ArgumentException("Nieprawidłowa wartość oceny. Dozwolone: 2.0,3.0,3.5,4.0,4.5,5.0 (bez 2.5).");

        using var cmd = new SqlCommand("UPDATE ocena SET wartosc = @wartosc WHERE ocena_id = @id", connection);
        cmd.Parameters.AddWithValue("@wartosc", newValue);
        cmd.Parameters.AddWithValue("@id", ocenaId);
        int rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
}
