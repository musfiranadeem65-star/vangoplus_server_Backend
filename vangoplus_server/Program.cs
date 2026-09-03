using Microsoft.EntityFrameworkCore;
using vangoplus_server.Infrastructure.Data;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Application.Services;
using vangoplus_server.Application.Handlers;
using IRouteHandler = vangoplus_server.Application.Interfaces.IRouteHandler;
using RouteHandler = vangoplus_server.Application.Handlers.RouteHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configure EF Core with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application services & handlers (services interact with DB directly)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserHandler, UserHandler>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentHandler, StudentHandler>();
builder.Services.AddScoped<IGuardianService, GuardianService>();
builder.Services.AddScoped<IGuardianHandler, GuardianHandler>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IDriverHandler, DriverHandler>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IRouteHandler, RouteHandler>();
builder.Services.AddScoped<IRouteStopService, RouteStopService>();
builder.Services.AddScoped<IRouteStopHandler, RouteStopHandler>();
builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
builder.Services.AddScoped<ISubscriptionPlanHandler, SubscriptionPlanHandler>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ISubscriptionHandler, SubscriptionHandler>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IAlertHandler, AlertHandler>();
builder.Services.AddScoped<IStudentRouteAssignmentService, StudentRouteAssignmentService>();
builder.Services.AddScoped<IStudentRouteAssignmentHandler, StudentRouteAssignmentHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Serve static files from wwwroot (uploads)
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
