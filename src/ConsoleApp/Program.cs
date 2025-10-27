using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Overefactor.Math.Annotations;
using Overefactor.Math.Set;
using Overefactor.Theory.FormalLanguage;

namespace ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = CfgBuilder.Empty;
        builder.SetStart("P");
        
        CreateTerminals(builder);
        
        builder.AddRule("W -> eol W")
            .AddRule("W -> ε");
        
        builder.AddRule("P -> W I C");
        
        builder.AddRule("I -> I' W I").AddRule("I -> ε")
            .AddRule("I' -> kw-import W lit-string kw-for W identifier eol");
        
        builder.AddRule("C -> C' W C").AddRule("C -> ε")
            .AddRule("C' -> kw-class identifier CB eol");
        
        builder.AddRule("CB -> sp-open-brace W CM sp-close-brace");
        
        builder.AddRule("CM -> CM' W CM")
            .AddRule("CM -> ε");
        
        builder.AddRule("CM' -> SFD");
        
        builder.AddRule("SFD -> kw-static identifier sp-open-paren PL sp-close-paren B eol");
        
        builder.AddRule("PL -> identifier PR'").AddRule("PL -> ε")
            .AddRule("PR -> PR' PR").AddRule("PR -> ε")
            .AddRule("PR' -> sp-comma identifier");

        builder.AddRule("B -> sp-open-brace B' sp-close-brace")
            .AddRule("B' -> E")
            .AddRule("B' -> ε")
            .AddRule("B' -> eol S");

        builder.AddRule("S -> S' W S").AddRule("S -> ε")
            .AddRule("S' -> B")
            .AddRule("S' -> DS")
            .AddRule("S' -> ES")
            .AddRule("S' -> WS")
            .AddRule("S' -> FS")
            .AddRule("S' -> BS")
            .AddRule("S' -> CS")
            .AddRule("S' -> RS")
            .AddRule("S' -> IS");

        builder.AddRule("DS -> kw-var identifier DS' eol")
            .AddRule("DS' -> op-equal E")
            .AddRule("DS' -> ε");

        builder.AddRule("ES -> E eol");

        builder.AddRule("WS -> kw-while sp-open-paren E sp-close-paren B eol");

        builder.AddRule("FS -> kw-for sp-open-paren identifier kw-in E sp-close-paren B eol");

        builder.AddRule("BS -> kw-break eol");
        builder.AddRule("CS -> kw-continue eol");
        
        builder.AddRule("RS -> kw-return RS' eol")
            .AddRule("RS' -> E")
            .AddRule("RS' -> ε");

        builder.AddRule("IS -> kw-if sp-open-paren E sp-close-paren B IS' eol")
            .AddRule("IS' -> kw-else B")
            .AddRule("IS' -> ε");

        builder.AddRule("E -> identifier")
            .AddRule("E -> op-minus")
            .AddRule("E -> op-not")
            .AddRule("E -> lit-null")
            .AddRule("E -> lit-whole")
            .AddRule("E -> lit-decimal")
            .AddRule("E -> lit-boolean")
            .AddRule("E -> lit-string");
        

        var cfg = builder.Build(CfgFactory.Default);
        
        var index = 1;
        var indexToRule = new Dictionary<int, CfgRule>();
        var ruleToIndex = new Dictionary<CfgRule, int>();
        foreach (var rule in cfg.Rules.OrderBy(r => r.ToString()))
        {
            indexToRule[index] = rule;
            ruleToIndex[rule] = index++;
        }

        index = 1;
        var indexToTerminal = new Dictionary<int, Symbol>();
        foreach (var symbol in cfg.Terminals.OrderBy(s => s.Name)) indexToTerminal[index++] = symbol;
        indexToTerminal[index] = Symbol.Eoi;

        index = 1;
        var indexToNonTerminal = new Dictionary<int, Symbol>();
        foreach (var symbol in cfg.NonTerminals.OrderBy(s => s.Name)) indexToNonTerminal[index++] = symbol;
        
        

        Console.WriteLine(new string('=', 50));
        Console.WriteLine("Id;Rule");
        foreach (var pair in indexToRule) Console.WriteLine($"{pair.Key};{pair.Value}");
        Console.WriteLine(new string('=', 50));
        

        var table = new int[cfg.NonTerminals.Count, cfg.Terminals.Count + 1];
        for (var i = 0; i < table.GetLength(0); i++)
        {
            for (var j = 0; j < table.GetLength(1); j++)
            {
                var nonTerminal = indexToNonTerminal[i + 1];
                var terminal = indexToTerminal[j + 1];

                var result = 0;
                
                foreach (var rule in cfg.GetRules(nonTerminal))
                {
                    if (!cfg.Predict(rule).Contains(terminal)) continue;

                    if (result != 0)
                    {
                        Console.WriteLine($"ERR: [{result}] ({indexToRule[result]}) <-> [{ruleToIndex[rule]}] ({rule})");
                    }
                        
                    result = result == 0 ? ruleToIndex[rule] : -1;
                }

                table[i, j] = result;
            }
        }
        
        Console.WriteLine();
        Console.WriteLine(new string('=', 50));
        
        Console.Write("\" \"");
        for (var i = 0; i < indexToTerminal.Count; i++)
        {
            var terminal = indexToTerminal[i + 1];
            Console.Write($";{terminal}");
        }
        Console.WriteLine();

        for (var i = 0; i < table.GetLength(0); i++)
        {
            var nonTerminal = indexToNonTerminal[i + 1];

            Console.Write(nonTerminal);
            for (var j = 0; j < table.GetLength(1); j++)
            {
                var value = table[i, j];
                
                Console.Write(';');
                Console.Write(value switch
                {
                    0 => "\" \"",
                    < 0 => "ERR",
                    _ => value.ToString()
                });
            }
            Console.WriteLine();
        }
        
        
        
        Console.WriteLine(new string('=', 50));
        
        
        

    }

    private static void CreateTerminals(CfgBuilder builder)
    {
        builder.AddTerminal("eof")
            .AddTerminal("eol");

        builder.AddTerminal("identifier");

        builder.AddTerminal("kw-class")
            .AddTerminal("kw-if")
            .AddTerminal("kw-else")
            .AddTerminal("kw-is")
            .AddTerminal("kw-in")
            .AddTerminal("kw-return")
            .AddTerminal("kw-var")
            .AddTerminal("kw-while")
            .AddTerminal("kw-static")
            .AddTerminal("kw-import")
            .AddTerminal("kw-for")
            .AddTerminal("kw-break")
            .AddTerminal("kw-continue")
            .AddTerminal("kw-null")
            .AddTerminal("kw-num")
            .AddTerminal("kw-bool")
            .AddTerminal("kw-string");

        builder.AddTerminal("sp-open-paren")
            .AddTerminal("sp-close-paren")
            .AddTerminal("sp-open-brace")
            .AddTerminal("sp-close-brace")
            .AddTerminal("sp-comma");

        builder.AddTerminal("lit-null")
            .AddTerminal("lit-whole")
            .AddTerminal("lit-decimal")
            .AddTerminal("lit-boolean")
            .AddTerminal("lit-string");

        builder.AddTerminal("op-minus")
            .AddTerminal("op-not")
            .AddTerminal("op-equal");

    }
}