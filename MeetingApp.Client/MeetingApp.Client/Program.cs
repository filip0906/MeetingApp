using MeetingApp.Client.Components;
using MeetingApp.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add HttpClient to interact with the API
builder.Services.AddScoped(sp =>
    new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    })
    {
        BaseAddress = new Uri("https://localhost:7126")
    });

var cookieOptions = new CookieOptions
{
    Secure = true, // Ensure cookies are sent only over HTTPS
    SameSite = SameSiteMode.None // Adjust based on your requirements
};


builder.Services.AddScoped<MeetingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
