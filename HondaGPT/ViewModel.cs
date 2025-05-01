using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace HondaGPT;

public partial class ViewModel : ObservableObject
{
    private readonly Chat c = new();
    [ObservableProperty] private ObservableCollection<Documento> _addedModels;

    //observable collection solo per la list box dei modelli/richieste aggiunti
    [ObservableProperty] private ObservableCollection<string> _addedProblems;

    //observable collection per la combo box dei modelli ai
    [ObservableProperty] private ObservableCollection<string> _aiModels;

    //observable collection per la list box dei modelli moto
    [ObservableProperty] private ObservableCollection<string> _bikeModels;
    [ObservableProperty] private string _colore;
    [ObservableProperty] private string _contenutoCronologia;
    [ObservableProperty] private string _cronologiaCancellata;

    [ObservableProperty] private Documento _documentoSelezionato;

    //observable collection per la list box dei manuali
    [ObservableProperty] private ObservableCollection<Documento> _documents;

    [ObservableProperty] private string _domandaDaInviare;
    [ObservableProperty] private string _domandaPreinpostata;
    private int _dotCount;
    [ObservableProperty] private string _fileCaricatoPath;
    [ObservableProperty] private string _input;
    [ObservableProperty] private string _inserisciModelloOutput;
    private bool _isLoading;

    [ObservableProperty] private bool _isPaneOpen;

    [ObservableProperty] private bool _isVisibileCaricamento;

    private Timer? _loadingTimer;
    [ObservableProperty] private string _marcaPersonalizzata;
    [ObservableProperty] private Documento _modelloDaEliminare;

    [ObservableProperty] private string _modelloMotoSelezionato;
    [ObservableProperty] private string _modelloPersonalizzato;
    [ObservableProperty] private string _modelloPersOutput;
    [ObservableProperty] private string _modelloSelezionato;
    [ObservableProperty] private int _numeroTokens;
    [ObservableProperty] private int _opacity0;
    [ObservableProperty] private int _opacity1;
    [ObservableProperty] private int _opacity2;
    [ObservableProperty] private int _opacity3;
    [ObservableProperty] private string _output;

    [ObservableProperty] private string _problemaDaEliminare;

    [ObservableProperty] private string _problemaSelezionato;

    //observable collection per la combo box dei problemi/richieste
    [ObservableProperty] private ObservableCollection<string> _problems;
    [ObservableProperty] private string _richiestaPersonalizzata;
    [ObservableProperty] private string _richiestaPersOutput;
    [ObservableProperty] private string _selezionaFile;
    [ObservableProperty] private bool _txtFileCaricatoVisibile;

    [ObservableProperty] private bool _usaDomandaPreinpostata;
    [ObservableProperty] private double _valoreTemperatura;
    public ViewModel()
    {
        //cerca i modelli e i problemi/richieste salvati nel file e li aggiunge
        c.TrovaModelli();
        c.TrovaProblemi();
        //riempimento della observable collection con i problemi/richieste
        _problems = new ObservableCollection<string>(c.Problemi);
        //riempimento della observable collection con i modelli moto
        _bikeModels = new ObservableCollection<string>(c.ModelliMoto);
        //rimepimento della observable collection con i modelli ai
        _aiModels = new ObservableCollection<string>(c.ModelliAI);
        //il primo modello è quello selezionato di default
        _modelloSelezionato = AiModels[0];
        //legge il contenuto del file di cronologia per visualizzarlo nella text box
        _contenutoCronologia = File.ReadAllText(c.FilePath);
        //numero di token impostati di default
        _numeroTokens = 512;
        //valore temperatura impostato di default
        _valoreTemperatura = 0.7;
        //riempimento della observable collection con i manuali
        _documents = new ObservableCollection<Documento>(c.Documenti);
        //di default il pannello è chiuso
        _isPaneOpen = false;
        //di default il colore del splitview pane è firebrick
        _colore = "Firebrick";
        //textbox dove si vede il file caricato non è visbile di default
        _txtFileCaricatoVisibile = false;
        //opacità dei 4 puntini di caricamento per animazione
        _opacity0 = 0;
        _opacity1 = 0;
        _opacity2 = 0;
        _opacity3 = 0;
        //animazione caricamento non c'è di default
        _isVisibileCaricamento = false;
        //riempimento della observable collection con i modelli/richieste aggiunti
        _addedModels = new ObservableCollection<Documento>(c.ModelliPers);
        _addedProblems = new ObservableCollection<string>(c.ProblemiPers);

        //per l'animazione
        _dotCount = 0;
        _isLoading = false;
    }

    [RelayCommand]
    public async Task ComunicazioneAsync()
    {
        if (_isLoading) return;

        _isLoading = true;
        InizioAnimazione();

        try
        {
            //crea la domanda preinpostata
            DomandaPreinpostata = $"{ProblemaSelezionato} per {ModelloMotoSelezionato}";
            //prompt migliore per l'ai della domanda preimpostata
            var domPre = $"{ProblemaSelezionato} per {ModelloMotoSelezionato}, rispondi sinteticamente in italiano";

            /*scieglie che domanda inviare a seconda ai radio button
            se frasi personalizzate è selezionato allora domandaDaInviare è uguale a input
            se frasi preinpostate è selezionato allora domandaDaInviare è uguale a domandaPreinpostata*/
            _domandaDaInviare = UsaDomandaPreinpostata ? domPre : Input;
            //copia delle domanda da inviare da inserire nel documento
            var domDaInv = UsaDomandaPreinpostata ? DomandaPreinpostata : Input;

            //controllo se la domanda è vuota in modo da non inviare una domanda vuota
            if (!string.IsNullOrEmpty(Input) || !string.IsNullOrEmpty(ProblemaSelezionato) ||
                !string.IsNullOrEmpty(ModelloMotoSelezionato))
            {
                //connessione al server
                var apiUrl = "http://localhost:4891/v1/chat/completions";
                //creazione di un nuovo client per connettersia al server + dopo 5 min di attesa chiude la connessione e si spegne il programma
                var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
                //creazione dell'header che contiene tutte le informazioni (tra cui la domanda) che il client manderà al server
                var requestData = new
                {
                    //modello ai
                    model = ModelloSelezionato,
                    //creazione del messaggio
                    messages = new[]
                    {
                        new { role = "user", content = _domandaDaInviare }
                    },
                    //massimo numero di token(caratteri) che il server può mandare
                    max_tokens = NumeroTokens,
                    /*indicatore creatività e diversità della risposta
                     (0.0 = risposta precisa e diretta, 1.0 = risposta creativa e fantasiosa)
                     maggiore la temperature più tempo ci mette*/
                    temperature = Math.Round(_valoreTemperatura, 2) // Math.Round arrotonda il numero a 2 cifre decimali
                };

                //serializzazione dell'header in formato json per essere mandato
                var jsonRequest = JsonConvert.SerializeObject(requestData);
                //codifica dell'header json per essere mandato
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                //invia dell'header al server
                var response = await client.PostAsync(apiUrl, content);

                //ricezione della risposta dal server
                var jsonResponse = await response.Content.ReadAsStringAsync();

                //trasformazione della risposta del server in un oggetto json
                using var doc = JsonDocument.Parse(jsonResponse);
                //estrazione del messaggio di risposta
                Output = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content")
                    .GetString();
            }
            else
            {
                domDaInv = string.Empty;
                Output = "Nessuna domanda fornita.";
            }

            //salva input e output nel file della cronologia
            c.SalvaInOut(domDaInv, Output);

            //resetta la text box che avvisa che la cronologia è stata cancellata
            CronologiaCancellata = "";

            //aggiorna il contenuto della text box della cronologia
            ContenutoCronologia = File.ReadAllText(c.FilePath);

            //finita la trasmissione della risposta resetta il contenuto delle combo box
            ModelloMotoSelezionato = string.Empty;
            ProblemaSelezionato = string.Empty;

            //resetta il contenuto della text box per le frasi personalizzate
            Input = string.Empty;
        }
        finally
        {
            FineAnimazione();
            _isLoading = false;
        }
    }

    [RelayCommand]
    public void ApriLocalDocs()
    {
        var path = DocumentoSelezionato.Path;
        // Controlla se il file esiste
        if (File.Exists(path))
            // Apre il file con l'applicazione predefinita per i file txt del pc dove viene eseguito il programma
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        else
            Console.WriteLine($"Il file {path} non esiste.");
    }

    [RelayCommand]
    public void OpenPane()
    {
        IsPaneOpen = !IsPaneOpen;

        if (Colore == "Firebrick")
            Colore = "IndianRed";
        else
            Colore = "Firebrick";
    }

    [RelayCommand]
    public void ApriFile()
    {
        try
        {
            FileCaricatoPath = SelezionaFile;
            TxtFileCaricatoVisibile = true;
        }
        catch (Exception ex)
        {
            FileCaricatoPath = $"Errore nella lettura del file: {ex.Message}";
        }
    }

    #region Animazione

    private void InizioAnimazione()
            {
                // Reset opacità
                Opacity0 = 0;
                Opacity1 = 0;
                Opacity2 = 0;
                Opacity3 = 0;
            
                // Reset del contatore
                _dotCount = 0;
            
                // Rende visibile l'animazione
                IsVisibileCaricamento = true;
            
                // Mappa degli aggiornamenti per ogni pallino
                var toggleActions = new Dictionary<int, Action>
                {
                    { 0, () => Opacity0 = Opacity0 == 0 ? 1 : 0 },
                    { 1, () => Opacity1 = Opacity1 == 0 ? 1 : 0 },
                    { 2, () => Opacity2 = Opacity2 == 0 ? 1 : 0 },
                    { 3, () => Opacity3 = Opacity3 == 0 ? 1 : 0 }
                };
            
                _loadingTimer = new Timer(_ =>
                {
                    _dotCount = (_dotCount + 1) % 4;
            
                    // Esegue l'azione corrispondente al pallino corrente
                    if (toggleActions.TryGetValue(_dotCount, out var action)) action.Invoke();
                }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
            }

    private void FineAnimazione()
    {
        // Mappa degli aggiornamenti per resettare le opacità
        var resetActions = new Dictionary<int, Action>
        {
            { 0, () => Opacity0 = 0 },
            { 1, () => Opacity1 = 0 },
            { 2, () => Opacity2 = 0 },
            { 3, () => Opacity3 = 0 }
        };

        foreach (var action in resetActions.Values) action.Invoke();

        // Nasconde l'animazione di caricamento
        IsVisibileCaricamento = false;

        // Disattiva e rimuove il timer
        _loadingTimer?.Dispose();
        _loadingTimer = null;
    }

    #endregion

    #region Cronologia

    //apre il file di cronologia
    [RelayCommand]
    public void VediCronologia()
    {
        var path = c.FilePath;
        // Controlla se il file esiste
        if (File.Exists(path))
            // Apre il file con l'applicazione predefinita per i file txt del pc dove viene eseguito il programma
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        else
            Console.WriteLine($"Il file {path} non esiste.");
    }

    //una volta premuto il bottone cancella cronologia viene una text box che avvisa che la cronologia è stata cancellata
    [RelayCommand]
    public void CancellaCronoliagia()
    {
        var ris = c.CancellaCronologia();
        if (ris)
        {
            //binding della text box che avvisa che la cronologia è stata cancellata
            CronologiaCancellata = "Cronologia cancellata";
            //aggiorna il contenuto della text box della cronologia
            ContenutoCronologia = File.ReadAllText(c.FilePath);
        }
    }

    #endregion

    #region Personalizzazione

    [RelayCommand]
    public void InserisciModello()
    {
        if (!string.IsNullOrEmpty(MarcaPersonalizzata) && !string.IsNullOrEmpty(ModelloPersonalizzato) &&
            !string.IsNullOrEmpty(FileCaricatoPath))
        {
            var modelloIntero = $"{MarcaPersonalizzata} {ModelloPersonalizzato}";
            var d = new Documento { Nome = modelloIntero, Path = FileCaricatoPath };
            Documents.Add(d);
            AddedModels.Add(d);
            c.SalvaInLD(d);
            BikeModels.Add(modelloIntero);
            var path = $@"Resources\Manuali\{modelloIntero}.pdf";
            d.Path = path;
            c.SalvaModello(modelloIntero, path);
            InserisciModelloOutput = "Modello inserito correttamente";
        }
        else
        {
            InserisciModelloOutput = "Inserisci marca o modello o manuale";
        }

        //resetta tutte le text box
        ModelloPersonalizzato = "";
        MarcaPersonalizzata = "";
        TxtFileCaricatoVisibile = false;

        //dopo 5 sec la text box che avvisa che il modello è stato inserito correttamente scompare
        var timer = new System.Timers.Timer(5000);
        timer.Elapsed += (s, e) =>
        {
            InserisciModelloOutput = "";
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    [RelayCommand]
    public void InsersciRichiesta()
    {
        if (!string.IsNullOrEmpty(RichiestaPersonalizzata))
        {
            Problems.Add(RichiestaPersonalizzata);
            AddedProblems.Add(RichiestaPersonalizzata);
            c.SalvaProblema(RichiestaPersonalizzata);
            RichiestaPersOutput = "Richiesta inserita correttamente";
        }
        else
        {
            RichiestaPersOutput = "Inserisci richiesta";
        }

        //resetta tutte le text box
        RichiestaPersonalizzata = "";
        //dopo 5 sec la text box che avvisa che la richiesta è stata inserita correttamente scompare
        var timer = new System.Timers.Timer(5000);
        timer.Elapsed += (s, e) =>
        {
            RichiestaPersOutput = "";
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    [RelayCommand]
    public void EliminaModello()
    {
        if (ModelloDaEliminare == null)
        {

            var nomeDaEliminare = ModelloDaEliminare.Nome?.Trim();
            var pathDaEliminare = ModelloDaEliminare.Path?.Trim();
            var filePathDaEliminare = ModelloDaEliminare.Path;

            // Rimuovi da lista e observable collection
            c.Documenti.RemoveAll(d =>
                d.Nome.Trim() == nomeDaEliminare &&
                d.Path.Trim() == pathDaEliminare);
            Documents.Remove(ModelloDaEliminare);

            c.ModelliPers.RemoveAll(d =>
                d.Nome.Trim() == nomeDaEliminare &&
                d.Path.Trim() == pathDaEliminare);
            AddedModels.Remove(ModelloDaEliminare);

            c.ModelliMoto.Remove(nomeDaEliminare);
            BikeModels.Remove(nomeDaEliminare);

            // Rimuovi dal file
            if (File.Exists(c.ModelliAggiuntiPath))
            {
                var righe = File.ReadAllLines(c.ModelliAggiuntiPath).ToList();

                var righeDaTenere = righe.Where(riga =>
                {
                    var parts = riga.Split(';');
                    if (parts.Length != 2) return true;

                    var nome = parts[0].Trim();
                    var path = parts[1].Trim();

                    // Tiene la riga solo se è diversa
                    return !(nome == nomeDaEliminare && path == pathDaEliminare);
                }).ToList();

                File.WriteAllLines(c.ModelliAggiuntiPath, righeDaTenere);
            }

            // Elimina fisicamente il file
            try
            {
                if (File.Exists(filePathDaEliminare)) File.Delete(filePathDaEliminare);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'eliminazione del file: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Modello da eliminare null");
        }
    }

    [RelayCommand]
    public void EliminaProblema()
    {
        // Normalizza la stringa da eliminare
        var daEliminare = ProblemaDaEliminare?.Trim();

        // Rimuovi da tutte le liste/collection
        c.ProblemiPers.RemoveAll(p => p.Trim() == daEliminare);
        AddedProblems.Remove(daEliminare);
        c.Problemi.RemoveAll(p => p.Trim() == daEliminare);
        Problems.Remove(daEliminare); // se non funziona, controlla string.Trim()

        // Leggi le righe dal file
        var righe = File.ReadAllLines(c.ProblemiAggiuntiPath).ToList();

        // Se c'è solo una riga, svuota il file
        if (righe.Count == 1 && righe[0].Trim() == daEliminare)
        {
            File.WriteAllText(c.ProblemiAggiuntiPath, string.Empty);
        }
        else
        {
            // Rimuovi solo la riga corrispondente
            var righeDaTenere = righe
                .Where(r => r.Trim() != daEliminare)
                .ToList();

            File.WriteAllLines(c.ProblemiAggiuntiPath, righeDaTenere);
        }
    }

    #endregion
}