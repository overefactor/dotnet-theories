namespace Overefactor.Theory.FormalLanguage;

public interface ICfgAnalyzer
{
    Cfg Grammar { get; }

    void Compute();
}