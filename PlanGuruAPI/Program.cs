using Application;
using AutoMapper;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using PlanGuruAPI.Mapping;
using Serilog;

namespace PlanGuruAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddApplication().AddInfrastructure();

            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseExceptionHandler("/error");

            app.seedData();

            app.MapControllers();

            app.Run();
        }
    }
}
