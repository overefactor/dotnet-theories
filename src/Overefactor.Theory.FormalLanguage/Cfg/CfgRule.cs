using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Overefactor.Theory.FormalLanguage;

public readonly struct CfgRule : IEquatable<CfgRule>
{
    public CfgRule(Symbol name, params IEnumerable<Symbol> expression)
    {
        Name = name;
        Symbols = [..expression];
    }

    private static Regex RuleRegex { get; } =
        new(@"^(?<name>[A-Z][A-Z0-9'-_]*)\s*->\s*(?<expr>[a-zA-Z0-9'-_ ]+|[ε])$", RegexOptions.Compiled);
    
    public Symbol Name { get; }
    
    public ImmutableArray<Symbol> Symbols { get; }
    
    
    public static CfgRule Parse(string input)
    {
        var match = RuleRegex.Match(input);
        if (!match.Success) throw new ArgumentException($"Rule `{input} is not a valid rule.", nameof(input));

        var name = match.Groups["name"].Value;
        var expression = match.Groups["expr"].Value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(Symbol.Create)
            .Where(e => e != Symbol.Epsilon);
        
        return new CfgRule(name, expression);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Name);
        sb.Append(" ->");
        
        foreach (var item in Symbols)
        {
            sb.Append(' ');
            sb.Append(item);
        }

        if (Symbols.Length > 0) return sb.ToString();
        
        sb.Append(' ');
        sb.Append(FormalLanguageTheory.Epsilon[0]);

        return sb.ToString();
    }

    

    public bool Equals(CfgRule other) => Name == other.Name && Symbols.SequenceEqual(other.Symbols);

    public override bool Equals(object obj) => obj is CfgRule other && Equals(other);

    public override int GetHashCode() => Symbols.Aggregate(Name.GetHashCode(), HashCode.Combine);

    public static bool operator ==(CfgRule left, CfgRule right) => left.Equals(right);

    public static bool operator !=(CfgRule left, CfgRule right) => !(left == right);
}