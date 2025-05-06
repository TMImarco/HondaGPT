# Prova esperta LLM Chat specializzata
## Overview
Progetto per la prova esperta del quarto anno del corso di informatica del ITT Barsanti-Galilei (As 2024-2025)
Creazione di un programma per un'AI specializzata locale, per questo scopo abbiamo scelto di utilizzare GPT4All.

### Settaggio di GPT4All per il funzionamento del programma
1. Aggiungere ai LocalDocs la cartella “..:/HondaGPT/HondaGPT/bin/Debug/net9.0/Resources/Manuali”;
2. Attivare il server API locale e inserire la porta locale 4891;
3. Installare i modelli “Llama 3 8B Instruct”, “DeepSeek-R1-Distill-Qwen-7B” e “Nous Hermes 2 Mistral DPO”.

## Funzionalità implementate
1. Invio di domande libere o richieste preimpostate;
2. Apertura diretta dei manuali tecnici associati a ciascuno dei modelli disponibili;
3. Gestione della cronologia delle conversazioni;
4. Selezione libera del modello AI tra i tre disponibili;
5. Modifica del numero massimo di tokens (lunghezza massima della risposta);
6. Regolazione del parametro di temperatura (creatività e precisione della risposta);
7. Personalizzazione:
  7.1 Possibilità di inserire o rimuovere manuali in formato PDF;
  7.2 Possibilità di aggiungere o togliere richieste preimpostate personalizzate.

### Sviluppi futuri
1. Possibilità di inviare foto del problema (impossibile per ora a causa di GPT4All);
2. Miglioramento interfaccia utente (auto selezione dei radio button);
3. Possibilità di modificare una domanda precedente e re inviarla.
