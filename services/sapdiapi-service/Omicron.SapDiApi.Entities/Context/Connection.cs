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
                Server = "OMICRON-PQRNL0E", 
                DbServerType = BoDataServerTypes.dst_MSSQL2016,
                CompanyDB = "Omicron_ProduccionTest",
                language = BoSuppLangs.ln_Spanish_La,
                UserName = "manager",
                Password = "0m1cr0nl4b",
                SLDServer = "OMICRON-PQRNL0E:40000",
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
