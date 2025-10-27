using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overefactor.Math.Set;

public class Set
{
    public static Set<T> Create<T>(params IEnumerable<T> values) => Set<T>.Create(values);

    public static Set<T> Union<T>(params IEnumerable<Set<T>> values)
    {
        return Set<T>.Create(values.SelectMany(s => s));
    }
}

public class Set<T> : IEnumerable<T>, IEquatable<Set<T>>
{
    private readonly HashSet<T> _hashSet;


    public Set() => _hashSet = [];

    private Set(params IEnumerable<T> values) => _hashSet = [..values];

    public static Set<T> Empty => new();

    public int Count => _hashSet.Count;
    
    public static Set<T> Create(params IEnumerable<T> values)
    {
        return new Set<T>(values);
    }
    



    public static bool operator ==(Set<T> left, Set<T> right)
    {
        if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
        
        return left.Equals(right);
    }

    public static bool operator !=(Set<T> left, Set<T> right) => !(left == right);

    public static Set<T> operator |(Set<T> left, Set<T> right)
    {
        var result = new Set<T>();
        foreach (var item in left) result._hashSet.Add(item);
        foreach (var item in right) result._hashSet.Add(item);

        return result;
    }
    
    public static Set<T> operator ^(Set<T> left, Set<T> right)
    {
        var result = new Set<T>();
        foreach (var item in left)
        {
            if (!right.Contains(item)) continue;

            result._hashSet.Add(item);
        }

        return result;
    }
    

    public override string ToString()
    {
        if (_hashSet.Count == 0) return "∅";
        
        var sb = new StringBuilder();
        sb.Append('{');
        
        foreach (var item in _hashSet)
        {
            sb.Append(item);
            sb.Append(' ');
        }

        sb.Length--;
        sb.Append('}');
        return sb.ToString();
    }

    public bool Contains(T item) => _hashSet.Contains(item);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();

    public bool Equals(Set<T> other)
    {
        if (ReferenceEquals(other, null)) return false;
        
        return _hashSet.SetEquals(other._hashSet);
    }

    public override bool Equals(object obj) => obj is Set<T> set && Equals(set);

    public override int GetHashCode() => _hashSet.GetHashCode();
}
