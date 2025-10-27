public class Zwierze
{
    protected string nazwa;

    public Zwierze(string nazwa)
    {
        this.nazwa = nazwa;
    }
    public virtual void dajGlos()
    {
        Console.WriteLine("...");
    }

}

public class Pies : Zwierze
{
    public Pies(string nazwa) : base(nazwa)
    {
    }

    public override void dajGlos()
    {
        Console.WriteLine($"{nazwa} robi woof woof!");
    }
}

public class Kot : Zwierze
{
    public Kot(string nazwa) : base(nazwa)
    {
    }
    public override void dajGlos()
    {
        Console.WriteLine($"{nazwa} robi miau miau!");
    }
}

public class Waz : Zwierze
{
    public Waz(string nazwa) : base(nazwa)
    { }
    public override void dajGlos()
    {
        Console.WriteLine($"{nazwa} robi ssssssss!");
    }
}

public abstract class Pracownik
{
    public abstract void Pracuj();
}


public class Piekarz : Pracownik
{
    public override void Pracuj()
    {
        Console.WriteLine("Trwa pieczenie...");
    }
}


public class Program
{
    public static void Main()
    {
        Pies azor = new Pies("Azor");
        azor.dajGlos();

        Kot stefan = new Kot("Stefan");
        stefan.dajGlos();
        
        Waz ryszard = new Waz("Ryszard");
        ryszard.dajGlos();

        powiedz_cos(azor);
        Console.WriteLine($"Typ obiektu: {azor.GetType().Name}\n");
        powiedz_cos(stefan);
        Console.WriteLine($"Typ obiektu: {stefan.GetType().Name}\n");
        powiedz_cos(ryszard);
        Console.WriteLine($"Typ obiektu: {ryszard.GetType().Name}\n");

        Piekarz janek = new Piekarz();
        janek.Pracuj();

    }

    public static void powiedz_cos(Zwierze zwierze)
    {
        zwierze.dajGlos();
    }
}