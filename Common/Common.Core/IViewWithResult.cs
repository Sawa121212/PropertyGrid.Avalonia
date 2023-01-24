namespace Common.Core
{
    public interface IViewWithResult<out T>
    {
        T Result { get; }
    }
}