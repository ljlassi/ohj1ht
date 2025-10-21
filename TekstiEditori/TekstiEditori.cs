using System;
using System.IO;
using System.Text;

/// @author lassi
/// @version 01.10.2025
/// <summary>
/// Ohjelman pääluokka
/// </summary>
public class TekstiEditori
{
    private static ConsoleKeyInfo syote;
    private static StringBuilder bufferi;
    private static readonly int bufferinKoko = 5000;
    private static readonly string fileName = "./tiedosto.txt";
    /// <summary>
    /// Ohjelman päämetodi
    /// </summary>
    public static void Main()
    {
        Console.TreatControlCAsInput = true;
        Console.WriteLine("Kirjoita teksi, paina DEL lopettaksesi:");
        bufferi = new StringBuilder("", bufferinKoko);
        do
        {
            syote = Console.ReadKey(false);
            bufferi.Append(MerkinTunnistus(syote.Key, syote.KeyChar));
        } while (syote.Key != ConsoleKey.Delete);
        Console.WriteLine("\n" + bufferi);
        Console.WriteLine($"Bufferia käytetty: {bufferi.Length} / {bufferinKoko}");
        File.WriteAllText(fileName, bufferi.ToString());
        Console.WriteLine($"Kirjoitettu tuloste tiedostoon {fileName}");
    }

    /// <summary>
    /// Tarkoitettu tiettyjen erikoismerkkien läpikäymiseen.
    /// </summary>
    /// <param name="nappain"></param>
    /// <param name="merkki"></param>
    /// <returns></returns>
    public static char MerkinTunnistus(ConsoleKey nappain, char merkki)
    {
        switch (nappain)
        {
            case ConsoleKey.Enter:
                return '\n';
            case ConsoleKey.Backspace:
                PoistaSyotetteesta(2);
                return '\0';
            default:
                return merkki;
        }
    }

    /// <summary>
    /// Tällä voi poistaa merkkejä syötteestä, esimerkiksi kun ollaan käytetty backspacea
    /// syötettä annettaessa.
    /// </summary>
    /// <param name="syvyys"></param>
    public static void PoistaSyotetteesta(int syvyys)
    {
        bufferi.Remove(bufferi.Length - syvyys, syvyys);
    }
}