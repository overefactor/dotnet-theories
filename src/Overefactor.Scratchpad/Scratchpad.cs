using System;
using System.IO;

namespace Overefactor.Math.Annotations;

public class Scratchpad : IDisposable
{
    private readonly TextWriter _writer;

    private Scratchpad(TextWriter writer)
    {
        _writer = writer;
    }

    public static Scratchpad Begin(TextWriter writer) => new(writer);
    
    public void Write(string value) => _writer.Write(value);

    public void WriteLine(object value) => _writer.WriteLine(value);

    public void Dispose() => _writer.Flush();
}