//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast")
//.WithOpenApi();

//app.Run();

//internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddKeyedSingleton<INotificationService, SmsNotificationService>("sms");
builder.Services.AddKeyedSingleton<INotificationService, EmailNotificationService>("email");
builder.Services.AddKeyedSingleton<INotificationService, PushNotificationService>("push");
builder.Services.AddSingleton<SmsWrapper>();
builder.Services.AddSingleton<EmailWrapper>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/sms", (SmsWrapper notifier) => notifier.Notify("Hello world"));
app.MapGet("/email", (EmailWrapper notifier) => notifier.Notify("Hello world"));

app.Run();


// Uses the key "sms" to select the SmsNotificationService specifically
public class SmsWrapper([FromKeyedServices("sms")] INotificationService sms)
{
    public string Notify(string message) => sms.Notify(message);
}

// Uses the key "email" to select the EmailNotificationService specifically
public class EmailWrapper(IServiceProvider sp)
{
    public string Notify(string message) => sp.GetRequiredKeyedService<INotificationService>("email").Notify(message);
}


public interface INotificationService
{
    string Notify(string message);
}

public class SmsNotificationService : INotificationService
{
    public string Notify(string message) => $"[SMS] {message}";
}

public class EmailNotificationService : INotificationService
{
    public string Notify(string message) => $"[Email] {message}";
}

public class PushNotificationService : INotificationService
{
    public string Notify(string message) => $"[Push] {message}";
}