using RandomBookmarkFolder;
using System.Text.Json;

await using var fs = new FileStream(
	Path.Join( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "Google", "Chrome", "User Data", "Default", "Bookmarks" ),
	FileMode.Open,
	FileAccess.Read,
	FileShare.ReadWrite );
var bookmarksFile = ( await JsonSerializer.DeserializeAsync<RawBookmarksFile>( fs, new JsonSerializerOptions() {
	PropertyNameCaseInsensitive = true,
} ) )!;
var bookmarkTree = BookmarkTreeNode.FromRawBookmarksFile( bookmarksFile );
var booksFolder = bookmarkTree.Single( x => x.Type == BookmarkTreeNodeType.Folder && x.Name == "books" );
var folders = booksFolder.Descendants().Where( x => x.Type == BookmarkTreeNodeType.Folder ).ToArray();
var rnd = new Random();
for ( var i = 0; i < 20; ++i )
	Console.WriteLine( folders[rnd.Next( folders.Length )].GetStringPath() );
