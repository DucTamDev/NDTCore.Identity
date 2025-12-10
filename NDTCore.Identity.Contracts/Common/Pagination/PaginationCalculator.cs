namespace NDTCore.Identity.Contracts.Common.Pagination;

/// <summary>
/// Provides helper methods for calculating pagination indexes and counts.
/// </summary>
public static class PaginationCalculator
{
    /// <summary>
    /// Gets the starting index (0-based) of items on the current page.
    /// </summary>
    public static int GetStartIndex(PaginationMetadata metadata)
        => (metadata.CurrentPage - 1) * metadata.PageSize;

    /// <summary>
    /// Gets the ending index (0-based) of items on the current page.
    /// </summary>
    public static int GetEndIndex(PaginationMetadata metadata)
        => Math.Min(GetStartIndex(metadata) + metadata.PageSize - 1, metadata.TotalRecords - 1);

    /// <summary>
    /// Gets the number of items expected on the current page.
    /// </summary>
    public static int GetItemsCount(PaginationMetadata metadata)
    {
        if (metadata.TotalRecords <= 0)
            return 0;

        int remaining = metadata.TotalRecords - GetStartIndex(metadata);
        return Math.Min(metadata.PageSize, remaining);
    }
}
