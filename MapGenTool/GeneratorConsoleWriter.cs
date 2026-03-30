using System.Text;

public class GeneratorConsoleWriter : TextWriter {
    private readonly TextWriter _inner;
    public override Encoding Encoding => _inner.Encoding;
    public string Prefix;

    public GeneratorConsoleWriter(TextWriter inner, string prefix) {
        _inner = inner;
        Prefix = prefix;
    }

    public override void Write(string? value) {
        _inner.Write(Prefix + value);
    }

    public override void WriteLine(string? value) {
        _inner.WriteLine(Prefix + value);
    }
}