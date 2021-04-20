// <summary>
// <copyright file="ExcelBuilder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.ReportBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using ClosedXML.Excel;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Constants;

    /// <summary>
    /// Class to create Excel reports.
    /// </summary>
    public class ExcelBuilder
    {
        /// <summary>
        /// Creates the excel.
        /// </summary>
        /// <param name="incidents">the incidets.</param>
        /// <returns>the data.</returns>
        public (MemoryStream, string) CreateIncidentExcel(List<IncidentDataModel> incidents)
        {
            var fileName = $"{ServiceConstants.IncidentFileName}{DateTime.Today.Day}{DateTime.Today.Month}{DateTime.Today.Year}.xlsx";
            var dataTable = this.CreateIncidentDataTable(incidents, ServiceConstants.IncidentKeys);

            var mss = new MemoryStream();
            var wb = new XLWorkbook();
            wb.Worksheets.Add(dataTable);
            wb.SaveAs(mss);
            mss.Position = 0;
            return (mss, fileName);
        }

        /// <summary>
        /// Creates the datatable for incidents.
        /// </summary>
        /// <param name="incidents">the incidents.</param>
        /// <param name="columns">the column.</param>
        /// <returns>the data.</returns>
        private DataTable CreateIncidentDataTable(List<IncidentDataModel> incidents, Dictionary<string, string> columns)
        {
            var dt = new DataTable();
            dt.TableName = ServiceConstants.IncidentDataTableName;
            columns.Keys.ToList().ForEach(x => dt.Columns.Add(x, typeof(string)));

            incidents.ForEach(i =>
            {
                var listValues = new List<string>();
                foreach (var k in columns.Keys.ToList())
                {
                    var dictValue = columns[k];
                    var property = i.GetType().GetProperty(dictValue);
                    var propertyValue = property.GetValue(i, null).ToString();
                    listValues.Add(propertyValue);
                }

                dt.Rows.Add(listValues.ToArray());
            });

            return dt;
        }
    }
}
