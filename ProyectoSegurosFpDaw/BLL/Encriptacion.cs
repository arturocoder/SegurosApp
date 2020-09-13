using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoSegurosFpDaw.BLL
{
    public static class Encriptacion
    {   
        
        public static string GetSHA256(string password)
        {
            using (SHA256 sha256 = SHA256Managed.Create())
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] stream = null;
                StringBuilder sb = new StringBuilder();
                stream = sha256.ComputeHash(encoding.GetBytes(password));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat(CultureInfo.GetCultureInfo("es-ES"), "{0:x2}", stream[i]);
                return sb.ToString();
            }
        }
    }
}