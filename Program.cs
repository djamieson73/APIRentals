using Serilog;
using System.Runtime;
using API_Rentals.Services;
using API_Rentals;
using Entities;
using Microsoft.EntityFrameworkCore;
using API_Rentals.Profile;
//using AutoMapper;
using Entities.Models;
using API_Rentals.Models;
using ExpressMapper;

//test

Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Debug()
  .WriteTo.Console()
  .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
	//rollinginterval = a new log file is created every day
	//the file is appended with current date, so the filename would be cityinfo20230114.txt
  .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EntitiesDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Host.UseSerilog();

Mapper.Register<City, CityWithoutPointsOfInterestDto>();
Mapper.Register<CityWithoutPointsOfInterestDto, City>();

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddControllers();

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
