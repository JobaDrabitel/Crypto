
using FP.Core.Project.Extentions;

var builder = WebApplication.CreateBuilder(args);


builder.AddServices(); // Схуяли? Срояли

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseCors(options => options
	.WithOrigins("http://localhost:3001", "http://localhost:3002", 
		"http://localhost:3000", "http://localhost:8080", 
		"http://localhost:4200", "http://localhost:5173")
	.AllowAnyHeader()
	.AllowAnyMethod()
	.AllowCredentials());

app.MapControllers();
await app.SetupDatabaseAsync();
app.Run();
