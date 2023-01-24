namespace Common.Core
{
    public interface IInitializable<in T>
    {
        void Initialize(T param);
    }
    public interface IInitializable
    {
        /// <summary>
        /// Инициализация модели представления.
        /// </summary>
        void Initialize();
    }
}