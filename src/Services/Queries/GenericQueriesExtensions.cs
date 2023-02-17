namespace Services.Queries;

public static class GenericQueriesExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageSize, int pageNum)
    {
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize), pageSize, "Page size must be greater than 0");

        if (pageNum <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(pageNum), pageNum, "Page number must be greater than 0");

        if (pageNum != 1)
            query = query.Skip(pageSize * (pageNum - 1));

        return query.Take(pageSize);
    }
}
