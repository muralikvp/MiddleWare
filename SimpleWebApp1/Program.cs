using Microsoft.AspNetCore.Builder;
using SimpleWebApp1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //This middleware is used reports app runtime errors in development environment.  
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseMiddleware<TimingMiddleWare>();

// app.UseTiming();


app.Map("/map1", HandleMapTest1);
app.Map("/map2", HandleMapTest2);

app.Map("/level1", level1App => {
    level1App.Map("/level2a", level2AApp);
    level1App.Map("/level2b", level2BApp);
});

app.MapWhen(context => context.Request.Query.ContainsKey("map3"), map3app);

app.Use(async (ctx, next) =>
{
    var start = DateTime.UtcNow;
    await next.Invoke(ctx);//Pass the context
    app.Logger.LogInformation($"Request {ctx.Request.Path}:{(DateTime.UtcNow - start).TotalMilliseconds}");
    //await ctx.Response.WriteAsync("Beginning to Middleware…"); 
});

// app.Use((HttpContext ctx, Func<Task> next) =>
// {
//     app.Logger.LogInformation("Terminating the Request");
//     return Task.CompletedTask;
// });

//This middleware is used to redirects HTTP requests to HTTPS.  
app.UseHttpsRedirection();
//This middleware is used to returns static files and short-circuits further request processing.   
app.UseStaticFiles();
//This middleware is used to route requests.   
app.UseRouting();

//This middleware is used to authorizes a user to access secure resources.  
app.UseAuthorization();

//This middleware is used to add Razor Pages endpoints to the request pipeline.    
app.MapRazorPages();

app.Run();


static void HandleMapTest1(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        app.UseTiming();
        await context.Response.WriteAsync("Map Test 1");
    });
}

static void HandleMapTest2(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 2");
    });
}


static void level2AApp(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("level2AApp");
    });
}

static void level2BApp(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("level2BApp");
    });
}



static void map3app(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("map3app");
    });
}

