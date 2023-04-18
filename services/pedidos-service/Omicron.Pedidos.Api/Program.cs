// <summary>
// <copyright file="Program.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

const string APP_NAME = "pedidos-svc";

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args).AddPlaceholderResolver();
    Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.Seq(builder.Configuration["SeqUrl"])
                .CreateLogger();
    builder.AppConfiguration()
        .UseApplication()
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"{APP_NAME} service failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
