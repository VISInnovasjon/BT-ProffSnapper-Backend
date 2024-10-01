using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Server.Context;
using Server.Controllers;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.WebHost.UseUrls("http://0.0.0.0");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
DotEnv.Load();
GlobalLanguage.Language = "nor";
builder.Services.AddDbContext<BtdbContext>(options =>
{
    options.UseNpgsql($"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Username={Environment.GetEnvironmentVariable("DATABASE_USER")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}").EnableDetailedErrors();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var localId = Environment.GetEnvironmentVariable("LOCAL_ID");
    var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
    var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
    if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(localId) || string.IsNullOrEmpty(clientId)) throw new NullReferenceException($"Couldn't fetch IDs. Tenant ID: {tenantId}, Valid Audience: {localId}");
    options.Authority = $"https://login.microsoftonline.com/{tenantId}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuers = [
            $"https://sts.windows.net/{tenantId}/",
            $"https://login.microsoftonline.com/{tenantId}",
            $"https://login.microsoftonline.com/{tenantId}/v2.0"
        ],
        ValidateAudience = true,
        ValidAudiences = [
            $"api://{localId}",
            $"api://{clientId}"
        ],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
        {
            var openIdConfigManager = new Microsoft.IdentityModel.Protocols.ConfigurationManager<OpenIdConnectConfiguration>(
                $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever()
            );
            var config = openIdConfigManager.GetConfigurationAsync(CancellationToken.None).Result;
            return config.SigningKeys;
        }

    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Content-Disposition");
    });
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.MapControllers();
app.MapFallbackToFile("./index.html");
app.UseHttpsRedirection();
app.Run();