using NDTCore.Identity.Contracts.Common.Results;

namespace NDTCore.Identity.Contracts.Common.Pagination;

/// <summary>
/// Represents a paginated collection of items with pagination metadata.
/// Encapsulates both the data items and pagination information in a single model.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public sealed record PaginatedCollection<T>
{
    /// <summary>
    /// The collection of items on the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>
    /// Pagination metadata containing page information.
    /// </summary>
    public PaginationMetadata Metadata { get; init; } = PaginationMetadata.Empty;

    public PaginatedCollection(IReadOnlyList<T> items, PaginationMetadata pagination)
    {
        Items = items ?? Array.Empty<T>();
        Metadata = pagination ?? PaginationMetadata.Empty;
    }

    /// <summary>
    /// Gets the number of items in the current page.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Indicates whether the current page has no items.
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Indicates whether the current page has any items.
    /// </summary>
    public bool HasItems => Count > 0;

    /// <summary>
    /// Gets the first item in the collection, or default if empty.
    /// </summary>
    public T? FirstOrDefault => Items.Count > 0 ? Items[0] : default;

    /// <summary>
    /// Gets the last item in the collection, or default if empty.
    /// </summary>
    public T? LastOrDefault => Items.Count > 0 ? Items[Items.Count - 1] : default;
}
