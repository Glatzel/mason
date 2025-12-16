namespace Mason.Core;

/// <summary>
/// Defines a Revit operator that performs a unit of work.
/// </summary>
public interface IRevitOperatorCook
{
    /// <summary>
    /// Executes the operator's main action.
    /// </summary>
    void Cook();
}

/// <summary>
/// Defines a Revit operator that maintains a cache of type <typeparamref name="TCache"/>.
/// </summary>
public interface IRevitOperatorCache<TCache>
{
    // You may later add members like:
    // TCache Cache { get; set; }
}

/// <summary>
/// Abstract base class for Revit operators that have context and optional caching.
/// </summary>
/// <typeparam name="TCache">The cache type associated with this operator.</typeparam>
public abstract class AbsOperator<TCache>
    : AbsContext,
        IRevitOperatorCook,
        IRevitOperatorCache<TCache>
{
    /// <inheritdoc />
    public abstract void Cook();
}
