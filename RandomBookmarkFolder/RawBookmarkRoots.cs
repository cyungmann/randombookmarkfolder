using System.Text.Json.Serialization;

namespace RandomBookmarkFolder;

internal record RawBookmarkRoots(
	[property: JsonPropertyName( "bookmark_bar" )] RawBookmarkTreeNode BookmarkBar,
	RawBookmarkTreeNode Other,
	RawBookmarkTreeNode Synced );