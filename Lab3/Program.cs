using System;

public class ComplexNumber : ICloneable, IEquatable<ComplexNumber>
{

    private double re;  
    private double im; 


    public double Re
    {
        get { return re; }
        set { re = value; }
    }

    public double Im
    {
        get { return im; }
        set { im = value; }
    }


    public ComplexNumber(double re, double im)
    {
        this.re = re;
        this.im = im;
    }


    public override string ToString()
    {
        string sign = im >= 0 ? " + " : " - ";
        return $"{re}{sign}{Math.Abs(im)}i";
    }


    public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.re + b.re, a.im + b.im);
    }


    public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.re - b.re, a.im - b.im);
    }


    public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
    {
        double real = a.re * b.re - a.im * b.im;
        double imag = a.re * b.im + a.im * b.re;
        return new ComplexNumber(real, imag);
    }


    public static ComplexNumber operator -(ComplexNumber a)
    {
        return new ComplexNumber(a.re, -a.im);
    }


    public object Clone()
    {
        return new ComplexNumber(this.re, this.im);
    }


    public bool Equals(ComplexNumber other)
    {
        if (other == null)
            return false;

        return this.re == other.re && this.im == other.im;
    }


    public override bool Equals(object obj)
    {
        if (obj is ComplexNumber c)
            return Equals(c);
        return false;
    }


    public override int GetHashCode()
    {
        return re.GetHashCode() ^ im.GetHashCode();
    }


    public static bool operator ==(ComplexNumber a, ComplexNumber b)
    {
        if (ReferenceEquals(a, b)) return true;
        if ((object)a == null || (object)b == null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(ComplexNumber a, ComplexNumber b)
    {
        return !(a == b);
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        ComplexNumber z1 = new ComplexNumber(6, 7);
        ComplexNumber z2 = new ComplexNumber(1, -2);

        Console.WriteLine($"z1 = {z1}");
        Console.WriteLine($"z2 = {z2}");
        Console.WriteLine($"Suma: {z1 + z2}");
        Console.WriteLine($"Różnica: {z1 - z2}");
        Console.WriteLine($"Iloczyn: {z1 * z2}");
        Console.WriteLine($"Sprzężenie z1: {-z1}");
        Console.WriteLine($"Kopia z1: {(ComplexNumber)z1.Clone()}");
        Console.WriteLine($"Czy z1 == z2? {(z1 == z2)}");
    }
}
