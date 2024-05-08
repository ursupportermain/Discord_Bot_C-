
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Core.Commands;
using Discord.Core.Service;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddKeyPerFile("/run/secrets", true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options => options.AddDefaultPolicy(builder =>
{
    builder.AllowAnyOrigin();
    builder.WithMethods(HttpMethod.Get.Method, HttpMethod.Post.Method, HttpMethod.Put.Method, HttpMethod.Delete.Method, HttpMethod.Options.Method);
    builder.WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, HeaderNames.AcceptLanguage, "x-api-key", "x-tenant", "x-client-version");
}));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var discordSocketConfig = new DiscordSocketConfig
{
    GatewayIntents = Discord.GatewayIntents.All
};
var interactionServiceConfig = new InteractionServiceConfig
{

};
builder.Services.AddSingleton(discordSocketConfig);
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(interactionServiceConfig);
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordService>();
builder.Services.AddSingleton<ExampleService>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddScoped<exampleCommand>();
builder.Services.AddScoped<VersionCommand>();
builder.Services.AddScoped<VersionService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.MapControllers();
app.Run();
