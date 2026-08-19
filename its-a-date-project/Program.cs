using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using its_a_date_project.Data;
using its_a_date_project.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
        sql => sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null)));

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
builder.Services.AddSingleton<LoginRateLimiter>();

builder.Services.AddAuthentication("AdminAuth")
    .AddCookie("AdminAuth", options =>
    {
        options.Cookie.Name = "ItsADateAdminAuth";
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations and seed baseline data (Pink theme, "welcome" invite, admin password) on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    var generatedPassword = SeedData.EnsureSeeded(db);
    if (generatedPassword is not null)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Generated admin password (shown once): {Password}", generatedPassword);
    }
}

// Configure the HTTP request pipeline.
// Trust X-Forwarded-* headers from the reverse proxy most hosts (including the free ASP.NET
// hosting this targets) sit behind — otherwise HTTPS redirection and secure cookies misbehave.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    // Send anyone hitting the default *.azurewebsites.net host (or any other host) straight to the
    // real custom domain, in one hop — same app either way, this just keeps one canonical public URL.
    const string CanonicalHost = "its-a-date.yousefaboelmagd.com";
    app.Use(async (context, next) =>
    {
        if (!string.Equals(context.Request.Host.Host, CanonicalHost, StringComparison.OrdinalIgnoreCase))
        {
            var url = $"https://{CanonicalHost}{context.Request.Path}{context.Request.QueryString}";
            context.Response.Redirect(url, permanent: true);
            return;
        }
        await next();
    });

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
