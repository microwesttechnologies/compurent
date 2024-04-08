using Microsoft.AspNetCore.Mvc;
using Project_MusicRadio.Models;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;



namespace Project_MusicRadio.Controllers
{
    public class AccesController : Controller
    {
        static string connectionString = "Data Source=(localdb)\\microwest; Initial Catalog=MUSICRADIODB;Integrated Security=true";
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            bool registerSuccesfull;
            string? message;

            if(user.PassUser == user.PassComfirm)
            {
                user.PassUser = ConvertirSha256(user.PassUser);
            }
            else
            {
                ViewData["Message"] = "Las contraseñas no coinciden";
                return View();
            } 


            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_registerUser",  cn);
                cmd.Parameters.AddWithValue("NameUser",             user.NameUser);
                cmd.Parameters.AddWithValue("Mail",                 user.Mail);
                cmd.Parameters.AddWithValue("Adders",               user.Adders);
                cmd.Parameters.AddWithValue("Phone",                user.Phone);
                cmd.Parameters.AddWithValue("PassUser",             user.PassUser);
                cmd.Parameters.AddWithValue("Rol",                  user.Rol);

                cmd.Parameters.Add("RegisterSuccesfull",    SqlDbType.Bit).Direction            = ParameterDirection.Output;
                cmd.Parameters.Add("Message",               SqlDbType.VarChar,100).Direction    = ParameterDirection.Output;

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registerSuccesfull = Convert.ToBoolean(cmd.Parameters["RegisterSuccesfull"].Value);
                message = cmd.Parameters["Message"].Value.ToString();

                cn.Close();

            }

            ViewData["Message"] = message;

            if(registerSuccesfull)
            {
              return  RedirectToAction("Login", "Acces");
            } else
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            user.PassUser = ConvertirSha256(user.PassUser);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_validateUser", cn);
               
                cmd.Parameters.AddWithValue("Mail",     user.Mail);
                cmd.Parameters.AddWithValue("PassUser", user.PassUser);

                cmd.CommandType = CommandType.StoredProcedure;
                
                cn.Open();

                user.IdUser = Convert.ToInt32( cmd.ExecuteScalar().ToString());

                cn.Close();
            } 

            if (user.IdUser != 0)
            {
                ViewData["IsAuthenticated"] = true;
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(user));
                return RedirectToAction("Dashboard", "Home");
            }

            else
            {
                ViewData["Message"] = "Usuario no existe!!!";
                return View();
            }
        }

        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
    }
}


