namespace DeleteFiles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;

    public partial class Service1 : ServiceBase
    {

        private static Logger Log = LogManager.GetCurrentClassLogger();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("ServiceRunning");
            var baseRoute = @"C:\Pruebas_ArchivosOmicronTemp\SaleOrders";
            var baseRouteProd = @"C:\ArchivosOmicronTemp\SaleOrders";

            var routes = new List<string>();
            routes.Add(baseRoute);
            routes.Add(baseRouteProd);

            routes.ForEach(r =>
            {
                var files = Directory.GetFiles(r);

                foreach (var f in files)
                {
                    try
                    {
                        File.Delete(f);
                        Log.Info($"File {f} deleted");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error al borrar archivo {ex.Message} -- {ex.StackTrace}");
                    }
                }
            });
        }

        protected override void OnStop()
        {
            Log.Info("ServiceStopped");
        }
    }
}
