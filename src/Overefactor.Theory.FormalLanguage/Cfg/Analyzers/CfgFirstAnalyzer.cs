using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Overefactor.Math.Annotations;
using Overefactor.Math.Set;

namespace Overefactor.Theory.FormalLanguage;

public class CfgFirstAnalyzer : ICfgAnalyzer
{
    private Dictionary<Symbol, Set<Symbol>> _sets = [];
    
    public CfgFirstAnalyzer(Cfg grammar) => Grammar = grammar;

    public Cfg Grammar { get; }

    void ICfgAnalyzer.Compute()
    {
        _sets = [];

        foreach (var symbol in Grammar.Terminals) 
            _sets[symbol] = Set.Create(symbol);
        
        foreach (var symbol in Grammar.NonTerminals) 
            _sets[symbol] = [];

        bool changed;
        do
        {
            changed = false;

            foreach (var rule in Grammar.Rules)
            {
                var newSet = _sets[rule.Name];
                
                foreach (var symbol in rule.Symbols)
                {
                    newSet |= First(symbol);
                    
                    if (Grammar.Empty(symbol) == []) break;
                }
                
                if (newSet == _sets[rule.Name]) continue;

                _sets[rule.Name] = newSet;
                changed = true;
            }
        } while (changed);
    }

    public Set<Symbol> First(Symbol symbol)
    {
        return _sets[symbol];
    }
    
    
    public Set<Symbol> First(SententialForm form)
    {
        if (form == SententialForm.Empty) return [];
        
        var result = new Set<Symbol>();
        
        foreach (var symbol in form)
        {
            result |= First(symbol);

            if (Grammar.Empty(symbol) == []) break;
        }

        return result;
    } 

}

public static class CfgFirstExtensions
{
    public static Set<Symbol> First(this Cfg cfg, Symbol symbol)
    {
        return cfg.GetAnalyzer<CfgFirstAnalyzer>().First(symbol);
    }
    
    public static Set<Symbol> First(this Cfg cfg, SententialForm form)
    {
        return cfg.GetAnalyzer<CfgFirstAnalyzer>().First(form);
    }
}
