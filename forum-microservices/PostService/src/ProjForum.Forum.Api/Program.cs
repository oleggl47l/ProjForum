using System.Reflection;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ProjForum.Forum.Api.ExceptionHandlers;
using ProjForum.Forum.Application.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjForum.Forum.Api.Options;
using ProjForum.Forum.Application.Forum.Commands.Categories;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Infrastructure.Data;
using ProjForum.Forum.Infrastructure.Data.Repositories;
using ProjForum.Forum.Infrastructure.Messaging.Consumers;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("PFForum")));

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddMediatR(cfg =>
{
    cfg.Lifetime = ServiceLifetime.Scoped;
    cfg.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).GetTypeInfo().Assembly);
});

builder.Services.AddApplication();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();

var swaggerOptions = configuration.GetSection("SwaggerDocOptions").Get<SwaggerDocOptions>()
                     ?? throw new InvalidOperationException(
                         "SwaggerDocOptions section is missing in appsettings.json");

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc(swaggerOptions.Name, new OpenApiInfo
    {
        Version = swaggerOptions.Version,
        Title = swaggerOptions.Title,
        Description = swaggerOptions.Description
    });

    foreach (var server in swaggerOptions.Servers)
    {
        opt.AddServer(new OpenApiServer
        {
            Url = server.Url,
            Description = server.Description
        });
    }

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });

    opt.EnableAnnotations();

    Action<SwaggerGenOptions>? configure = null;

    configure?.Invoke(opt);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidIssuer = jwtSettings["Issuer"],
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException()))
        };
    });

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });

    x.AddConsumer<UserStatusChangedEventConsumer>();
    x.AddConsumer<UserDeletedConsumer>();
});

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint($"/swagger/{swaggerOptions.Name}/swagger.json",
            $"{swaggerOptions.Title} {swaggerOptions.Version}");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();