# Prova esperta LLM Chat specializzata

## Overview
Progetto per la prova esperta del quarto anno del corso di informatica del ITT Eugenio Barsanti (As 2024-2025)
Creazione di un programma per un'AI specializzata (che gira in locale).

Framework: .NET 9.0 e Avalonia UI 11.2.8  
Pacchetti usati: Community ToolKit.Mvvm 8.4.0, Newtonsoft.Json 13.0.3, PDFsharp 6.1.1  
Porta server api: 4891
Servizio per AI locale: gpt4all  
Funzioni:

- [x] specializzazione su alcune moto honda (app rivolta ai meccanici)  
- [x] cronologia chat (documento)  
- [x] cancella cronologia chat (documento)  
- [x] cronologia chat (conversazione \- textbox per visualizzare il file)  
- [x] frasi già fatte (selezionare termini \- combo box)  
- [x] frasi personalizzate(scrivi tu)  
- [x] \[problema\] possibilità di aprire e consultare i documenti inseriti in local docs (list box per selezionare quali documenti \[oggetti documento gia creati {23/4}\])  
- [x] \[problema\] sistemare path documenti per list box (23/4)  
- [x] ~~applicazione monolitica avviabile senza il bisogno di nessuna IDE(rider)~~  
- [x] scelta di più modelli IA (combo box)  
- [x] personalizzazione temperatura (slider)  
- [x] personalizzazione tokens (numeric up down)  
- [x] selezionare frasi fatte o personalizzate tramite check box (un unico pulsante invia)  
- [x] ~~\[problema\] sistemare domanda fantasma delle frasi fatte (22/4)~~  
- [x] sistemare domanda fatta input utente non mostrati(22/4)  
- [x] ~~\[problema\] sistemare che la text box con la cronologia della conversazione parte sempre dall’alto e scoccia scrollare giù ogni volta (22/4)~~  
- [x] creazione file che va al contrario (la conversazione più recente viene messa il altro)  
- [x] \[problema\] fare in modo che una volta mandata la domanda la text box della frase personalizzata e le combo box delle frasi gia fatte si svuotino (23/4)  
- [x] mettere l’apertura dei documenti local docs nelle impostazioni prima delle impostazioni avanzate e per scorrere tutti usare un content scroller  
- [x] ~~grafica ([Tutorial](https://www.youtube.com/@AngelSix/videos) \- canvas, dock panel)~~  
- [x] \[grafica\] la text box con la conversazione va ~~sotto~~ sopra mentre le cose per fare le domande sopra  
- [x] creare opzioni per personalizzare l’applicazione (inserisci nuovo modello: marca, modello, manuale \[path e pdf\], problema/richiesta)  
- [x] salvare le cose personalizzate nell’applicazione per sempre  
- [x] trovare un modo per mettere il manuale personalizzato nei local docs di gpt4all  
- [x] ~~fare una cartella unica tra local docs di gpt4all e manuali dentro al programma(usando la tecnica degli assets come con le immagini)~~ gpt4all userà quella cartella come localdocs (perché c’è scritto che i local docs si modificano ogni volta che la cartella subisce cambiamenti) cartella per i local docs HondaGPT/HondaGPT/bin/DEbug/net9.0/Resources/Manuali (in questa cartella verranno inseriti anche i manuali inseriti dall’utente)  
- [x] sistemare il modo con il quale la domanda viene posta nella chat del server e mostrarla nella cronologia  
- [x] ~~vedere se riesci a fare che quando selezioni una combo box o scrivi qualcosa sulla textbox il radio button si cambia automaticamente~~  
- [x] ~~\[problema\] fare qualcosa per eliminare le cose personalizzate~~  
- [x] \[grafica\] sistemare la personalizzazione (dopo che scrivi una roba si deve cancellare in qualche modo) \+ sistemare colori  
- [x] \[grafica\] scrivere —inserisci richiesta— e —inserisci modello— nelle combo box delle domande (come cosa default)  
- [x] \[grafica\] logo honda come titolo  
- [x] \[grafica\] cambiamento modello, temperatura, tokens sotto l’opzione impostazioni avanzate mettere icona impostazioni [icone](https://avaloniaui.github.io/icons.html) con expander (cronologia su setting normali)  
- [x] \[grafica\] split view per impostazioni (modello, cronologia, temperatura, documenti)  
- [x] ~~\[grafica\] cambiare colore freccette numericUpDown~~  
- [x] ~~\[grafica\] cambiare colore freccette combo box~~  
- [x] ~~\[grafica\] cambiare colore agli item interni alle combo box~~  
- [x] ~~\[grafica\] cambiare colore ai cerchietti dei radio button~~  
- [x] ~~\[grafica\] mettere i bordi agli item nella list box~~  
- [x] ~~\[grafica\] mettere icona honda sull’icona del processo~~  
- [x] ~~\[grafica\] provare a mettere colore più scuro nelle parti dove c’è rosso chiaro~~  
- [x] \[grafica\] mettere come watermark nella text box dove scrivi la domanda (scrivi domanda)  
- [x] \[grafica\] mettere qualcosa che segnali che sta lavorando  
- [x] \[grafica\] modificare segnali che sta lavorando e mettere qualcosa di figo  
- [ ] \[problema \- grafica\] sistemare la sequenza caricamento dopo la prima domanda  
- [x] pulire e sistemare il codice
## Fonti

* [Documentazione Avalonia](https://docs.avaloniaui.net/) (grafica)  
* [Documentazione GPT4All](https://docs.gpt4all.io/) (funzionalità)  
* Chat GPT (come assistente per funzionalità e grafica)  
* [Sito local docs](https://www.manualeduso.it/motore/honda?category=auto-barche-moto) (local docs gpt4all)  
* [Avalonia Tutorial: How To Show and Hide a Split View Pane with MVVM | SplitView | Cross Platform](https://youtu.be/oBpCrMsrx-0) (tutorial grafica)
