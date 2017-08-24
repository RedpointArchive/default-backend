using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace RedpointDefaultBackend
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                if (context.Request.Path.HasValue && context.Request.Path.Value == "/healthz")
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Ok");
                    return;
                }

                if (context.Request.GetTypedHeaders().Accept.Any(x => x.MediaType == "application/json") ||
                    (context.Request.Headers.ContainsKey("X-Format") && context.Request.Headers["X-Format"].Aggregate((a, b) => a + b).Trim().Contains("application/json")))
                {
                    // Emit a "please retry" response.
                    if (context.Request.Headers.ContainsKey("X-Code"))
                    {
                        context.Response.StatusCode = int.Parse(context.Request.Headers["X-Code"].First());
                    }
                    else
                    {
                        context.Response.StatusCode = 502;
                    }
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        code = 6001,
                        message = "The service was temporarily unable to handle your request, please retry again",
                        fields = (string)null,
                    }));
                    return;
                }

                if (context.Request.Headers.ContainsKey("X-Code"))
                {
                    context.Response.StatusCode = int.Parse(context.Request.Headers["X-Code"].First());
                }
                else
                {
                    context.Response.StatusCode = 404;
                }

                await context.Response.WriteAsync(context.Response.StatusCode + " " + Enum.GetName(typeof(HttpStatusCode), context.Response.StatusCode));
            });
        }
    }
}
