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
    private static readonly int bufferinKoko = 50;
    private static readonly string tiedostonNimi = "./tiedosto.txt";
    private static readonly string ohjeTeksti = "Kirjoita teksi, paina DEL lopettaksesi:";
    private static readonly string bufferionTaynnaTeksti = "Bufferi on täynnä!";
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
            syote = Console.ReadKey(true);
            char merkki = MerkinTunnistus(syote.Key, syote.KeyChar);
            if(bufferi.Length > 0 && bufferinKoko - 1 < bufferi.Length)
            {
                BufferiTaynna();
            }
            else
            {
                bufferi.Append(merkki);
                Console.Write(merkki);
            }
        } while (syote.Key != ConsoleKey.Delete);
        Console.WriteLine("\n" + bufferi);
        Console.WriteLine($"Bufferia käytetty: {bufferi.Length} / {bufferinKoko}");
        KirjoitaBufferiTiedostoon(tiedostonNimi, bufferi);
        Console.WriteLine($"Kirjoitettu tuloste tiedostoon {tiedostonNimi}");
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
    /// <param name="poistaViimeisinMerkki"></param>
    public static void PoistaSyotetteesta(int syvyys, bool poistaViimeisinMerkki = false)
    {
        if (bufferi.Length - 1 > syvyys)
        {
            bufferi.Remove(bufferi.Length - syvyys, syvyys);
            if (poistaViimeisinMerkki) bufferi.Remove(bufferi.Length, 1);
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
        Console.Write(bufferi);
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
    public static void KirjoitaBufferiTiedostoon(string tiedostonPolku, StringBuilder teksti)
    {
        bufferi.Replace("\0", "");
        File.WriteAllText(tiedostonPolku, teksti.ToString());
    }
}