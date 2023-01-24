namespace Common.Core.Extensions
{
    /// <summary>
    /// Методы расширения для <see cref="IComparable" />.
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        /// Проверка на вхождения в диапазон значений.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="lowerBound">Нижняя граница.</param>
        /// <param name="upperBound">Верхняя граница.</param>
        /// <param name="includeLowerBound">Включить нижнюю границу.</param>
        /// <param name="includeUpperBound">Включить верхнюю границу.</param>
        /// <returns>true - Входит в указанный диапазон.</returns>
        public static bool IsBetween(this IComparable value, IComparable lowerBound, IComparable upperBound,
            bool includeLowerBound, bool includeUpperBound)
        {
            var lower = value.CompareTo(lowerBound);
            var upper = value.CompareTo(upperBound);
            return (lower > 0 || includeLowerBound && lower == 0)
                   && (upper < 0 || includeUpperBound && upper == 0);
        }
    }
}
