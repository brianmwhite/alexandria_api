using System.Text.Json.Serialization;
using alexandria.api.Helpers;
using alexandria.api.Repositories;
using alexandria.api.Services;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.ListenAnyIP(5384); });
// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddSignalR();

    services.AddSingleton<BookDataContext>();
    services.AddSingleton<AppDataContext>();

    services.AddCors();
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // ignore omitted parameters on models to enable optional params (e.g. User update)
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // configure DI for application services
    services.AddScoped<IBookRepository, BookRepository>();
    services.AddScoped<IBookService, BookService>();
    services.AddScoped<IFileService, FileService>();
    services.AddScoped<IKnownDeviceService, KnownDeviceService>();
    services.AddScoped<IDeviceTypeService, DeviceTypeService>();
}

var app = builder.Build();

// ensure database and tables exist
{
    using var scope = app.Services.CreateScope();
    var bookDataContext = scope.ServiceProvider.GetRequiredService<BookDataContext>();
    var appDataContext = scope.ServiceProvider.GetRequiredService<AppDataContext>();
    // await context.Init();
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.MapHub<MessageHub>("/hubs/devices");

    app.MapControllers();
}

app.Run();