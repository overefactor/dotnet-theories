using System.Collections.Generic;
using Overefactor.Math.Set;

namespace Overefactor.Theory.FormalLanguage;

public abstract class CfgSetAnalyzer : ICfgAnalyzer
{
    private Dictionary<Symbol, Set<Symbol>> _sets = [];

    protected CfgSetAnalyzer(Cfg grammar) => Grammar = grammar;

    public Cfg Grammar { get; }

    public void Compute()
    {
        
    }

    protected abstract void Initialize();
}
