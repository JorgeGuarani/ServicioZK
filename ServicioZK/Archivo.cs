﻿using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


namespace ServicioZK
{
    partial class Archivo : ServiceBase
    {
        bool bandera = false;
        // public static SAPbobsCOM.Company oSBO = null;
        public static HanaConnection _hana = null;
        public Archivo()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            stLapso.Start();
            
        }

        protected override void OnStop()
        {
            stLapso.Stop();

        }

        private void stLapso_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (bandera == true) return;

            //agarramos la hora actual
            int hora = DateTime.Now.Hour;
            int v_horaSAP = 0;
            Conex.conexion();
            //if (_hana.State.ToString() != "Open")
            //{
            //    _hana.Open();
            //}
            //consultamos la hora configurada en SAP
            SAPbobsCOM.Recordset oHoraSap;
            oHoraSap = (SAPbobsCOM.Recordset)Conex._sbo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            oHoraSap.DoQuery("SELECT \"U_Valor\" FROM \"@APP_CONFIG\" WHERE \"Name\"='ServicioZK' ");
            v_horaSAP = int.Parse(oHoraSap.Fields.Item(0).Value.ToString());

            //IDbCommand dbCommandZK = _hana.CreateCommand();
            //dbCommandZK.CommandText = "SELECT \"U_Valor\" FROM \"@APP_CONFIG\" WHERE \"Name\"='ServicioZK' ";
            //IDataReader dataReaderZK = dbCommandZK.ExecuteReader();
            //while(dataReaderZK.Read()) 
            //{
            //    v_horaSAP = dataReaderZK.GetInt32(0);
            //}

            try
            {
                bandera = true;
                if (v_horaSAP == hora)
                {
                    Conex.conexion();
                    //instanciamos el servicio
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                    //BasicHttpBinding binding = new BasicHttpBinding();
                    //binding.Security.Mode = BasicHttpSecurityMode.Transport;

                    marcacion_1.ZhcmMarcacionRequest request = new marcacion_1.ZhcmMarcacionRequest();
                    marcacion_1.ZhcmMarcacionResponse response = new marcacion_1.ZhcmMarcacionResponse();
                    marcacion_1.ZhcmMarcacion parametros = new marcacion_1.ZhcmMarcacion();
                    marcacion_1.E1hrkk1teup campo = new marcacion_1.E1hrkk1teup();
                    marcacion_1.ZHCM_WS_MARCACION_EMPClient servMarcacion = new marcacion_1.ZHCM_WS_MARCACION_EMPClient();
                    //mandamos las credenciales
                    servMarcacion.ClientCredentials.UserName.UserName = "S_WSOCHOA";
                    servMarcacion.ClientCredentials.UserName.Password = "BcM%brbLP%YA";


                    //EventLog.WriteEntry("Proceso de migración de marcaciones", EventLogEntryType.Information);

                    //comprobamos si esta conectado a HANA
                    //if (_hana.State.ToString() != "Open")
                    //{
                    //    _hana.Open();
                    //}
                    //distinguimos los distintos codigos de enrolamientos
                    SAPbobsCOM.Recordset oCodEnro;
                    oCodEnro = (SAPbobsCOM.Recordset)Conex._sbo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    oCodEnro.DoQuery("SELECT DISTINCT \"U_COD_ENROLAC\" FROM \"@RELOJ_ZK\"");
                    while (!oCodEnro.EoF)
                    {
                        //agarramos el codigo de enrolamiento en una variable
                        //int v_codEnro = int.Parse(oCodEnrolac.Fields.Item(0).Value.ToString());
                        int v_codEnro = int.Parse(oCodEnro.Fields.Item(0).Value.ToString());
                        //consultamos la ultima marcación
                        SAPbobsCOM.Recordset oMaxEntry;
                        oMaxEntry = (SAPbobsCOM.Recordset)Conex._sbo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oMaxEntry.DoQuery("SELECT MAX(\"DocEntry\") FROM \"@RELOJ_ZK\" WHERE \"U_COD_ENROLAC\"=" + v_codEnro + "");
                        string v_UltMarc = oMaxEntry.Fields.Item(0).Value.ToString();

                        //string v_UltMarc = null;
                        //IDbCommand dbCommand2 = _hana.CreateCommand();
                        //dbCommand2.CommandText = "SELECT MAX(\"DocEntry\") FROM \"@RELOJ_ZK\" WHERE \"U_COD_ENROLAC\"=" + v_codEnro + "";
                        //IDataReader dataReader2 = dbCommand2.ExecuteReader();
                        //while (dataReader2.Read())
                        //{
                        //    v_UltMarc = dataReader2.GetString(0);
                        //}
                        //buscamos sus datos
                        String v_fechaHora = null;
                        int v_espacio = 0;
                        int v_total = 0;
                        string v_fecha = null;
                        DateTime darformaFecha;
                        string _fechaFormato = null;
                        string v_hora = null;
                        string v_fechaYhora = null;
                        string fechaprueba = null;
                        SAPbobsCOM.Recordset oDatos;
                        oDatos = (SAPbobsCOM.Recordset)Conex._sbo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oDatos.DoQuery("SELECT \"U_COD_ENROLAC\",\"U_FECHA_HORA\" FROM \"@RELOJ_ZK\" WHERE \"DocEntry\"='" + v_UltMarc + "'");
                        while (!oDatos.EoF)
                        {
                            v_fechaHora = oDatos.Fields.Item(0).Value.ToString();
                            v_espacio = v_fechaHora.IndexOf(" ");
                            v_total = v_fechaHora.Length;
                            v_fecha = v_fechaHora.Remove(v_espacio, v_total - v_espacio);//.Replace("/", string.Empty);
                            fechaprueba = v_fechaHora.Remove(v_espacio, v_total - v_espacio).Replace("/", string.Empty);
                            darformaFecha = DateTime.Parse(v_fecha);
                            _fechaFormato = darformaFecha.ToString("yyyyMMdd");
                            v_hora = v_fechaHora.Remove(0, v_espacio + 1).Replace(":", string.Empty);
                            v_fechaYhora = _fechaFormato + v_hora;
                        }

                        //IDbCommand dbCommand3 = _hana.CreateCommand();
                        //dbCommand3.CommandText = "SELECT \"U_COD_ENROLAC\",\"U_FECHA_HORA\" FROM \"@RELOJ_ZK\" WHERE \"DocEntry\"='" + v_UltMarc + "'";
                        //IDataReader dataReader3 = dbCommand3.ExecuteReader();
                        //string fechaprueba = null;
                        //while (dataReader3.Read())
                        //{
                        //    v_fechaHora = dataReader3.GetString(1);
                        //    v_espacio = v_fechaHora.IndexOf(" ");
                        //    v_total = v_fechaHora.Length;
                        //    v_fecha = v_fechaHora.Remove(v_espacio, v_total - v_espacio);//.Replace("/", string.Empty);
                        //    fechaprueba = v_fechaHora.Remove(v_espacio, v_total - v_espacio).Replace("/", string.Empty);
                        //    darformaFecha = DateTime.Parse(v_fecha);
                        //    _fechaFormato = darformaFecha.ToString("yyyyMMdd");
                        //    v_hora = v_fechaHora.Remove(0, v_espacio + 1).Replace(":", string.Empty);
                        //    v_fechaYhora = _fechaFormato + v_hora;
                        //}
                        //consumimos el WS de Chornos
                        //mandamos los datos al WS
                        campo.Satza = "P01";
                        campo.Terid = "71";
                        campo.Ldate = _fechaFormato;
                        campo.Ltime = v_hora;
                        campo.Erdat = _fechaFormato;
                        campo.Ertim = v_hora;
                        campo.Zausw = v_codEnro.ToString();
                        parametros.IMarcacion = campo;
                        request.ZhcmMarcacion = parametros;
                        response = servMarcacion.ZhcmMarcacion(request.ZhcmMarcacion);
                        //guardamos la respuesta
                        dynamic R = response.EStatus;
                        string resp = response.EResultado.ToString();

                        //insertamos en hana
                        //consultamos el ultimo docentry y le sumamos uno
                        SAPbobsCOM.Recordset oMaxMarc;
                        oMaxMarc = (SAPbobsCOM.Recordset)Conex._sbo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oMaxMarc.DoQuery("SELECT MAX(\"DocEntry\")+1 FROM \"@RELOJ_MARCACIONES\" ");
                        string NuevoDocEntry = oMaxMarc.Fields.Item(0).Value.ToString();

                        //string NuevoDocEntry = null;
                        //IDbCommand dbCommand4 = _hana.CreateCommand();
                        //dbCommand4.CommandText = "SELECT MAX(\"DocEntry\")+1 FROM \"@RELOJ_MARCACIONES\" ";
                        //IDataReader dataReader4 = dbCommand4.ExecuteReader();
                        //while (dataReader4.Read())
                        //{
                        //    NuevoDocEntry = dataReader4.GetString(0);
                        //}

                        if (R == "OK")
                        {
                            SAPbobsCOM.Recordset oInsert;
                            oInsert = (SAPbobsCOM.Recordset)Conex._sbo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oInsert.DoQuery("INSERT INTO \"@RELOJ_MARCACIONES\" (\"Code\",\"Name\",\"DocEntry\",\"U_COD_ENROLAC\",\"U_FECHA_HORA\",\"U_COD_RELOJ\",\"U_IND_ENTSAL\",\"U_FECHAREAL\",\"U_RESPUESTAWS\",\"Object\",\"CreateDate\") VALUES ('" + NuevoDocEntry + "','','" + NuevoDocEntry + "','" + v_codEnro + "','" + v_fechaYhora + "','71','S','" + v_fechaYhora + "','" + resp + "','RELOJ_MARCACIONES','" + _fechaFormato + "') ");
                            //procedemos a limpiar la tabla temporal
                            SAPbobsCOM.Recordset oDelete;
                            oDelete = (SAPbobsCOM.Recordset)Conex._sbo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oDelete.DoQuery("DELETE FROM \"@RELOJ_ZK\" WHERE \"U_COD_ENROLAC\"=" + v_codEnro);

                            //IDbCommand dbCommand5 = _hana.CreateCommand();
                            //dbCommand5.CommandText = "INSERT INTO \"@RELOJ_MARCACIONES\" (\"Code\",\"Name\",\"DocEntry\",\"U_COD_ENROLAC\",\"U_FECHA_HORA\",\"U_COD_RELOJ\",\"U_IND_ENTSAL\",\"U_FECHAREAL\",\"U_RESPUESTAWS\",\"Object\",\"CreateDate\") VALUES ('" + NuevoDocEntry + "','','" + NuevoDocEntry + "','" + v_codEnro + "','" + v_fechaYhora + "','71','S','" + v_fechaYhora + "','" + resp + "','RELOJ_MARCACIONES','" + _fechaFormato + "') ";
                            //IDataReader dataReader5 = dbCommand5.ExecuteReader();

                            //procedemos a limpiar la tabla temporal
                            //IDbCommand dbCommand6 = _hana.CreateCommand();
                            //dbCommand6.CommandText = "DELETE FROM \"@RELOJ_ZK\" WHERE \"U_COD_ENROLAC\"=" + v_codEnro;
                            //IDataReader dataReader6 = dbCommand6.ExecuteReader();
                        }
                    }

                    //IDbCommand dbCommand = _hana.CreateCommand();
                    //dbCommand.CommandText = "SELECT DISTINCT \"U_COD_ENROLAC\" FROM \"@RELOJ_ZK\"";
                    //IDataReader dataReader = dbCommand.ExecuteReader();
                    //while (dataReader.Read())
                    //{
                        



                    //}
                }
                

            }
            catch (Exception ex)
            {
                //EventLog.WriteEntry(ex.Message.ToString(),EventLogEntryType.Error);
            }

            bandera = false;
        }
    }
}
