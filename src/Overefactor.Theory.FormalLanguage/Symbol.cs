using System;

namespace Overefactor.Theory.FormalLanguage;

public readonly struct Symbol : IEquatable<Symbol>
{
    public Symbol() : this(FormalLanguageTheory.Epsilon)
    {
        
    }

    private Symbol(string name) => Name = name;

    public static Symbol Create(string name) => new Symbol(name);
    
    public static Symbol Epsilon { get; } = new();

    public static Symbol Eoi { get; } = new("$");

    public static Symbol Soi { get; } = new("$");

    public string Name { get; }

    public static bool operator ==(Symbol left, Symbol right) => left.Equals(right);

    public static bool operator !=(Symbol left, Symbol right) => !left.Equals(right);

    public static implicit operator Symbol(string value) => new Symbol(value);

    public override string ToString() => Name;

    public bool Equals(Symbol other) => Name == other.Name;

    public override bool Equals(object obj) => obj is Symbol other && Equals(other);

    public override int GetHashCode() => Name != null ? Name.GetHashCode() : 0;
}