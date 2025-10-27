using System;
using System.Collections.Generic;

namespace Overefactor.Theory.FormalLanguage;

public interface ICfgFactory
{
    ICfgFactory RegisterAnalyzer<T>(Func<Cfg, T> factory) where T : class, ICfgAnalyzer;
    
    Cfg Create(IReadOnlySet<Symbol> terminals, IReadOnlyList<CfgRule> rules, Symbol symbol);
}