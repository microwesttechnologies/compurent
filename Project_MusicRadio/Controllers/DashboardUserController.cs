using Microsoft.AspNetCore.Mvc;
using Project_MusicRadio.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Project_MusicRadio.Controllers
{
    public class DashboardUserController : Controller
    {
        static string connectionString = "Data Source=(localdb)\\microwest; Initial Catalog=MUSICRADIODB;Integrated Security=true";

        public IActionResult DashboardUser()
        {
            List<Album> albums = new List<Album>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_getAlbums", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Album album = new Album();

                    if (!reader.IsDBNull(reader.GetOrdinal("IdAlbum")))
                    {
                        album.IdAlbum = reader.GetInt32(reader.GetOrdinal("IdAlbum"));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("NameAlbum")))
                    {
                        album.NameAlbum = reader.GetString(reader.GetOrdinal("NameAlbum"));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("MusicBand")))
                    {
                        album.MusicBand = reader.GetString(reader.GetOrdinal("MusicBand"));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("ReleaseYear")))
                    {
                        album.ReleaseYear = reader.GetString(reader.GetOrdinal("ReleaseYear"));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("ImageAlbum")))
                    {
                        album.ImageAlbum = reader.GetString(reader.GetOrdinal("ImageAlbum"));
                    }


                    albums.Add(album);
                }

                cn.Close();
            }

            return View(albums);
        }

        public IActionResult SongsView(int idAlbum, string nameAlbum) 
        {
            List<Song> songs = new List<Song>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetSongsByAlbumId", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAlbum", idAlbum);

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Song song = new Song();

                    if (!reader.IsDBNull(reader.GetOrdinal("NameAlbum")))
                    {
                        song.NameAlbum = reader.GetString(reader.GetOrdinal("NameAlbum"));
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("NameSong")))
                    {
                        song.NameSong = reader.GetString(reader.GetOrdinal("NameSong"));
                    }
                    songs.Add(song);
                }

                cn.Close();
            }

            return View(songs);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
