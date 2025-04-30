using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HondaGPT;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new ViewModel();
    }

    //finestra di dialogo di windows per far scegliere il dile
    private async void ApriFileAsync(object? sender, RoutedEventArgs e)
    {
        var ApriFileDialog = new OpenFileDialog
        {
            AllowMultiple = false
        };

        var result = await ApriFileDialog.ShowAsync(this);

        if (result != null && result.Length > 0)
        {
            var nomeFile = result[0]; //percorso del file selezionato
            //passo la variabile e il metodo al viewmodel
            (DataContext as ViewModel).SelezionaFile = nomeFile;
            (DataContext as ViewModel).ApriFile();
        }
    }
}