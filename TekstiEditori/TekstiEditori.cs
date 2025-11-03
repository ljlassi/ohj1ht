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
    private static int bufferinKoko = 10000;
    private static string tiedostonNimi = "./tiedosto.txt";
    private static readonly string ohjeTeksti = "Kirjoita teksti, paina ESC lopettaksesi:";
    private static readonly string bufferionTaynnaTeksti = "Bufferi on täynnä!";
    /// <summary>
    /// Ohjelman päämetodi
    /// </summary>
    public static void Main(string[] args)
    {
        Console.TreatControlCAsInput = true;
        if (args.Length != 0)
        {
            tiedostonNimi = args[0];
        }
        if (args.Length > 1)
        {
            bufferinKoko = Convert.ToInt32(args[1]);
        }
        bufferi = new StringBuilder("", bufferinKoko);
        if (File.Exists(tiedostonNimi))
        {
            bufferi.Append(File.ReadAllText(tiedostonNimi));
        }
        ResetoiKonsoli();
        do
        {
            syote = Console.ReadKey(true);
            char merkki = MerkinTunnistus(syote.Key, syote.KeyChar);
            if (merkki == '\0')
            {
                
            }
            else if(bufferi.Length > 0 && bufferinKoko - 1 < bufferi.Length)
            {
                BufferiTaynna();
            }
            else
            {
                bufferi.Append(merkki);
                Console.Write(merkki);
            }
        } while (syote.Key != ConsoleKey.Escape);

        Console.Clear();
        Console.WriteLine("\n" + bufferi);
        if (KirjoitaBufferiTiedostoon(tiedostonNimi))
        {
            Console.WriteLine($"Kirjoitettu tuloste tiedostoon {tiedostonNimi}");
        }
        Console.WriteLine($"Bufferia käytetty: {bufferi.Length} / {bufferinKoko}");
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
                PoistaSyotetteesta(1);
                return '\0';
            case ConsoleKey.Escape:
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
    /// <param name="poistaViimeisinMerkki"></param>
    public static void PoistaSyotetteesta(int syvyys, bool tyhjennysOptio = true, bool poistaViimeisinMerkki = false)
    {
        if (bufferi.Length > syvyys)
        {
            bufferi.Remove(bufferi.Length - syvyys, syvyys);
            if (poistaViimeisinMerkki) bufferi.Remove(bufferi.Length, 1);
        }
        else if((syvyys == bufferi.Length || syvyys > bufferi.Length) && tyhjennysOptio)
        {
            bufferi.Clear();
        }
        ResetoiKonsoli();
    }

    /// <summary>
    /// Tämä metodi resetoi konsolin ja kirjoittaa ohjetekstin ja bufferin senhetkisen sisällän ruudulle.
    /// </summary>
    public static void ResetoiKonsoli()
    {
        Console.Clear();
        Console.WriteLine(ohjeTeksti);
        Console.Write(bufferi.ToString());
    }

    /// <summary>
    /// Huomauttaa käyttäjälle bufferin täyttymisestä.
    /// </summary>
    public static void BufferiTaynna()
    {
        Console.WriteLine($"{bufferionTaynnaTeksti}, bufferia käytetty: {bufferi.Length} / {bufferinKoko}");
    }

    /// <summary>
    /// Sanitoi ja kirjoittaa bufferin sisällön tiedostoon.
    /// </summary>
    /// <param name="tiedostonPolku"></param>
    /// <param name="teksti"></param>
    public static bool KirjoitaBufferiTiedostoon(string tiedostonPolku)
    {
        bufferi.Replace("\0", "");
        if (bufferi.Length == 1)
        {
            bufferi.Replace("\n", "");
            bufferi.Replace(" ", "");
        }
        if (bufferi.Length > 0)
        {
            File.WriteAllText(tiedostonPolku, bufferi.ToString());
            return true;
        }

        return false;
    }
}