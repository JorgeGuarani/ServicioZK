using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAPbobsCOM;


namespace ServicioZK
{
    internal class Conex
    {
        public static SAPbobsCOM.Company _sbo;
        public static void conexion()
        {
            //string connstr = "Server = 192.168.20.1:30015; UserName = SYSTEM; Password = Admin123; CS = FG_PROD";
            //using (HanaConnection conn = new HanaConnection(connstr))
            //{
            //    conn.Open();
            //    Archivo._hana=conn;
            //}
            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            oCompany.Server = "saphana:30015";
            oCompany.DbServerType = BoDataServerTypes.dst_HANADB;
            oCompany.UseTrusted = false;
            oCompany.DbUserName = "SYSTEM";
            oCompany.DbPassword = "Admin123";
            oCompany.CompanyDB = "FG_DESA";
            oCompany.UserName = "manager";
            oCompany.Password = "54321";
            oCompany.language = BoSuppLangs.ln_Spanish_La;
            //oCompany.LicenseServer = "192.168.20.1:30015";
            int conexion = oCompany.Connect();
            _sbo = oCompany;

        }
    }
}
