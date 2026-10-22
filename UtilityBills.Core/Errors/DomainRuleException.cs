using System.Diagnostics.CodeAnalysis;

namespace UtilityBills.Core.Errors;

/// <summary>Порушено правило предметної області.</summary>
[SuppressMessage(
    "Design",
    "CA1032:Implement standard exception constructors",
    Justification = "Виняток завжди створюється з ідентифікатором правила.")]
public class DomainRuleException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainRuleException"/> class.
    /// </summary>
    /// <param name="rule">Ідентифікатор правила, наприклад "bill.payable".</param>
    /// <param name="message">Повідомлення з конкретними значеннями.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainRuleException"/> class.
    /// </summary>
    /// <param name="rule">Ідентифікатор правила.</param>
    /// <param name="message">Повідомлення з конкретними значеннями.</param>
    /// <param name="inner">Початковий виняток.</param>
    public DomainRuleException(string rule, string message, Exception inner)
        : base(message, inner)
    {
        Rule = rule;
    }

    /// <summary>Gets the ідентифікатор порушеного правила.</summary>
    public string Rule { get; }
}
