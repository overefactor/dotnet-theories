using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Overefactor.Math.Set;

namespace Overefactor.Theory.FormalLanguage;

public class Cfg
{
    private HashSet<ICfgAnalyzer> _computedAnalyzers = [];
    private readonly ImmutableDictionary<Type, ICfgAnalyzer> _analyzers;
    
    private readonly ImmutableDictionary<Symbol, Set<CfgRule>> _rules;

    internal Cfg(Set<Symbol> terminals, ImmutableDictionary<Symbol, Set<CfgRule>> rules,
        Symbol start, ImmutableDictionary<Type, Func<Cfg, ICfgAnalyzer>> analyzerFactories)
    {
        Terminals = terminals;
        NonTerminals = Set.Create(rules.Keys);
        Rules = Set.Union(rules.Values);
        
        _rules = rules;
        Start = start;

        var analyzers = ImmutableDictionary.CreateBuilder<Type, ICfgAnalyzer>();
        foreach (var (key, factory) in analyzerFactories) 
            analyzers.Add(key, factory(this));

        _analyzers = analyzers.ToImmutable();
    }
    
    public Symbol Start { get; }
    
    public Set<Symbol> Terminals { get; }
    
    public Set<Symbol> NonTerminals { get; }
    
    public Set<CfgRule> Rules { get; }
    
    [ContractAnnotation("=> true, rules: notnull; => false, rules: null")]
    public bool TryGetRules(Symbol nonTerminal, out Set<CfgRule> rules)
    {
        if (_rules.TryGetValue(nonTerminal, out var set))
        {
            rules = set;
            return true;
        }

        rules = null;
        return false;
    }

    public Set<CfgRule> GetRules(Symbol nonTerminal)
    {
        if (_rules.TryGetValue(nonTerminal, out var rules)) return rules;

        throw new ArgumentException($"`{nonTerminal}` ∉ N", nameof(nonTerminal));
    }

    public T GetAnalyzer<T>()
    {
        if (!_analyzers.TryGetValue(typeof(T), out var analyzer))
            throw new InvalidOperationException($"No factory registered for analyzer type {typeof(T)}");

        if (_computedAnalyzers.Add(analyzer)) analyzer.Compute();
        
        return (T)analyzer;
    }

    public IEnumerable<(SententialForm form, int derivation)> DeriveLeftIteratively(SententialForm form)
    {
        var queue = new Queue<(SententialForm form, int derivation)>();
        queue.Enqueue((form, 0));

        while (queue.TryDequeue(out var item))
        {
            yield return item;

            foreach (var derivedForm in DeriveLeft(item.form)) 
                queue.Enqueue((derivedForm, item.derivation + 1));
        }
    }
    
    public IEnumerable<SententialForm> DeriveLeft(SententialForm form, int derivation)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(derivation, 0);
        
        var queue = new Queue<(SententialForm form, int derivation)>();
        queue.Enqueue((form, 0));

        while (queue.TryDequeue(out var item))
        {
            if (item.derivation == derivation)
            {
                yield return item.form;
                continue;
            }

            foreach (var derivedForm in DeriveLeft(item.form)) 
                queue.Enqueue((derivedForm, item.derivation + 1));
        }
    }

    public IEnumerable<SententialForm> DeriveLeft(SententialForm form)
    {
        if (form == SententialForm.Empty) yield break;

        var index = form.FirstIndexOf(s => NonTerminals.Contains(s));
        if (index == -1) yield break;
        
        foreach (var rule in GetRules(form[index]))
        {
            yield return form[..index] + SententialForm.Create(rule.Symbols) + form[(index + 1)..];
        }
    }
}