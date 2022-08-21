using System.Collections;

namespace RandomBookmarkFolder;

internal sealed record BookmarkTreeNode(
	List<BookmarkTreeNode> Children,
	DateTime? DateAdded,
	DateTime? DateModified,
	long Id,
	Guid Guid,
	string Name,
	string? Url,
	BookmarkTreeNodeType Type ) : IEnumerable<BookmarkTreeNode>
{
	internal BookmarkTreeNode? Parent { get; private set; }

	private BookmarkTreeNode(
		BookmarkTreeNode? parent,
		List<BookmarkTreeNode> children,
		DateTime? dateAdded,
		DateTime? dateModified,
		long id,
		Guid guid,
		string name,
		string? url,
		BookmarkTreeNodeType type ) : this( children, dateAdded, dateModified, id, guid, name, url, type ) =>
		Parent = parent;

	private static DateTime? ChromeBookmarkDateToDateTime( string? date ) =>
		date is null ? null : new DateTime( 1601, 01, 01, 0, 0, 0, 0, DateTimeKind.Local ).AddMilliseconds( long.Parse( date ) / 1000 );

	private static BookmarkTreeNode FromRaw( RawBookmarkTreeNode raw )
	{
		var result = new BookmarkTreeNode(
			null,
			raw.Children?.Select( FromRaw ).ToList() ?? new List<BookmarkTreeNode>(),
			ChromeBookmarkDateToDateTime( raw.DateAdded ),
			ChromeBookmarkDateToDateTime( raw.DateModified ),
			long.Parse( raw.Id ),
			raw.Guid,
			raw.Name,
			raw.Url,
			Enum.Parse<BookmarkTreeNodeType>( raw.Type, true ) );
		foreach ( var child in result.Children )
			child.Parent = result;
		return result;
	}

	private static BookmarkTreeNode FromRoots( IEnumerable<RawBookmarkTreeNode> rawBookmarkTreeNodes )
	{
		var result = new BookmarkTreeNode(
			null,
			rawBookmarkTreeNodes.Select( FromRaw ).ToList(),
			null,
			null,
			default,
			default,
			"Root",
			null,
			BookmarkTreeNodeType.Folder );
		foreach ( var child in result.Children )
			child.Parent = result;
		return result;
	}

	private static BookmarkTreeNode FromRoots( params RawBookmarkTreeNode[] rawBookmarkTreeNodes ) =>
		FromRoots( rawBookmarkTreeNodes.AsEnumerable() );

	private static BookmarkTreeNode FromRawBookmarkRoots( RawBookmarkRoots roots ) =>
		FromRoots( roots.BookmarkBar, roots.Synced, roots.Other );

	internal static BookmarkTreeNode FromRawBookmarksFile( RawBookmarksFile file ) =>
		FromRawBookmarkRoots( file.Roots );

	internal IEnumerable<BookmarkTreeNode> AncestorsAndSelf()
	{
		yield return this;
		for ( var parent = Parent; parent is not null; parent = parent.Parent )
			yield return parent;
	}

	internal IEnumerable<BookmarkTreeNode> Path() =>
		AncestorsAndSelf().Reverse();

	internal string GetStringPath() =>
		string.Join( " > ", Path().Select( x => x.Name ) );

	internal IEnumerable<BookmarkTreeNode> DescendantsAndSelf()
	{
		yield return this;
		foreach ( var descendant in Descendants() )
			yield return descendant;
	}

	internal IEnumerable<BookmarkTreeNode> Descendants() =>
		Children.SelectMany( child => child.DescendantsAndSelf() );

	public IEnumerator<BookmarkTreeNode> GetEnumerator() =>
		DescendantsAndSelf().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}