using ClinicLogic.Models;
using ClinicLogic.Managers;
using Services.GiftServices.Managers;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
//
EnsureDataFilesExist();

//logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configure AppConfig
builder.Services.Configure<AppConfig>(
        builder.Configuration.GetSection("AppConfig"));
//Singleton for Controller Constructor
builder.Services.AddTransient<PatientManager>();
builder.Services.AddTransient<GiftManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseAuthorization();

app.MapControllers();

app.Run();


void EnsureDataFilesExist()
{
    string dataDir = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "data");

    if (!Directory.Exists(dataDir))
        Directory.CreateDirectory(dataDir);

    string[] requiredFiles = { "patients.txt", "users.txt" };

    foreach (var file in requiredFiles)
    {
        string path = Path.Combine(dataDir, file);
        if (!File.Exists(path))
        {
            File.Create(path).Dispose(); // Create and close
            Log.Information($"Created missing file: {path}");
        }
    }
}