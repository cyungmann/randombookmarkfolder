using System.Text.Json.Serialization;

namespace RandomBookmarkFolder;

internal record RawBookmarksFile(
	int Version,
	[property: JsonPropertyName( "sync_metadata" )] string SyncMetadata,
	string Checksum,
	RawBookmarkRoots Roots );