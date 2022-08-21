using System.Text.Json.Serialization;

namespace RandomBookmarkFolder;

internal record RawBookmarkTreeNode(
	List<RawBookmarkTreeNode>? Children,
	[property: JsonPropertyName( "date_added" )] string? DateAdded,
	[property: JsonPropertyName( "date_modified" )] string? DateModified,
	string Id,
	Guid Guid,
	string Name,
	string? Url,
	string Type )
{
	internal IEnumerable<RawBookmarkTreeNode> DescendantsAndSelf()
	{
		yield return this;
		foreach ( var child in Children ?? Enumerable.Empty<RawBookmarkTreeNode>() )
			foreach ( var x in child.DescendantsAndSelf() )
				yield return x;
	}

	internal IEnumerable<RawBookmarkTreeNode> Descendants()
	{
		foreach ( var child in Children ?? Enumerable.Empty<RawBookmarkTreeNode>() )
			foreach ( var x in child.DescendantsAndSelf() )
				yield return x;
	}
}