using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using ShareSpaceApi.Data.Context;
using ShareSpaceApi.Hubs;
using ShareSpaceApi.Jwt;
using ShareSpaceApi.Repository;
using ShareSpaceApi.Repository.Contracts;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddSignalR();
builder.Services.AddCors();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INotificationRepostiory, NotificationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();

builder.Services.AddDbContext<ShareSpaceDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]
    );
});

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection(nameof(TokenSettings)));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var token_settings = builder.Configuration
            .GetSection(nameof(TokenSettings))
            .Get<TokenSettings>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = token_settings!.Issuer,
            ValidateAudience = true,
            ValidAudience = token_settings!.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(token_settings.SecretKey)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(
    policy =>
        policy
            .WithOrigins(["http://localhost:4200", "https://localhost:4200"])
            .AllowAnyHeader()
            .WithHeaders(HeaderNames.ContentType)
);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.UseStaticFiles();
app.MapHub<MessageHub>("/messagehub");
app.MapHub<NotificationHub>("/notificationhub");
app.UseHttpsRedirection();

app.Run();