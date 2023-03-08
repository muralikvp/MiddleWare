using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWebApp1
{
    public class TimingMiddleWare
    {
        private RequestDelegate Next { get; }
        private ILogger<TimingMiddleWare> Logger { get; }

        public TimingMiddleWare(ILogger<TimingMiddleWare> logger, RequestDelegate next)
        {
            this.Next = next;
            Logger = logger;
        }

        public async Task Invoke(HttpContext ctx)
        {
            var start = DateTime.UtcNow;
            await Next.Invoke(ctx);//Pass the context
            Logger.LogInformation($"Timing {ctx.Request.Path}:{(DateTime.UtcNow - start).TotalMilliseconds}");
        }
    }

    public static class TimingExtensions 
    {
        public static IApplicationBuilder UseTiming(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TimingMiddleWare>();
        }
    }
}