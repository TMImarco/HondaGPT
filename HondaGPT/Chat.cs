using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharp.Pdf;

namespace HondaGPT;

public class Chat
{
    public Chat()
    {
        FilePath = @"Resources\Cronologia.txt";
        ModelliAggiuntiPath = @"Resources\ModelliAggiunti.txt";
        ProblemiAggiuntiPath = @"Resources\ProblemiAggiunti.txt";

        Documenti = new List<Documento>
        {
            new()
            {
                Nome = "Honda CB500FA",
                Path = @"Resources\Manuali\CB500 Hornet - CBR500R - NX500 YM24.pdf"
            },
            new()
            {
                Nome = "Honda CB500RA",
                Path = @"Resources\Manuali\CB500 Hornet - CBR500R - NX500 YM24.pdf"
            },
            new()
            {
                Nome = "Honda XA CB500RA",
                Path = @"Resources\Manuali\CB500 Hornet - CBR500R - NX500 YM24.pdf"
            },
            new()
            {
                Nome = "Honda CB650RA",
                Path = @"Resources\Manuali\CB650R - CBR650R YM24.pdf"
            },
            new()
            {
                Nome = "Honda CBR600R3",
                Path = @"Resources\Manuali\CBR600R YM24.pdf"
            },
            new()
            {
                Nome = "Honda CBR1000SP",
                Path = @"Resources\Manuali\CBR1000RR-R YM24.pdf"
            },
            new()
            {
                Nome = "Honda CBR1000ST",
                Path = @"Resources\Manuali\CBR1000RR-R YM24.pdf"
            },
            new()
            {
                Nome = "Honda AFRICA TWIN",
                Path = @"Resources\Manuali\CRF1100 Africa Twin YM24.pdf"
            },
            new()
            {
                Nome = "Honda EM1e",
                Path = @"Resources\Manuali\EM1 e YM24.pdf"
            },
            new()
            {
                Nome = "Honda FORZA 125 (NSS125AD)",
                Path = @"Resources\Manuali\Forza 125 YM24.pdf"
            },
            new()
            {
                Nome = "Honda FORZA 250 (NSS250A)",
                Path = @"Resources\Manuali\Forza 125 YM24.pdf"
            },
            new()
            {
                Nome = "Honda FORZA 350 (NSS350A)",
                Path = @"Resources\Manuali\Forza 350 YM24.pdf"
            },
            new()
            {
                Nome = "Honda GOlDWING GL1800",
                Path = @"Resources\Manuali\GL1800 Gold Wing YM24.pdf"
            },
            new()
            {
                Nome = "Honda GOLDWING GL1800 BD",
                Path = @"Resources\Manuali\GL1800 Gold Wing YM24.pdf"
            },
            new()
            {
                Nome = "Honda GOLDWING GL1800 DA",
                Path = @"Resources\Manuali\GL1800 Gold Wing YM24.pdf"
            },
            new()
            {
                Nome = "Honda SH 125",
                Path = @"Resources\Manuali\SH 125-150 YM24.pdf"
            },
            new()
            {
                Nome = "Honda SH 150",
                Path = @"Resources\Manuali\SH 125-150 YM24.pdf"
            },
            new()
            {
                Nome = "Honda SH MODE 125",
                Path = @"Resources\Manuali\SH Mode 125 YM24.pdf"
            },
            new()
            {
                Nome = "Honda FSH 125",
                Path = @"Resources\Manuali\SH Mode 125 YM24.pdf"
            }
        };
        ModelliAI = new List<string>
        {
            "Llama 3 8B Instruct",
            "DeepSeek-R1-Distill-Qwen-7B",
            "Nous Hermes 2 Mistral DPO"
        };
        ModelliMoto = new List<string>
        {
            "Honda CB500FA",
            "Honda CB500RA",
            "Honda XA_CB500RA",
            //documento: CB650R - CBR650R YM24.pdf
            "Honda CB650RA",
            "Honda CBR650RA",
            //documento: CBR600R YM24.pdf
            "Honda CBR600R3",
            //documento: CBR1000RR-R YM24.pdf
            "Honda CBR1000SP",
            "Honda CBR1000ST",
            //documento: CRF1100 Africa Twin YM24.pdf
            "Honda AFRICA TWIN",
            //documento: EM1 e YM24.pdf
            "Honda EM1e",
            //documento: Forza 125 YM24.pdf
            "Honda FORZA 125 (NSS125AD)",
            "Honda FORZA 250 (NSS250A)",
            //documento: Forza 350 YM24.pdf
            "Honda FORZA 350 (NSS350A)",
            //documento: GL1800 Gold Wing YM24.pdf
            "Honda GOlDWING GL1800",
            "Honda GOLDWING GL1800 BD",
            "Honda GOLDWING GL1800 DA",
            //documento: SH 125-150 YM24.pdf
            "Honda SH 125",
            "Honda SH 150",
            //documento: SH Mode 125 YM24.pdf
            "Honda SH MODE 125",
            "Honda FSH 125"
        };
        Problemi = new List<string>
        {
            "Cambio olio motore",
            "Olio motore consigliato",
            "Cambio olio freni",
            "Olio freni consigliato",
            "Cambio pastiglie freni",
            "Pastiglie freni consigliate",
            "Cambio pneumatici",
            "Pressione pneumatici consigliata",
            "Cilindrata",
            "Coppia motore",
            "Cavalli motore",
            "Consumo carburante",
            "Patente richiesta",
            "Velocita massima",
            "Consigli di sicurezza",
            "Dispositivi di sicurezza consigliati"
        };
        ProblemiPers = new List<string>();
        ModelliPers = new List<Documento>();
    }

    public string FilePath { get; set; }
    public string ModelliAggiuntiPath { get; set; }
    public string ProblemiAggiuntiPath { get; set; }
    public List<Documento> Documenti { get; set; }
    public List<string> ModelliAI { get; set; }
    public List<string> ModelliMoto { get; set; }

    public List<string> Problemi { get; set; }

    //solo per modelli e problemi aggiunti dall'utente
    public List<string> ProblemiPers { get; set; }
    public List<Documento> ModelliPers { get; set; }

    //salvataggio risposta del server e domanda del client nel file di cronologia
    public void SalvaInOut(string input, string output)
    {
        var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        var contenuto = $"[{timestamp}] \n" +
                        $" Domanda: {input} \n" +
                        $" Risposta: {output} \n" +
                        $" ------------------------------ \n";

        // Legge il contenuto esistente del file
        var contenutoEsistente = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

        // Scrive il nuovo contenuto sopra quello esistente
        File.WriteAllText(FilePath, contenuto + contenutoEsistente);
    }

    public bool CancellaCronologia()
    {
        File.WriteAllText(FilePath, string.Empty);
        return true;
    }

    public void SalvaModello(string marcamodello, string path)
    {
        var contenuto = $"{marcamodello};{path}\n";
        File.AppendAllText(ModelliAggiuntiPath, contenuto);
    }

    public void SalvaProblema(string problema)
    {
        var contenuto = $"{problema}\n";
        File.AppendAllText(ProblemiAggiuntiPath, contenuto);
    }

    public void TrovaModelli()
    {
        var mod = new List<string>();

        mod = File.ReadAllLines(ModelliAggiuntiPath).ToList();

        foreach (var s in mod)
        {
            var arr = new string[2];
            arr = s.Split(';');

            var d = new Documento { Nome = arr[0], Path = arr[1] };
            Documenti.Add(d);

            ModelliPers.Add(d);

            ModelliMoto.Add(arr[0]);
        }
    }

    public void TrovaProblemi()
    {
        var prob = new List<string>();
        prob = File.ReadAllLines(ProblemiAggiuntiPath).ToList();

        foreach (var s in prob)
        {
            Problemi.Add(s);
            ProblemiPers.Add(s);
        }
    }

    //SOLO E UNICAMENTE PDF
    public void SalvaInLD(Documento d)
    {
        //creazione nuovo documento PDF
        var doc = new PdfDocument();
        doc.Info.Title = d.Nome;
        var filename = d.Nome + ".pdf";

        var source = d.Path;
        var dest = @"Resources\Manuali\" + filename;

        //cerca di copiare il file del documento in un file nuvoo nella cartella manuali che ha come nome il nome del modello
        try
        {
            // Copia il file
            File.Copy(source, dest, true);
            Console.WriteLine("File copiato con successo!");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Errore durante la copia del file: {ex.Message}");
        }
    }
}