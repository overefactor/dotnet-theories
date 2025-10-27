using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Overefactor.Theory.FormalLanguage;

public readonly struct SententialForm : IEquatable<SententialForm>, IEnumerable<Symbol>
{
    private readonly ImmutableArray<Symbol> _symbols;
    
    
    public SententialForm() => _symbols = [Symbol.Epsilon];

    private SententialForm(ImmutableArray<Symbol> symbols) => _symbols = symbols;

    public static SententialForm Empty { get; } = new();

    public static SententialForm Create(params IEnumerable<Symbol> symbols)
    {
        var immutableSymbols = symbols.Where(s => s != Symbol.Epsilon).ToImmutableArray();
        if (immutableSymbols.Length == 0) return Empty;

        return new SententialForm(immutableSymbols);
    }

    public static SententialForm Concat(params IEnumerable<SententialForm> forms)
    {
        return Create(forms.SelectMany(f => f._symbols));
    }
    
    public Symbol this[int index]
    {
        get
        {
            if (index < 0) return Symbol.Epsilon;
            if (index >= Length) return Symbol.Epsilon;
            
            return _symbols[index];
        }
    }

    public Symbol this[Index index] => this[index.GetOffset(Length)];

    public SententialForm this[Range range]
    {
        get
        {
            var start = range.Start.GetOffset(Length);
            var end = range.End.GetOffset(Length);
            
            if (start < 0) start = 0;
            if (start > Length) start = Length;
            
            if (end < 0) end = 0;
            if (end > Length) end = Length;

            if (start == end) return Empty;
            
            return new SententialForm(_symbols[start..end]);
        }
    }

    public int Length => _symbols.Length;

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        foreach (var symbol in _symbols)
        {
            sb.Append(symbol);
            sb.Append(' ');
        }

        sb.Length--;
        return sb.ToString();
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        foreach (var symbol in _symbols)
            yield return symbol;
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static bool operator ==(SententialForm left, SententialForm right) => left.Equals(right);

    public static bool operator !=(SententialForm left, SententialForm right) => !left.Equals(right);

    public static SententialForm operator +(SententialForm left, Symbol right) => Create([..left._symbols, right]);

    public static SententialForm operator +(Symbol left, SententialForm right) =>
        Create([left, ..right._symbols]);

    public static SententialForm operator +(SententialForm left, SententialForm right) =>
        Create([..left._symbols, ..right._symbols]);
    

    public bool Equals(SententialForm other) => _symbols.SequenceEqual(other._symbols);
    
    public override bool Equals(object obj) => obj is SententialForm other && Equals(other);

    public override int GetHashCode() => _symbols.GetHashCode();

    public int FirstIndexOf(Func<Symbol, bool> predicate)
    {
        var index = 0;
        foreach (var symbol in _symbols)
        {
            if (predicate(symbol)) return index;
            
            index++;
        }

        return -1;
    }
}