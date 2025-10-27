using System;

namespace Overefactor.Math.Annotations;

public class MathAnnotationAttribute : Attribute
{
    private readonly string _name;

    public MathAnnotationAttribute(string name) => _name = name;
}