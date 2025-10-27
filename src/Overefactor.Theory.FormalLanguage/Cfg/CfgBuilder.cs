using System;
using System.Collections.Generic;
using System.Linq;

namespace Overefactor.Theory.FormalLanguage;

public class CfgBuilder
{
    private readonly HashSet<Symbol> _terminals = [];
    private readonly Dictionary<Symbol, List<CfgRule>> _rules = [];
    private Symbol _start;

    public CfgBuilder()
    {
        
    }

    public static CfgBuilder Empty => new CfgBuilder();
    
    public CfgBuilder AddTerminal(Symbol name)
    {
        if (_rules.ContainsKey(name)) 
            throw new ArgumentException($"Cannot add terminal: N ∩ T = ∅ and `{name}` ∈ N", nameof(name));

        if (_terminals.Add(name)) return this;

        throw new ArgumentException($"Cannot add terminals: N ∩ T = ∅ and `{name}` ∈ T");
    }

    public CfgBuilder AddRule(string rule) => AddRule(CfgRule.Parse(rule));

    public CfgBuilder AddRule(CfgRule rule)
    {
        if (_terminals.Contains(rule.Name))
            throw new ArgumentException($"Cannot add rule: N ∩ T = ∅ and `{rule.Name}` ∈ T", nameof(rule));

        if (_rules.TryGetValue(rule.Name, out var rules))
        {
            rules.Add(rule);
            return this;
        }

        _rules.Add(rule.Name, [rule]);
        return this;
    }


    public CfgBuilder SetStart(Symbol symbol)
    {
        if (_terminals.Contains(symbol)) throw new ArgumentException($"Cannot set start: `{symbol}` ∈ T");
            
        _start = symbol;
        return this;
    }

    public Cfg Build(ICfgFactory factory) => factory.Create(_terminals, _rules.SelectMany(pair => pair.Value).ToArray(), _start);
}