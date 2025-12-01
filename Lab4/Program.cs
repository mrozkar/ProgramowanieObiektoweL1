using System;
using System.Collections.Generic;
using System.Linq;

public interface IModular
{
    double Module();
}

public class ComplexNumber : ICloneable, IEquatable<ComplexNumber>, IModular, IComparable<ComplexNumber>, IComparable
{
    private double re;
    private double im;
    public double Re { get => re; set => re = value; }
    public double Im { get => im; set => im = value; }

    public ComplexNumber(double re, double im)
    {
        this.re = re; this.im = im;
    }

    public override string ToString()
    {
        string sign = im >= 0 ? "+" : "-";
        return $"{re} {sign} {Math.Abs(im)}i (|z|={Module():F3})";
    }

    public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
        => new ComplexNumber(a.re + b.re, a.im + b.im);

    public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
        => new ComplexNumber(a.re - b.re, a.im - b.im);

    public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
        => new ComplexNumber(a.re * b.re - a.im * b.im, a.re * b.im + a.im * b.re);

    public static ComplexNumber operator -(ComplexNumber a)
        => new ComplexNumber(a.re, -a.im);

    public object Clone() => new ComplexNumber(re, im);

    public bool Equals(ComplexNumber other)
    {
        if (other == null) return false;
        return re == other.re && im == other.im;
    }

    public override bool Equals(object obj)
        => obj is ComplexNumber other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(re, im);

    public static bool operator ==(ComplexNumber a, ComplexNumber b)
        => a?.Equals(b) ?? b is null;

    public static bool operator !=(ComplexNumber a, ComplexNumber b)
        => !(a == b);

    // Implementacja metody z IModular
    public double Module()
        => Math.Sqrt(re * re + im * im);

    // IComparable<ComplexNumber> - porównujemy po module
    public int CompareTo(ComplexNumber? other)
    {
        if (other is null) return 1;
        return Module().CompareTo(other.Module());
    }

    // IComparable
    int IComparable.CompareTo(object? obj)
    {
        if (obj is ComplexNumber other) return CompareTo(other);
        throw new ArgumentException("Object is not a ComplexNumber");
    }

    // Opcjonalne operatory porównawcze ułatwiające czytelność
    public static bool operator <(ComplexNumber a, ComplexNumber b) => a.CompareTo(b) < 0;
    public static bool operator >(ComplexNumber a, ComplexNumber b) => a.CompareTo(b) > 0;
    public static bool operator <=(ComplexNumber a, ComplexNumber b) => a.CompareTo(b) <= 0;
    public static bool operator >=(ComplexNumber a, ComplexNumber b) => a.CompareTo(b) >= 0;
}

class Program
{
    static void PrintTitle(string title)
    {
        Console.WriteLine();
        Console.WriteLine("=== " + title + " ===");
    }

    static void Print<T>(IEnumerable<T> items)
    {
        foreach (var it in items) Console.WriteLine(it);
    }

    static void Main()
    {
        // 2. Tablica pięciu liczb zespolonych
        ComplexNumber[] arr = new[]
        {
            new ComplexNumber(3, 4),    // |z| = 5
            new ComplexNumber(-1, -1),  // |z| = sqrt(2)
            new ComplexNumber(0, 2),    // |z| = 2
            new ComplexNumber(5, 0),    // |z| = 5
            new ComplexNumber(-2, 3)    // |z| = sqrt(13)
        };

        PrintTitle("2a. Tablica - foreach");
        Print(arr);

        // 2b. Posortuj w oparciu o moduł i wypisz
        Array.Sort(arr); // używa IComparable -> sortuje po module
        PrintTitle("2b. Tablica - posortowana po module (rosnaco)");
        Print(arr);

        // 2c. Minimum i maksimum tablicy
        var minArr = arr.Min();
        var maxArr = arr.Max();
        PrintTitle("2c. Tablica - minimum i maksimum (po module)");
        Console.WriteLine("Min: " + minArr);
        Console.WriteLine("Max: " + maxArr);

        // 2d. Odfiltruj liczby o ujemnej części urojonej
        var negativeImag = arr.Where(z => z.Im < 0).ToArray();
        PrintTitle("2d. Tablica - ujemna czesc urojona");
        Print(negativeImag);

        // 3. Lista liczb zespolonych
        var list = new List<ComplexNumber>
        {
            new ComplexNumber(2, 1),
            new ComplexNumber(-3, -4),
            new ComplexNumber(0, -2),
            new ComplexNumber(1, 1),
            new ComplexNumber(4, -1)
        };

        PrintTitle("3. Lista - poczatkowa");
        Print(list);

        // Sprawdź te same operacje (sortowanie, min, max, filter)
        list.Sort(); // sortuje po module
        PrintTitle("3. Lista - posortowana po module");
        Print(list);

        Console.WriteLine("Min: " + list.Min());
        Console.WriteLine("Max: " + list.Max());

        var listNeg = list.Where(z => z.Im < 0);
        PrintTitle("3. Lista - ujemna czesc urojona");
        Print(listNeg);

        // 3a. Usuń drugi element z listy i wypisz
        if (list.Count >= 2) list.RemoveAt(1);
        PrintTitle("3a. Lista - po usunieciu drugiego elementu");
        Print(list);

        // 3b. Usuń najmniejszy element z listy i wypisz
        var smallest = list.Min();
        list.Remove(smallest);
        PrintTitle("3b. Lista - po usunieciu najmniejszego elementu");
        Print(list);

        // 3c. Usuń wszystkie elementy z listy i wypisz
        list.Clear();
        PrintTitle("3c. Lista - po wyczyszczeniu");
        Print(list); // nic nie wypisze

        // 4. HashSet
        var z1 = new ComplexNumber(6, 7);
        var z2 = new ComplexNumber(1, 2);
        var z3 = new ComplexNumber(6, 7);
        var z4 = new ComplexNumber(1, -2);
        var z5 = new ComplexNumber(-5, 9);

        var set = new HashSet<ComplexNumber> { z1, z2, z3, z4, z5 };

        PrintTitle("4a. HashSet - zawartosc (duplikaty powinny byc odfiltrowane)");
        Print(set);

        // 4b. Operacje min/max/sort/filter na zbiorze (konwersja do IEnumerable)
        PrintTitle("4b. HashSet - min, max, posortowane, filtrowane");
        if (set.Any())
        {
            Console.WriteLine("Min: " + set.Min());
            Console.WriteLine("Max: " + set.Max());
        }
        Console.WriteLine("Posortowane:");
        Print(set.OrderBy(z => z));
        Console.WriteLine("Ujemna czesc urojona:");
        Print(set.Where(z => z.Im < 0));

        // 5. Slownik
        var dict = new Dictionary<string, ComplexNumber>
        {
            ["z1"] = z1,
            ["z2"] = z2,
            ["z3"] = z3,
            ["z4"] = z4,
            ["z5"] = z5
        };

        PrintTitle("5a. Slownik - wszystkie elementy (klucz, wartosc)");
        foreach (var kv in dict) Console.WriteLine($"({kv.Key}, {kv.Value})");

        PrintTitle("5b. Slownik - klucze");
        foreach (var k in dict.Keys) Console.WriteLine(k);

        PrintTitle("5b. Slownik - wartosci");
        foreach (var v in dict.Values) Console.WriteLine(v);

        PrintTitle("5c. Sprawdzenie czy istnieje klucz 'z6'");
        Console.WriteLine("z6 istnieje? " + dict.ContainsKey("z6"));

        // 5d. Wykonaj na słowniku zadania 2c i 2d na wartościach
        if (dict.Values.Any())
        {
            Console.WriteLine("5d. Slownik - Min: " + dict.Values.Min());
            Console.WriteLine("5d. Slownik - Max: " + dict.Values.Max());
        }
        Console.WriteLine("5d. Slownik - wartosci z ujemna czescia urojona:");
        Print(dict.Values.Where(z => z.Im < 0));

        // 5e. Usun element o kluczu "z3"
        dict.Remove("z3");
        PrintTitle("5e. Slownik - po usunieciu klucza 'z3'");
        foreach (var kv in dict) Console.WriteLine($"({kv.Key}, {kv.Value})");

        // 5f. Usun drugi element ze slownika (wg porzadku enumeracji)
        if (dict.Count >= 2)
        {
            var secondKey = dict.Keys.ElementAt(1);
            dict.Remove(secondKey);
            PrintTitle($"5f. Slownik - po usunieciu drugiego elementu (klucz '{secondKey}')");
            foreach (var kv in dict) Console.WriteLine($"({kv.Key}, {kv.Value})");
        }

        // 5g. Wyczyść słownik
        dict.Clear();
        PrintTitle("5g. Slownik - po wyczyszczeniu");
        Console.WriteLine("Count = " + dict.Count);

        Console.WriteLine();
        Console.WriteLine("Koniec programu.");
    }
}