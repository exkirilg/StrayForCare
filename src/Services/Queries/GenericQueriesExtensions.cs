namespace Services.Queries;

public static class GenericQueriesExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageSize, int pageStartZeroBased)
    {
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize), pageSize, "Page size cannot be less or equal to zero");

        if (pageStartZeroBased < 0)
            throw new ArgumentOutOfRangeException(
                nameof(pageStartZeroBased), pageStartZeroBased, "Page start cannot be less than zero");

        if (pageStartZeroBased != 0)
            query = query.Skip(pageStartZeroBased);

        return query.Take(pageSize);
    }
}
