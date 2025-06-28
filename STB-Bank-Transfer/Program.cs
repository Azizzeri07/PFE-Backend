using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure Middleware Pipeline
ConfigureMiddleware(app);

// Seed Initial Admin User
await SeedInitialAdmin(app.Services);

app.Run();

// Service Configuration Method
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Core Services
    services.AddControllers();
    services.AddRazorPages();

    // Database Configuration
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    // JWT Authentication
    ConfigureJwtAuthentication(services, configuration);

    // Authorization Policies
    services.AddAuthorization(options =>
    {
        options.AddPolicy("ClientOnly", policy => policy.RequireRole(UserRoles.Client));
        options.AddPolicy("BanquierOnly", policy => policy.RequireRole(UserRoles.Banquier));
    });

    // Swagger Configuration
    ConfigureSwagger(services);

    // Application Services
    services.AddSingleton<JwtService>();
}

// JWT Configuration Method
static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
{
    var jwtSettings = configuration.GetSection("Jwt");
    var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
}

// Swagger Configuration Method
static void ConfigureSwagger(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "STB Bank API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    });
}

// Middleware Configuration Method
static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapRazorPages();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

// Admin Seeding Method
static async Task SeedInitialAdmin(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!await dbContext.Banquiers.AnyAsync(b => b.Role == UserRoles.Admin))
    {
        var admin = new Banquier
        {
            Nom = "Admin",
            Email = "admin@stb.com",
            MotDePasse = "Admin@123",
            Role = UserRoles.Admin
        };

        admin.SetPassword(admin.MotDePasse);
        dbContext.Banquiers.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}