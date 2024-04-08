namespace Project_MusicRadio.Models
{
    public class Song
    {
        public int IdSong { get; set; } = 0;
        public string NameSong { get; set; } = string.Empty;
        public string NameAlbum { get; set; } = string.Empty;

        public int AlbumId { get; set; } = 0;
    }
}

