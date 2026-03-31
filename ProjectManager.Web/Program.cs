using ProjectManager.DAL.Connection;
using ProjectManager.DAL.Repositories;
using ProjectManager.BLL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<ProjectService>();

// Injection de dépendance - DAL
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddSingleton(new DatabaseConnection(connectionString));
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // ?? doit être AVANT UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();