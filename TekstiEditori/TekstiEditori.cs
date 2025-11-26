using System;
using System.IO;
using System.Text;

/// @author lassi
/// @version 01.10.2025
/// <summary>
/// Ohjelman pääluokka. Ohjelma on yksinkertainen tekstieditori jolla voi avata halutun tiedoston
/// komentoriviparametrin pohjalta ja muokata sen tekstisisältöä.
/// </summary>
public class TekstiEditori
{
    /// <summary>
    /// Luodaan luokan attribuutiksi ConsoleKeyInfo jonka avulla saadaan syötettä
    /// otettua talteen. Tehdään tästä attribuutti koska tarvitsemme tätä osassa aliohjelmia
    /// ja halutaan välttää toistoa ja samanaikaisia tietorakenteita.
    /// </summary>
    private static ConsoleKeyInfo _syote;
    /// <summary>
    /// Toimii bufferina jonne kirjoitettu teksti viedään näppäinpainallusten pohjalta.
    /// Bufferin sisältämää tekstiä saatetaan myös prosessoida aliohjelmissa.
    /// </summary>
    private static StringBuilder _bufferi;
    /// <summary>
    /// Bufferin suurin sallittu koko, minkä oletusarvo voidaan yliajaa komentorivi-paramterin avulla.
    /// Mikäli suurin sallittu koko ylittyy, ei enää viedä näppäinpainalluksia bufferriin
    /// ja käyttäjää varoitetaan asiasta.
    /// </summary>
    private static int _bufferinKoko = 10000;
    /// <summary>
    /// Tiedoston nimi (tai polku) joka ldataan ja johon ohjelman lopuksia voidaan kirjoittaa bufferin sisältö.
    /// Komentoriviparametrin avulla muokattavissa.
    /// </summary>
    private static string _tiedostonNimi = "./tiedosto.txt";
    /// <summary>
    /// Ohjelman perusohjeteksti, tulostetaan ruudulle eri yhteksissä. Ajateltu että
    /// readonly-attribuutti sopii tähän paremmin kuin vakio, sillä mikäli ohjelmaa tästä laajentaisi,
    /// voisi hyvinkin tulla kyseeseen alustaa teksti esimerkiksi konfiguraatiotiedoston pohjalta.
    /// </summary>
    private static readonly string _ohjeTeksti = "Kirjoita teksti, paina ESC lopettaksesi:";
    /// <summary>
    /// Teksti joka tulostetaan ruuudlle mikäli bufferi on täynnä. Samat perustelut askan suhteen kuin _ohjeTeksti osalta.
    /// </summary>
    private static readonly string _bufferionTaynnaTeksti = "Bufferi on täynnä!";
    /// <summary>
    /// Kursorin sijainti komentorivillä tekstin sisällä, X-askelilla.
    /// </summary>
    private static int _kursoriX;
    /// <summary>
    /// Kursorin sijainti komentorivillä tekstin sisällä, Y-akselilla.
    /// HUOM: Tämän toteutus on nykyisellään hiukan vaillinnainen, koska kursorin liikuttaminen Y-akselilla
    /// ylös-alas nuolinäppäimillä osoittautui äärimmäisen vaikeaksi toteuttaa .NET-kontekstissa ainakaan
    /// cross-platform -tyylisesti.
    /// </summary>
    private static int _kursoriY;
    
    
    /// <summary>
    /// Ohjelman päämetodi. Sallitut komentoriviparametrit ovat:
    /// tiedostonnimi (string)
    /// bufferinKoko (int)
    /// </summary>
    public static void Main(string[] args)
    {
        Console.TreatControlCAsInput = true; // Estetään ohjelman automaattinen sulku painettaessa Ctrl+C.
        
        if (args.Length != 0)
        {
            _tiedostonNimi = args[0];
        }
        if (args.Length > 1)
        {
            _bufferinKoko = Convert.ToInt32(args[1]);
        }
        
        _bufferi = new StringBuilder("", _bufferinKoko);
       
        if (File.Exists(_tiedostonNimi)) // Mikäli tiedosto on jo olemassa
        {
            string ladattuTeksti = File.ReadAllText(_tiedostonNimi);
            if (ladattuTeksti.Length > _bufferinKoko)
            {
                Console.WriteLine($"Tiedoston {_tiedostonNimi} koko ylittää bufferin maksimikoon ({ladattuTeksti.Length} / {_bufferinKoko})");
                return;
            }
            _bufferi.Append(ladattuTeksti); // Luetaan tiedoston sisältö bufferiin
        }
        ResetoiKonsoli(); // Resetoidaan konsolin sisältö ja näytetään olemassa oleva teksti
        
        do // Ohjelman pääsilmukka, luetaan näppäinpainalluksia pääsääntöisesti kunnes käyttäjä painaa ESC.
        {
            _syote = Console.ReadKey(true); // aletaan ottaa vastan syötettä
            char merkki = MerkinTunnistus(_syote.Key, _syote.KeyChar);
            if (merkki == '\0')
            {
                // Mikäli palautetu merkki on tässä ehtolauseen ehdossa täyttyvä merkki, ei
                // oikeastaan haluta tehdä yhtään mitään - paitsi skipata alla olevat ehtolauseet.
            }
            else if (_bufferi.Length > 0 && _bufferinKoko - 1 < _bufferi.Length)
            {
                // Tämän ehtolauseen ehdon täyttyessä bufferi on täysi eikä haluta sinne enää kirjoittaa
                BufferiTaynna(); // Kerrotaan asiasta myös käyttäjälle.
            }
            else if (_kursoriX < _bufferi.Length)
            {
                // Tämän ehtolauseen ehdon täyttyessä kursori on X-akselilla muualla kuin tekstin lopussa, jolloin
                // halutaan lisätä uusi merkki kursorin osoittamaan paikkaan, eikä vain tekstin loppuun.
                _bufferi.Insert(_kursoriX, merkki);
                
                SiirraKursoria(_kursoriX + 1, _kursoriY); // Siirretään kursoria nyt listäyn merkin verran oikealle.
                ResetoiKonsoli(false);    // Tämä on hiukan raskas ratkaisutapa, mutta keksinyt parempaakaan tekstin päivittämiseksi
                                                    // Muut ratkaisutavat ovat ongelmallisia nyt kun kursoria voi liikuttaa ainakin X-akselilla.
            }
            else
            {
                _bufferi.Insert(_bufferi.Length, merkki); // Lisätään merkki bufferin loppuun, eli tämänhetkisen tekstin loppuun.
                SiirraKursoria(_kursoriX + 1, _kursoriY); // Siirretään kursoria nyt listäyn merkin verran oikealle.
                ResetoiKonsoli(false);
            }
        } while (_syote.Key != ConsoleKey.Escape);
        
        if (KirjoitaBufferiTiedostoon(_tiedostonNimi))
        {
            Console.WriteLine($"Kirjoitettu tuloste tiedostoon {_tiedostonNimi}");
        }
        else
        {
            Console.WriteLine("Tiedostoon ei tehty muutoksia.");
        }
        
        Console.WriteLine($"Bufferia käytettiin: {_bufferi.Length} / {_bufferinKoko}");
        
    }
    

    /// <summary>
    /// Tämä metodi on tarkoitettu tiettyjen erikoismerkkien tunnistamiseen ja läpikäymiseen.
    /// </summary>
    /// <param name="nappain">Syötteestä saatu näppäinpainallus muodossa ConsoleKey </param>
    /// <param name="merkki">Syötteestä saadun näppäinpainalluksen merkki char-muodossa</param>
    /// <returns>Palauttaa halutun merkin</returns>
    /// <example>
    /// <pre name="test">
    /// MerkinTunnistus(ConsoleKey.Enter, '\n') === '\n';
    /// MerkinTunnistus(ConsoleKey.Enter, 's') === '\n';
    /// MerkinTunnistus(ConsoleKey.Escape, 'E') === '\0';
    /// MerkinTunnistus(ConsoleKey.E, 'E') === 'E';
    /// </pre>
    /// </example>
    public static char MerkinTunnistus(ConsoleKey nappain, char merkki)
    {
        switch (nappain)
        {
            case ConsoleKey.Enter: // Halutaan rivinvaihto
                return '\n';
            case ConsoleKey.Backspace: // Poistetaan syötteestä edellinen merkki
                PoistaSyotetteesta(1);
                return '\0'; // Huom: '\0' palautetaan koska jotain jokin merkki on palautettava, ja ko. merkki on tyhjä.
            case ConsoleKey.Escape: // Halutaan sulkea ohjelma
                return '\0';
            case ConsoleKey.LeftArrow: // Siirretään kursoria konsolin sisällä.
                SiirraKursoria(_kursoriX - 1, _kursoriY);
                return '\0';
            case ConsoleKey.RightArrow:
                SiirraKursoria(_kursoriX + 1, _kursoriY);
                return '\0';
            case ConsoleKey.UpArrow:
                SiirraKursoria(0, _kursoriY - 1);
                return '\0';
            case ConsoleKey.DownArrow:
                SiirraKursoria(_kursoriX, _kursoriY + 1);
                return '\0';
            case ConsoleKey.Tab:
                return '\t';
            default:
                return merkki; // Ei ole erikoistapaus, eli palautetaan vain alkuperäinen merkki.
        }
    }

    
    /// <summary>
    /// Tällä voi poistaa merkkejä syötteestä, esimerkiksi kun ollaan käytetty backspacea
    /// syötettä annettaessa.
    /// </summary>
    /// <param name="syvyys">Määrittää kuinka pitkältä/syvältä syötteestä lähdetään merkkiä poistamaan</param>
    /// <param name="tyhjennysOptio">Mikäli tosi, tyhjennetään koko bufferi</param>
    /// <param name="poistaViimeisinMerkki">Mikäli tosi, poistetaan myös viimeinen merkki</param>
    private static void PoistaSyotetteesta(int syvyys, bool tyhjennysOptio = true, bool poistaViimeisinMerkki = false)
    {
        if (_bufferi.Length > syvyys && _kursoriX - syvyys >= 0 && _kursoriX <= _bufferi.Length)
        {
            _bufferi.Remove(_kursoriX - 1, syvyys);
            
            if (poistaViimeisinMerkki) _bufferi.Remove(_bufferi.Length, 1);
            
            SiirraKursoria(_kursoriX -1, _kursoriY);
            ResetoiKonsoli(false);
            
        }
        
        else if ((syvyys == _bufferi.Length || syvyys > _bufferi.Length) && tyhjennysOptio)
        {
            _bufferi.Clear();
            ResetoiKonsoli();
        }
    }

    
    /// <summary>
    /// Tämä metodi resetoi konsolin ja kirjoittaa ohjetekstin ja bufferin senhetkisen sisällän ruudulle.
    /// Resetoi myös kursorin sijainnin.
    /// <param name="resetoiKursori">Resetoidaanko myös kursorin sijainti? Oletuksena kyllä</param>
    /// </summary>
    private static void ResetoiKonsoli(bool resetoiKursori = true)
    {
        Console.Clear();
        Console.WriteLine(_ohjeTeksti);
        Console.Write(_bufferi.ToString());

        if (resetoiKursori)
        {
            SiirraKursoria(_bufferi.Length, 1);
        }
        
    }

    
    /// <summary>
    /// Siirretään kursoria konsolin sisällä.
    /// </summary>
    /// <param name="x">Mihin siirretään kursori vaakasuunnassa</param>
    /// <param name="y">Mihin siirretään kursori pystysuunnassa</param>
    private static void SiirraKursoria(int x, int y)
    {
        if (x >= 0 && x < _bufferi.Length + 1) // Varmistetaan, että kursorin sijainti pysyy kuitenkin tekstin rajoissa.
        {
            Console.CursorLeft = x;
            _kursoriX = x;
        }

        else if (x <= 0 && y > 1) // Mikäli X-akselilla ollaan menossa negativiisen puolelle ja Y-akselilla on mahdollista siirtyä ylöspäin
        {
            Console.CursorTop = y - 1;
            _kursoriY = y - 1;
            ResetoiKonsoli(false);
            return;
        }
        
        if (y > 0)
        {
            Console.CursorTop = y;
            _kursoriY = y;
        }
        
    }

    /// <summary>
    /// Huomauttaa käyttäjälle bufferin täyttymisestä.
    /// </summary>
    private static void BufferiTaynna()
    {
        Console.WriteLine($"{_bufferionTaynnaTeksti}, bufferia käytetty: {_bufferi.Length} / {_bufferinKoko}");
    }

    
    /// <summary>
    /// Sanitoi ja kirjoittaa bufferin sisällön tiedostoon.
    /// </summary>
    /// <param name="tiedostonPolku">Tiedoston polku merkkijonona</param>
    /// <returns>Tosi, mikäli päädyttiin tallentamaan tiedostoon, muuten epätosi.</returns>
    private static bool KirjoitaBufferiTiedostoon(string tiedostonPolku)
    {
        _bufferi.Replace("\0", ""); // Poistetaan nyt nämä ns. tyhjät merkit
        if (_bufferi.Length == 1) // Mikäli bufferin sisältö on vain yksi merkki, emme halua tallentaa rivinvaihtoa tai välilyöntiä.
        {
            _bufferi.Replace("\n", "");
            _bufferi.Replace(" ", "");
        }

        _syote = Console.ReadKey(true);
        Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        Console.WriteLine("Tallennetaanko tiedostoon tehdyt muutokset? Kyllä/Ei (K/E)");
        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        if (KirjoitetaankoTiedostoon(_syote.Key)) { // Kysytään käyttäjltä haluaako hän tallentaa muutokset.
            File.WriteAllText(tiedostonPolku, _bufferi.ToString());
            return true;
        }

        return false;
    }

    
    /// <summary>
    /// Kysyy käyttäjältä, haluaako hän tallentaa tiedostoon tehdyt muutokset.
    /// HUOM: Tämä metodi käyttää rekursiota siinä tapauksessa, että käyttäjä painaa
    /// näppäintä joka ei kelpaa (ei ole K/E), jotta saadaan kysyttyä asiasta uudestaan.
    /// </summary>
    /// <param name="nappain">Painetun näppäimen arvo</param>
    /// <returns>Tosi mikäli käyttäjä haluaa tallentaa, muuten epätosi</returns>
    /// <example>
    /// <pre name="test">
    /// KirjoitetaankoTiedostoon(ConsoleKey.K) === true;
    /// KirjoitetaankoTiedostoon(ConsoleKey.E) === false;
    /// </pre>
    /// </example>
    public static bool KirjoitetaankoTiedostoon(ConsoleKey nappain)
    {
        
        if (nappain == ConsoleKey.Delete) // Jos näppäin on delete, kysytään uudestaan (liittyy rekursion toteutustapaan)
        {
            _syote = Console.ReadKey();
            nappain = _syote.Key;
        }
        
        switch (nappain)
        {
            case ConsoleKey.K:
                return true;
            case ConsoleKey.E:
                break;
            default: // Epävalidi näppäin, kysytään uudestaan rekursion avulla.
                return KirjoitetaankoTiedostoon(ConsoleKey.Delete);
        }
        
        return false;
    }
}
