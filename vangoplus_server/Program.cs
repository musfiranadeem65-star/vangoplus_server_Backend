using Microsoft.EntityFrameworkCore;
using vangoplus_server.Infrastructure.Data;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Application.Services;
using vangoplus_server.Application.Handlers;
using vangoplus_server.Application.Chat;
using IRouteHandler = vangoplus_server.Application.Interfaces.IRouteHandler;
using RouteHandler = vangoplus_server.Application.Handlers.RouteHandler;

var builder = WebApplication.CreateBuilder(args);

// Train the chatbot intent model and exit:  dotnet run -- train
if (args.Contains("train"))
{
    var trainingCsv = Path.Combine(builder.Environment.ContentRootPath, "ML", "vango_intents.csv");
    var modelFile = Path.Combine(builder.Environment.ContentRootPath, "ML", "intent_model.zip");
    vangoplus_server.ML.ModelTrainer.Train(trainingCsv, modelFile);
    return;
}

// Check what the model makes of a message, without starting the API or touching the
// database:  dotnet run -- classify "van kab aye gi"
if (args.Length >= 2 && args[0] == "classify")
{
    var classifier = new IntentClassifier(builder.Environment);
    var result = classifier.Classify(string.Join(' ', args.Skip(1)));
    Console.WriteLine($"{result.Intent}  (confidence {result.Confidence:P1}, matched by {result.Source})");
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

// Parent chatbot. The classifier is a SINGLETON because loading the ML.NET model is
// expensive and must happen once, not on every request.
builder.Services.AddSingleton<IntentClassifier>();
builder.Services.AddScoped<IChatContextService, ChatContextService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IEscalationService, EscalationService>();
builder.Services.AddScoped<IChatHandler, ChatHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Serve static files from wwwroot (uploads)
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
