using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Oracle.DataAccess.Client;
using System.Web.Configuration;

namespace WhareHouse.Controllers
{
    public class Connection
    {
        private OracleConnection Cn { get; set; }
        public OracleConnection GetConection()
        {

            if (Cn == null)
            {
                string conection = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionDb"].ToString();
                Cn = new OracleConnection(conection);
            }
            return Cn;
        }
    }
}