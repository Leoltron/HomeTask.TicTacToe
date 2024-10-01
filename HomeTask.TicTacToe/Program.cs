using System.Text.Json.Serialization;
using HomeTask.TicTacToe.Hubs;
using HomeTask.TicTacToe.Models;
using HomeTask.TicTacToe.Models.Games;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.ConfigureHttpJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter();
    opts.SerializerOptions.Converters.Add(enumConverter);
});
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameListManager, GameListManager>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Index}/{id?}");
app.MapHub<GameHub>("/gameHub");

app.Run();