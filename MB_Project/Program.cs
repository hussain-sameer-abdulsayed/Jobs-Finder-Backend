using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MB_Project.Data;
using System.Text.Json.Serialization;
using MB_Project;
using MB_Project.IRepos;
using MB_Project.Repos;
using MB_Project.AuthJwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MB_ProjectContextConnection") ?? throw new InvalidOperationException("Connection string 'MB_ProjectContextConnection' not found.");

builder.Services.AddDbContext<MB_ProjectContext>(options =>
{
    options.UseSqlServer(connectionString);
    //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddAutoMapper(typeof(DataMapper));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MB_ProjectContext>().AddRoles<IdentityRole>();

// Add services to the container.``
/*
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:7181",
            ValidAudience = "https://localhost:7181",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                                    (builder.Configuration.GetSection("Jwt:Key").Value!))
        };
    });
*/

builder.Services.AddAuthentication(x => {
x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o => {
var Key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
o.SaveToken = true;
o.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Key),
    ClockSkew = TimeSpan.Zero
};
});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IReviewRepo, ReviewRepo>();
builder.Services.AddScoped<IPostImageRepo, PostImageRepo>();
builder.Services.AddScoped<IPostFeatureRepo, PostFeatureRepo>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
builder.Services.AddScoped<IJWTManagerRepo, JWTManagerRepo>();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000");
                          policy.AllowAnyMethod();
                          policy.AllowAnyHeader();
                      });
});

/*
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminOrSeller",
        policy => policy.RequireRole("ADMIN", "SELLER"));
});
*/
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
var app = builder.Build();

//i added these two lines
//app.MapGet("/security/getMessage",
//() => "Hello World!").RequireAuthorization();








app.UseStaticFiles(); // i added this for see images middleware 

//builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
//builder.Services.AddScoped<IProductRepo, ProductRepo>();
//builder.Services.AddScoped<ICartItemRepo, CartItemRepo>();
//builder.Services.AddScoped<IOrderItemRepo, OrderItemRepo>();
//builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication(); // i added this

app.UseAuthorization();



app.MapControllers();

app.Run();
