using BrewPoint.Data;
using BrewPoint.Models;
using BrewPoint.Repositories.Implementations;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Implementations;
using BrewPoint.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Convert Render's PostgreSQL URL to Npgsql connection string
var databaseUrl = builder.Configuration.GetConnectionString("DefaultConnection");
if (databaseUrl != null && databaseUrl.StartsWith("postgresql://"))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    databaseUrl = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = databaseUrl;
}

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Repositories
builder.Services.AddScoped<ICoffeeRepository, CoffeeRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();

// Services
builder.Services.AddScoped<ICoffeeService, CoffeeService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddControllersWithViews();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BrewPoint API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {your JWT token}",
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

var app = builder.Build();

// Seed roles on startup
// Seed roles on startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // ── Seed Ingredients ──────────────────────────────────
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Ingredients.Any())
    {
        var ingredients = new List<Ingredient>
        {
            new Ingredient { Name = "Oat Milk",      Price = 0.50m },
            new Ingredient { Name = "Almond Milk",   Price = 0.50m },
            new Ingredient { Name = "Vanilla Syrup", Price = 0.30m },
            new Ingredient { Name = "Caramel Syrup", Price = 0.30m },
            new Ingredient { Name = "Whipped Cream", Price = 0.40m },
            new Ingredient { Name = "Cinnamon",      Price = 0.20m },
            new Ingredient { Name = "Extra Shot",    Price = 0.70m },
        };
        db.Ingredients.AddRange(ingredients);
        await db.SaveChangesAsync();
    }

    // ── Seed Coffees ───────────────────────────────────────
    if (!db.Coffees.Any())
    {
        var oatMilk = db.Ingredients.First(i => i.Name == "Oat Milk");
        var vanillaSyrup = db.Ingredients.First(i => i.Name == "Vanilla Syrup");
        var caramelSyrup = db.Ingredients.First(i => i.Name == "Caramel Syrup");
        var whippedCream = db.Ingredients.First(i => i.Name == "Whipped Cream");
        var extraShot = db.Ingredients.First(i => i.Name == "Extra Shot");
        var cinnamon = db.Ingredients.First(i => i.Name == "Cinnamon");

        var coffees = new List<Coffee>
        {
            new Coffee
            {
                Name        = "Espresso",
                Price       = 2.50m,
                Description = "A strong, concentrated coffee shot.",
                ImagePath   = "/images/espresso.jpg",
              
                CoffeeIngredients = new List<CoffeeIngredient>
                {
                    new CoffeeIngredient { Ingredient = extraShot }
                }
            },
            new Coffee
            {
                Name        = "Cappuccino",
                Price       = 3.50m,
                Description = "Espresso with steamed milk and thick foam.",
                ImagePath   = "/images/cappuccino.jpg",
 
                CoffeeIngredients = new List<CoffeeIngredient>
                {
                    new CoffeeIngredient { Ingredient = oatMilk },
                    new CoffeeIngredient { Ingredient = cinnamon }
                }
            },
            new Coffee
            {
                Name        = "Vanilla Latte",
                Price       = 4.00m,
                Description = "Smooth espresso with vanilla and steamed milk.",
                ImagePath   = "/images/latte.jpg",
           
                CoffeeIngredients = new List<CoffeeIngredient>
                {
                    new CoffeeIngredient { Ingredient = vanillaSyrup },
                    new CoffeeIngredient { Ingredient = oatMilk }
                }
            },
            new Coffee
            {
                Name        = "Caramel Macchiato",
                Price       = 4.50m,
                Description = "Espresso with caramel syrup and whipped cream.",
                ImagePath   = "/images/macchiato.jpg",
               
                CoffeeIngredients = new List<CoffeeIngredient>
                {
                    new CoffeeIngredient { Ingredient = caramelSyrup },
                    new CoffeeIngredient { Ingredient = whippedCream }
                }
            },
            new Coffee
            {
                Name        = "Americano",
                Price       = 3.00m,
                Description = "Espresso diluted with hot water.",
                ImagePath   = "/images/americano.jpg",
                
                CoffeeIngredients = new List<CoffeeIngredient>
                {
                    new CoffeeIngredient { Ingredient = extraShot }
                }
            },
            new Coffee
            {
                Name        = "Flat White",
                Price       = 3.80m,
                Description = "Velvety espresso with microfoam milk.",
                ImagePath   = "/images/flatwhite.jpg",
                
                CoffeeIngredients = new List<CoffeeIngredient>
                {
                    new CoffeeIngredient { Ingredient = oatMilk },
                    new CoffeeIngredient { Ingredient = extraShot }
                }
            }
        };

        db.Coffees.AddRange(coffees);
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();       
app.UseRouting();           
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapControllers();

app.Run();