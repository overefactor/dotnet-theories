using System.Collections.Generic;
using System.Linq;
using Overefactor.Math.Set;

namespace Overefactor.Theory.FormalLanguage;

public class CfgFollowAnalyzer : ICfgAnalyzer
{
    private Dictionary<Symbol, Set<Symbol>> _sets = [];

    public CfgFollowAnalyzer(Cfg grammar) => Grammar = grammar;

    public Cfg Grammar { get; }
    
    public void Compute()
    {
        _sets = [];
        
        foreach (var symbol in Grammar.NonTerminals) _sets[symbol] = [];
        _sets[Grammar.Start] = Set.Create(Symbol.Eoi);
        
        bool changed;
        
        do
        {
            changed = false;

            foreach (var rule in Grammar.Rules)
            {
                for (var i = 0; i < rule.Symbols.Length; i++)
                {
                    var nonTerminal = rule.Symbols[i];
                    if (Grammar.Terminals.Contains(nonTerminal)) continue;
                    
                    var y = SententialForm.Create(rule.Symbols)[(i + 1)..];
                    
                    var newSet = _sets[nonTerminal];
                    if (y != SententialForm.Empty) newSet |= Grammar.First(y);
                    if (Grammar.Empty(y) != []) newSet |= _sets[rule.Name];
                    
                    if (newSet == _sets[nonTerminal]) continue;

                    _sets[nonTerminal] = newSet;
                    changed = true;
                }
            }
            
        } while (changed);
    }

    public Set<Symbol> Follow(Symbol symbol)
    {
        return _sets[symbol];
    }
}

public static class CfgFollowExtensions
{
    public static Set<Symbol> Follow(this Cfg cfg, Symbol symbol)
    {
        return cfg.GetAnalyzer<CfgFollowAnalyzer>().Follow(symbol);
    }
}