namespace Common.Core
{
    public interface IResult<out TResult>
    {
        TResult GetResult();
    }
}