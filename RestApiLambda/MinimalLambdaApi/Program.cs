var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    message = "Welcome to running ASP.NET Core Minimal API on AWS Lambda"
}));

app.Run();