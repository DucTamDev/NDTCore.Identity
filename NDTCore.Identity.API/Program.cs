using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NDTCore.Identity.API.Configuration;
using NDTCore.Identity.API.Filters;
using NDTCore.Identity.API.HealthChecks;
using NDTCore.Identity.API.Middleware.Extensions;
using NDTCore.Identity.Application;
using NDTCore.Identity.Contracts.Configuration;
using NDTCore.Identity.Infrastructure;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting NDTCore Identity API");

    builder.Services.AddMemoryCache();

    // Add services to the container
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // JWT Configuration
    var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
        ?? throw new InvalidOperationException("JWT settings not configured");

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
    builder.Services.Configure<TokenValidationSettings>(builder.Configuration.GetSection(TokenValidationSettings.SectionName));
    builder.Services.Configure<SeedSettings>(builder.Configuration.GetSection(SeedSettings.SectionName));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("Authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("Token validated for user: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

    // Authorization Policies (RBAC)
    builder.Services.AddAuthorizationPolicies(builder.Configuration);

    // Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    // CORS
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
        ?? new[] { "*" };

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DefaultCorsPolicy", policy =>
        {
            if (allowedOrigins.Contains("*"))
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
        });
    });

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck<DatabaseHealthCheck>("database")
        .AddCheck<IdentityHealthCheck>("identity");

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();

    // Swagger Configuration
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "NDTCore Identity API",
            Version = "v1",
            Description = "Production-ready Identity and Authentication Service using Clean Architecture",
            Contact = new OpenApiContact
            {
                Name = "NDTCore Team",
                Email = "support@ndtcore.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your token"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                Array.Empty<string>()
            }
        });

        // Include XML comments if available
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    var app = builder.Build();

    // Seed static permissions into database
    await app.Services.InitializeIdentityDatabaseAsync();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "NDTCore Identity API v1");
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });
    }

    // Middleware pipeline
    app.UseGlobalExceptionHandling();
    app.UseRequestLogging();

    app.UseHttpsRedirection();

    app.UseCors("DefaultCorsPolicy");

    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    // Health checks endpoint
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds
                }),
                timestamp = DateTime.UtcNow
            });
            await context.Response.WriteAsync(result);
        }
    });

    app.MapControllers();

    Log.Information("NDTCore Identity API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}