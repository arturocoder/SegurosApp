using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoSegurosFpDaw.BLL
{
    public static class Encriptacion
    {
        /// <summary>
        /// Método de Encriptación SHA256 
        /// <para>Basado en:https://hdeleon.net/funcion-para-encriptar-en-sha256-en-c-net/ </para> 
        /// </summary>
        /// <param name="password">password</param>
        /// <returns> string encriptado SHA256 </returns>
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