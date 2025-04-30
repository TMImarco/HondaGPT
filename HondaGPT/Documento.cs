namespace HondaGPT;

public class Documento
{
    public string Nome { get; set; }
    public string Path { get; set; }

    public override string ToString()
    {
        return Nome;
    }
}