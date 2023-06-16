using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sap.Data.Hana;


namespace ServicioZK
{
    internal class Conex
    {
        public static void conexion()
        {
            string connstr = "Server = 192.168.20.1:30015; UserName = SYSTEM; Password = Admin123; CS = FG_PROD";
            using (HanaConnection conn = new HanaConnection(connstr))
            {
                conn.Open();
                Archivo._hana=conn;
            }

        }
    }
}
