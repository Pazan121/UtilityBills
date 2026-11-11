using System.Reflection;
using UtilityBills.Core.Domain;
using Xunit;

namespace UtilityBills.Tests;

public class ArchitectureTests
{
    private const BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic
        | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly Assembly Core = typeof(UtilityBill).Assembly;

    private static readonly string[] ApplicationLayer =
    {
        "UtilityBills.Core.Abstractions", "UtilityBills.Core.Billing", "UtilityBills.Core.Pricing",
        "UtilityBills.Core.Services", "UtilityBills.Core.Reporting", "UtilityBills.Core.Errors",
        "UtilityBills.Core.Parsing",
    };

    private static readonly string[] InfrastructureLayer =
    {
        "UtilityBills.Core.Infrastructure",
        "UtilityBills.Core.Storage",
        "UtilityBills.Core.Auditing",
    };

    [Fact]
    public void Domain_DoesNotDependOnOtherLayers()
    {
        var violations = DependenciesOf(ns => ns == "UtilityBills.Core.Domain")
            .Where(ns => ns.StartsWith("UtilityBills.", StringComparison.Ordinal)
                && ns != "UtilityBills.Core.Domain")
            .Distinct()
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void Application_DoesNotDependOnInfrastructure()
    {
        var violations = DependenciesOf(ns => ApplicationLayer.Contains(ns))
            .Where(ns => InfrastructureLayer.Contains(ns))
            .Distinct()
            .ToList();

        Assert.Empty(violations);
    }

    private static IEnumerable<string> DependenciesOf(Func<string, bool> inLayer)
    {
        IEnumerable<Type> layerTypes = Core.GetTypes()
            .Where(type => type.Namespace is not null && inLayer(type.Namespace));
        foreach (Type type in layerTypes)
        {
            foreach (Type used in UsedTypes(type).SelectMany(Flatten))
            {
                if (used.Namespace is not null)
                {
                    yield return used.Namespace;
                }
            }
        }
    }

    private static IEnumerable<Type> UsedTypes(Type type)
    {
        if (type.BaseType is not null)
        {
            yield return type.BaseType;
        }

        foreach (Type contract in type.GetInterfaces())
        {
            yield return contract;
        }

        foreach (FieldInfo field in type.GetFields(AllMembers))
        {
            yield return field.FieldType;
        }

        foreach (PropertyInfo property in type.GetProperties(AllMembers))
        {
            yield return property.PropertyType;
        }

        foreach (MethodBase method in type.GetMethods(AllMembers).Cast<MethodBase>()
            .Concat(type.GetConstructors(AllMembers)))
        {
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                yield return parameter.ParameterType;
            }
        }
    }

    private static IEnumerable<Type> Flatten(Type type)
    {
        yield return type;
        if (type.HasElementType && type.GetElementType() is Type element)
        {
            foreach (Type inner in Flatten(element))
            {
                yield return inner;
            }
        }

        if (type.IsGenericType)
        {
            foreach (Type argument in type.GetGenericArguments().SelectMany(Flatten))
            {
                yield return argument;
            }
        }
    }
}
