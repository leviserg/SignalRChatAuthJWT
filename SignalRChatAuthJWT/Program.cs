using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using SignalRChatAuthJWT.Auth;
using SignalRChatAuthJWT.Hubs;
using SignalRChatAuthJWT.Providers;
using SignalRChatAuthJWT.Workers;

var builder = WebApplication.CreateBuilder(args);

// ========== SERVICES ==========

builder.Services.AddOptions();
builder.Services.Configure<UsersList>(builder.Configuration.GetSection("FakeUsers"));
builder.Services.AddControllers();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserProvider>();
builder.Services.AddHostedService<SubscriptionWorker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

var parameters = new TokenValidationParameters();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.Issuer,
            RequireAudience = true,
            ValidateAudience = true,
            ValidAudience = AuthOptions.Audience,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            IssuerSigningKey = AuthOptions.PublicKey
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["token"];

                if (!string.IsNullOrWhiteSpace(accessToken) &&
                    context.Request.Path.StartsWithSegments("/chat"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("MyPolicy", policy =>
    {
        policy.Requirements.Add(new MyAuthPolicy());
    });
});

builder.Services.AddResponseCompression(
    options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" })
);


// ========== APP ==========


var app = builder.Build();

app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("MyCorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();
