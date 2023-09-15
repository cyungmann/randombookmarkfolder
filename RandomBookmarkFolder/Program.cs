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
var rnd = new Random();
for ( var i = 0; i < 100; ++i )
	Console.WriteLine( RandomWalk( booksFolder, rnd ).GetStringPath() );

static BookmarkTreeNode RandomWalk(BookmarkTreeNode root, Random random)
{
	while (true)
	{
		var chooseFrom = root.Children.Where( x => x.Type == BookmarkTreeNodeType.Folder ).ToList();
		var i = random.Next( chooseFrom.Count + 1 );
		if ( i == chooseFrom.Count )
			return root;
		root = chooseFrom[i];
	}
}