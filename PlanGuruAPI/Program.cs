using Application;
using Application.Common.Interface.Persistence;
using AutoMapper;
using Domain.Entities;
using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Utilities;
using Infrastructure;
using Infrastructure.Persistence.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PlanGuruAPI.Controllers;
using PlanGuruAPI.GraphQL.Mutations;
using PlanGuruAPI.GraphQL.Queries;
using PlanGuruAPI.GraphQL.Schemas;
using PlanGuruAPI.GraphQL.Types;
using PlanGuruAPI.Hubs;
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

            builder.Services.AddGraphQL().AddSystemTextJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.MapType<TargetType>(() => new OpenApiSchema
                {
                    Type = "integer",
                    Enum = Enum.GetValues(typeof(TargetType)).Cast<int>().Select(e => (IOpenApiAny)new OpenApiInteger(e)).ToList(),
                    Description = "Type of target. Values: 0 = Post, 1 = Comment, 2 = Wiki"
                });

                c.EnableAnnotations();
            });
            builder.Services.AddApplication().AddInfrastructure();
            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(typeof(Program));

            // Add GraphQL services
            builder.Services.AddScoped<WikiType>();
            builder.Services.AddScoped<ProductType>();
            builder.Services.AddScoped<ContentSectionType>();
            builder.Services.AddScoped<WikiQuery>();
            builder.Services.AddScoped<WikiMutation>();
            builder.Services.AddScoped<WikiSchema>();
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            builder.Services.AddMemoryCache(options =>
            {
                options.SizeLimit = 120; // Giới hạn cache tối đa là 120 mục
                //options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // Tần suất quét để xóa cache hết hạn
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://192.168.1.44:3000") // Allow this origin
                          .AllowAnyMethod()                    // Allow all HTTP methods
                          .AllowAnyHeader()                    // Allow all headers
                          .AllowCredentials();

                    policy.WithOrigins("http://localhost:3000") // Allow this origin
                          .AllowAnyMethod()                    // Allow all HTTP methods
                          .AllowAnyHeader()                    // Allow all headers
                          .AllowCredentials();          // Allow cookies/auth tokens
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapse mặc định
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            };

            app.UseHttpsRedirection();

            // Ensure CORS is applied before any endpoints that require it
            app.UseCors("AllowSpecificOrigin");

            app.UseAuthorization();

            // Seed data if necessary
            app.seedData();

            // GraphQL middleware
            app.UseGraphQL<WikiSchema>();
            app.UseGraphQLGraphiQL("/ui/graphql");

            // Map controllers and SignalR hubs
            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub").RequireCors("AllowSpecificOrigin");

            app.MapUserEndpoints();

            app.MapPostEndpoints();

            app.Run();
        }
    }
}
