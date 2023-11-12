using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfigurationController()
    .AddUseCases()
    .AddConnectionsDI(builder.Configuration);

var app = builder.Build();

app.UseCors("*");
app.UseDocumentation();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }