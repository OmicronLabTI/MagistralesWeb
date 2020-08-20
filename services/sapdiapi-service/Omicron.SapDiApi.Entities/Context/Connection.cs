// <summary>
// <copyright file="Connection.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Entities.Context
{
    using SAPbobsCOM;
    using System.Configuration;

    /// <summary>
    /// The connetion to Sap
    /// </summary>
    public class Connection
    {
        private static Company company = null;

        public static Company Company 
        {
            get
            {
                if(company == null)
                {
                    company = Connect();
                }

                return company;
            } 
        }

        public Connection()
        {
        }

        /// <summary>
        /// The connection.
        /// </summary>
        public static Company Connect()
        {
            Company oCompany = new Company
            {
                Server = ConfigurationManager.AppSettings["SapServer"],
                DbServerType = BoDataServerTypes.dst_MSSQL2016,
                CompanyDB = ConfigurationManager.AppSettings["SapDb"],
                language = BoSuppLangs.ln_Spanish_La,
                UserName = ConfigurationManager.AppSettings["Usuario"],
                Password = ConfigurationManager.AppSettings["UserPwd"],
                SLDServer = ConfigurationManager.AppSettings["SldServer"],
            };

            oCompany.Connect();

            var connected = oCompany.Connected;
            if (!connected)
            {
                oCompany.Connect();
            }


            return oCompany;
        }
    }
}
