using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.repositories;
using dotnet_8_backend_template.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

SetupDependencyInjection();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

void SetupDependencyInjection()
{
    builder.Services.AddTransient<IIpService, IpService>();
    
    builder.Services.AddHttpClient<IIpRepository,IpRepository>(client => client.BaseAddress = new Uri(builder.Configuration["IP_API_URL"]!));
}
