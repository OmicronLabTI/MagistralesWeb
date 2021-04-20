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
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Constants;
    using SpreadsheetLight;

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
            var ms = new MemoryStream();
            var fileName = $"{ServiceConstants.IncidentFileName}{DateTime.Today.Day}{DateTime.Today.Month}{DateTime.Today.Year}.xlsx";
            var dataTable = this.CreateIncidentDataTable(incidents, ServiceConstants.IncidentKeys);
            using (SLDocument sl = new SLDocument())
            {
                var startRowIndex = 0;
                var startColumnIndex = 0;
                sl.ImportDataTable(startRowIndex, startColumnIndex, dataTable, true);

                int endRowIndex = startRowIndex + dataTable.Rows.Count + 1 - 1;
                int endColumnIndex = startColumnIndex + dataTable.Columns.Count - 1;
                SLTable table = sl.CreateTable(startRowIndex, startColumnIndex, endRowIndex, endColumnIndex);
                table.SetTableStyle(SLTableStyleTypeValues.Medium17);
                sl.InsertTable(table);
                sl.SaveAs(ms);
            }

            ms.Position = 0;

            return (ms, fileName);
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
