using System.Net;
using EmberFrameworksService.Managers.MVC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors((options) =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200",
                "https://emberframeworks.xyz",
                "https://emberframeworks.xyz/",
                "https://management.emberframeworks.xyz",
                "https://api.emberframeworks.xyz");
            policy.WithMethods("GET", "POST", "OPTIONS");
            policy.AllowAnyHeader();
        });
});
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMvc((options =>
{
    options.InputFormatters.Insert(0, new RawJsonBodyInputFormatter());
}));
builder.Services.AddSwaggerGen();
builder.WebHost.ConfigureKestrel((Options) => { });
builder.WebHost.UseUrls("http://0.0.0.0:5000", "https://0.0.0.0:5001");
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("192.168.1.135"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders();  

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();