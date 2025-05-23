using Microsoft.Data.SqlClient;
using System.Data;

namespace WS_2_0.Models.Logueo
{
    public class LogInAdministrador
    {
        public static Usuario Entrar(Usuario usu, string StringdeConexion)
        {
            using (SqlConnection conn = new SqlConnection(StringdeConexion))
            {
                SqlCommand cmd = new SqlCommand("admVal", conn);
                cmd.Parameters.AddWithValue("@Correo", usu.Correo);
                cmd.Parameters.AddWithValue("@Contraseña", usu.Contraseña);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.ExecuteScalar();
                usu.id_adm = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }
            return usu;
        }
    }
}