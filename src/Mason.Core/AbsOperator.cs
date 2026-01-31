namespace Mason.Core;

public interface IRevitOperatorCook
{
    void Cook();
}

public interface IRevitOperatorCache<TCache>;

public abstract class AbsOperator<TCache>
    : AbsContext,
        IRevitOperatorCook,
        IRevitOperatorCache<TCache>
{
    public abstract void Cook();
}
