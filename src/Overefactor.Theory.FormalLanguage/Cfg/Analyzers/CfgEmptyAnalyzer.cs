using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using Overefactor.Math.Set;

namespace Overefactor.Theory.FormalLanguage;

public class CfgEmptyAnalyzer : ICfgAnalyzer
{
    private Dictionary<Symbol, Set<Symbol>> _sets = [];
    
    public CfgEmptyAnalyzer(Cfg grammar) => Grammar = grammar;

    public Cfg Grammar { get; }
    
    void ICfgAnalyzer.Compute()
    {
        _sets = [];
        
        foreach (var symbol in Grammar.Terminals) 
            _sets[symbol] = [];

        foreach (var symbol in Grammar.NonTerminals)
            _sets[symbol] = [];

        bool changed;
        do
        {
            changed = false;
            
            foreach (var rule in Grammar.NonTerminals.SelectMany(s => Grammar.GetRules(s)))
            {
                var empty = ComputeEmptyForForm(SententialForm.Create(rule.Symbols));
                if (empty == []) continue;

                var newSet = _sets[rule.Name] | empty;
                if (newSet == _sets[rule.Name]) continue;

                _sets[rule.Name] = newSet;
                changed = true;
            }
        } while (changed);
    }

    public Set<Symbol> Empty(SententialForm form) => ComputeEmptyForForm(form);

    public Set<Symbol> Empty(Symbol symbol) => _sets[symbol];

    private Set<Symbol> ComputeEmptyForForm(SententialForm form)
    {
        if (form == SententialForm.Empty || form.All(symbol => _sets[symbol].Contains(Symbol.Epsilon))) 
            return Set.Create(Symbol.Epsilon);

        return [];
    }
}

public static class CfgEmptyExtensions
{
    public static Set<Symbol> Empty(this Cfg cfg, Symbol symbol)
    {
        return cfg.GetAnalyzer<CfgEmptyAnalyzer>().Empty(symbol);
    }
    
    public static Set<Symbol> Empty(this Cfg cfg, SententialForm form)
    {
        return cfg.GetAnalyzer<CfgEmptyAnalyzer>().Empty(form);
    }
}
