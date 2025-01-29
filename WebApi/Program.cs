using Application.Services;
using Application.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Precisa ter a senha do sql server definida nas variáveis de ambiente 
var password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD");
string sqlConnection = builder.Configuration.GetConnectionString("SqlServerConnection");

string connectionString = "";

if (!string.IsNullOrEmpty(password))
{
    connectionString = sqlConnection + password + ";";
}
else
{
    connectionString = sqlConnection;
}

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

//Precisa ter a chave de autenticação definida nas variáveis de ambiente 
var authenticationKey = Environment.GetEnvironmentVariable("AUTH_KEY");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
    };

    if (!string.IsNullOrEmpty(authenticationKey))
    {
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationKey));
    }
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// Register services
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserEventService, UserEventService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

builder.Services.AddAutoMapper(typeof(Program));  // AutoMapper configuration
builder.Services.AddAutoMapper(typeof(MappingProfileService));

builder.Services.AddControllers();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy.WithOrigins("http://localhost:8080")  // Frontend URL
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        options.RoutePrefix = string.Empty; 
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowLocalhost"); 

app.UseRouting();
app.UseStaticFiles();

app.UseMiddleware<UnauthorizedResponseMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

