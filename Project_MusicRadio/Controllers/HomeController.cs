using Microsoft.AspNetCore.Mvc;
using Project_MusicRadio.Models;
using Project_MusicRadio.Permissions;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;



namespace Project_MusicRadio.Controllers
{
    [ValidateSessionAtrib]
    public class HomeController : Controller
    {
        static string connectionString = "Data Source=(localdb)\\microwest; Initial Catalog=MUSICRADIODB;Integrated Security=true";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


		public IActionResult Dashboard()
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

        public IActionResult SongsView(int idAlbum)
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
                    if (!reader.IsDBNull(reader.GetOrdinal("IdSong")))
                    {
                        song.IdSong = reader.GetInt32(reader.GetOrdinal("IdSong"));
                    }

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

        public IActionResult DeleteAlbum(int idAlbum)
        {
            string message = "";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("sp_deleteAlbum", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AlbumId", idAlbum);

                    var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 1000)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(messageParameter);

                    command.ExecuteNonQuery();

                    message = messageParameter.ToString();
                }
            }

            ViewData["Message"] = message;
            return RedirectToAction("Dashboard");
        }

        public IActionResult DeleteSong(int idSong)
        {
            string message = "";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("sp_deleteSong", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdSong", idSong);

                    var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 1000)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(messageParameter);

                    command.ExecuteNonQuery();

                    message = messageParameter.ToString();
                }
            }

            ViewData["Message"] = message;
            return RedirectToAction("Dashboard");
        }

        public ActionResult LogOut()
        {
            var userJson = HttpContext.Session.GetString("user");

            if (!string.IsNullOrEmpty(userJson))
            {
                HttpContext.Session.Remove("user");
            }
            return RedirectToAction("Login", "Acces");
        }

        public IActionResult Index()
        {
            ViewData["IsAuthenticated"] = HttpContext.Session.GetString("user") != null;
            return View();
        }

        public IActionResult CreateAlbum()
        {
            return View();
        }

        public IActionResult CreateSong(int idAlbum)
        {
            ViewBag.AlbumId = idAlbum;
            return View();
        }

        [HttpPost]
        public ActionResult CreateAlbum(Album album)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_createAlbum", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@NameAlbum", album.NameAlbum);
                    cmd.Parameters.AddWithValue("@MusicBand", album.MusicBand);
                    cmd.Parameters.AddWithValue("@ReleaseYear", album.ReleaseYear);
                    cmd.Parameters.AddWithValue("@ImageAlbum", album.ImageAlbum);

                    var createAlbumSuccessParameter = new SqlParameter("@CreateAlbumSuccesfull", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(createAlbumSuccessParameter);

                    var messageParameter = new SqlParameter("@Message", SqlDbType.VarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(messageParameter);

                    cmd.ExecuteNonQuery();

                    bool createAlbumSuccess = (bool)createAlbumSuccessParameter.Value;
                    string message = messageParameter.ToString();

                    ViewBag.Message = message;
                    ViewBag.Success = createAlbumSuccess;
                }
            }

            return View();
        }


        [HttpPost]
        public ActionResult CreateSong(Song song)
        {
            bool createSongSuccess = false;
            string message = "";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("sp_createSong", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@NameSong", song.NameSong);
                    command.Parameters.AddWithValue("@AlbumId", song.AlbumId);

                    var createSongSuccessParameter = new SqlParameter("@CreateSongSuccesfull", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(createSongSuccessParameter);

                    var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(messageParameter);

                    command.ExecuteNonQuery();

                    createSongSuccess = (bool)createSongSuccessParameter.Value;
                    message = messageParameter.ToString();
                }
            }

            if (createSongSuccess)
            {
                ViewBag.SuccessMessage = message;
            }
            else
            {
                ViewBag.ErrorMessage = message;
            }

            return RedirectToAction("Dashboard", "Home");
        }

    

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
