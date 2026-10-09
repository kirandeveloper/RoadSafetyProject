using RoadSafetyProject.Data;
using RoadSafetyProject.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddScoped<RspMasterRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("OracleDb");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Connection string 'OracleDb' was not found in appsettings.json.");
    }

    return new RspMasterRepository(connectionString);
});

builder.Services.AddScoped<MonthlyLcStatusRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();