using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Overefactor.Math.Set;

namespace Overefactor.Theory.FormalLanguage;

public class CfgFactory : ICfgFactory
{
    private readonly Dictionary<Type, Func<Cfg, ICfgAnalyzer>> _analyzerFactories = new();

    public static ICfgFactory Default {
        get
        {
            var factory = new CfgFactory();
            factory.RegisterAnalyzer(cfg => new CfgEmptyAnalyzer(cfg))
                .RegisterAnalyzer(cfg => new CfgFirstAnalyzer(cfg))
                .RegisterAnalyzer(cfg => new CfgFollowAnalyzer(cfg))
                .RegisterAnalyzer(cfg => new CfgPredictAnalyzer(cfg));
            
            return factory;
        } 
    } 


    public ICfgFactory RegisterAnalyzer<T>(Func<Cfg, T> factory) where T : class, ICfgAnalyzer
    {
        _analyzerFactories[typeof(T)] = factory;
        return this;
    }

    public Cfg Create(IReadOnlySet<Symbol> terminals, IReadOnlyList<CfgRule> rules, Symbol symbol)
    {
        Set<Symbol> terminalsMathSet = Set.Create(terminals);

        var nonTerminals = ImmutableDictionary.CreateBuilder<Symbol, Set<CfgRule>>();
        foreach (var rule in rules) nonTerminals.TryAdd(rule.Name, []);

        if (!nonTerminals.ContainsKey(symbol))
            throw new ArgumentException($"Cannot create cfg with start: `{symbol}` ∈ N");

        foreach (var rule in rules)
        {
            if (terminalsMathSet.Contains(rule.Name))
                throw new ArgumentException($"Rule {rule} cannot extend terminal: `{rule.Name}` ∈ T", nameof(rules));

            var errorSymbol = rule.Symbols.FirstOrDefault(s =>
                !nonTerminals.ContainsKey(s) && !terminalsMathSet.Contains(s));

            if (errorSymbol != null)
                throw new ArgumentException($"Rule {rule} uses undefined symbol: `{errorSymbol}` ∉ (N ∪ T)",
                    nameof(rules));

            nonTerminals[rule.Name] |= Set.Create(rule);
        }

        return new Cfg(terminalsMathSet, nonTerminals.ToImmutable(), symbol,
            _analyzerFactories.ToImmutableDictionary());
    }
}