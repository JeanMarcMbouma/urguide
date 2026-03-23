using System;
using System.Reflection;
using System.Linq;

var assemblyPath = "/home/runner/.nuget/packages/bbq.outcome/1.0.16/lib/net10.0/BbQ.Outcome.dll";
var assembly = Assembly.LoadFrom(assemblyPath);
var outcomeType = assembly.GetType("BbQ.Outcome.Outcome`1");

Console.WriteLine("=== Outcome<T> Static Methods ===");
if (outcomeType != null)
{
    var methods = outcomeType.GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Where(m => !m.IsSpecialName)
        .OrderBy(m => m.Name)
        .ToList();
    
    foreach (var method in methods)
    {
        var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
        var returnType = method.ReturnType.Name;
        if (returnType.Contains("`")) returnType = returnType.Replace("`1", "<T>");
        Console.WriteLine($"  {method.Name}({parameters}): {returnType}");
    }
}
