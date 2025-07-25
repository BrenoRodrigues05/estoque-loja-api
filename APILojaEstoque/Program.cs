using APILojaEstoque.Context;
using APILojaEstoque.DTOs.Mappings;
using APILojaEstoque.Extensions;
using APILojaEstoque.Filters;
using APILojaEstoque.Logging;
using APILojaEstoque.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mysqlconnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<APILojaEstoqueContext> (options=> options.UseMySql(mysqlconnection, ServerVersion.
    AutoDetect(mysqlconnection)));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<APILojaEstoque.Filters.APILoggingFilter>();
});

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLogggerProviderConfiguration{
    LogLevel = LogLevel.Information
}));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), (typeof (GenericRepository<>)));
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
