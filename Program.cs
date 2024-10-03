using Ex3.API;
using Ex3.API.MiddleWares;

var builder = WebApplication.CreateBuilder(args);

// Adding services to container
builder.Services.AddSingleton<List<Product>>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RateLimiterMiddleWare>();

app.MapControllers();
app.Run();