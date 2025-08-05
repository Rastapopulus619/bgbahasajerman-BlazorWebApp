using bgbahasajerman_BlazorWebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs for different environments
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    // In Docker container - bind to all interfaces
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Dynamically set API base URL
string apiBaseUrl;
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    Console.WriteLine("Running in Docker");
    // Running in Docker: use internal Docker network address
    apiBaseUrl = "http://dataaccessapi:8090/"; // Replace with your actual service name and port
}
else
{
    Console.WriteLine("Running locally from Visual Studio");
    // Local development: use Tailscale IP of Ubuntu server
    apiBaseUrl = "http://100.117.149.44:8090/"; // Tailscale IP and API port
}

// Log the API URL for debugging
Console.WriteLine($"[BLAZOR] Using API Base URL: {apiBaseUrl}");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl),
    Timeout = TimeSpan.FromSeconds(30)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
