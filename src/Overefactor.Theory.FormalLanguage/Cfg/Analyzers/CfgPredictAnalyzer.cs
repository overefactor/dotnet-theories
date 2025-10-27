using Overefactor.Math.Set;

namespace Overefactor.Theory.FormalLanguage;

public class CfgPredictAnalyzer : ICfgAnalyzer
{
    public CfgPredictAnalyzer(Cfg grammar) => Grammar = grammar;

    public Cfg Grammar { get; }
    
    public void Compute()
    {
        
    }

    public Set<Symbol> Predict(CfgRule rule)
    {
        var x = SententialForm.Create(rule.Symbols);

        if (Grammar.Empty(x) != []) return Grammar.First(x) | Grammar.Follow(rule.Name); 
        
        return Grammar.First(x);
    }
}

public static class CfgPredictExtensions
{
    public static Set<Symbol> Predict(this Cfg cfg, CfgRule rule)
    {
        return cfg.GetAnalyzer<CfgPredictAnalyzer>().Predict(rule);
    }
}
