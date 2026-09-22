using Microsoft.EntityFrameworkCore;
using vangoplus_server.Infrastructure.Data;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Application.Services;
using vangoplus_server.Application.Handlers;
using vangoplus_server.Application.Chat;
using IRouteHandler = vangoplus_server.Application.Interfaces.IRouteHandler;
using RouteHandler = vangoplus_server.Application.Handlers.RouteHandler;

var builder = WebApplication.CreateBuilder(args);

// Train the chatbot intent model and exit:
// dotnet run -- train
if (args.Contains("train"))
{
    var trainingCsv = Path.Combine(
        builder.Environment.ContentRootPath,
        "ML",
        "vango_intents.csv"
    );

    var modelFile = Path.Combine(
        builder.Environment.ContentRootPath,
        "ML",
        "intent_model.zip"
    );

    vangoplus_server.ML.ModelTrainer.Train(trainingCsv, modelFile);
    return;
}

// Check what the chatbot model makes of a message, without starting
// the API or touching the database:
// dotnet run -- classify "van kab aye gi"
if (args.Length >= 2 && args[0] == "classify")
{
    var classifier = new IntentClassifier(builder.Environment);

    var result = classifier.Classify(
        string.Join(' ', args.Skip(1))
    );

    Console.WriteLine(
        $"{result.Intent}  (confidence {result.Confidence:P1}, matched by {result.Source})"
    );

    return;
}

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
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Application services & handlers
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

// School settings
builder.Services.AddScoped<ISchoolSettingService, SchoolSettingService>();
builder.Services.AddScoped<ISchoolSettingHandler, SchoolSettingHandler>();

// ---------------------------------------------------------
// Parent Chatbot
// ---------------------------------------------------------

// IntentClassifier is Singleton because the ML.NET model is expensive
// to load and should only be loaded once.
builder.Services.AddSingleton<IntentClassifier>();

builder.Services.AddScoped<IChatContextService, ChatContextService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IEscalationService, EscalationService>();
builder.Services.AddScoped<IChatHandler, ChatHandler>();

// ---------------------------------------------------------

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Serve static files from wwwroot (uploads)
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Seed missing subscription plans at startup
// (do not duplicate existing records)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Read existing plan names to avoid duplicates
    var existingNames = db.SubscriptionPlans
        .Select(p => p.Name)
        .ToList();

    var plansToAdd = new List<SubscriptionPlan>();

    if (!existingNames.Contains("Basic"))
    {
        plansToAdd.Add(new SubscriptionPlan
        {
            Name = "Basic",
            Price = 3000m,
            MaxChildren = 1,
            Features = "Morning pickup or afternoon drop,Live trip tracking,SMS & in-app alerts"
        });
    }

    if (!existingNames.Contains("Standard"))
    {
        plansToAdd.Add(new SubscriptionPlan
        {
            Name = "Standard",
            Price = 5000m,
            MaxChildren = 2,
            Features = "Morning pickup and afternoon drop,Live trip tracking,Priority support,Up to 2 children"
        });
    }

    if (!existingNames.Contains("Premium"))
    {
        plansToAdd.Add(new SubscriptionPlan
        {
            Name = "Premium",
            Price = 7500m,
            MaxChildren = 3,
            Features = "Round-trip transport for up to 3 children,Live trip tracking,24/7 priority support,Unlimited guardians"
        });
    }

    if (plansToAdd.Any())
    {
        db.SubscriptionPlans.AddRange(plansToAdd);
        db.SaveChanges();
    }
}

app.Run();