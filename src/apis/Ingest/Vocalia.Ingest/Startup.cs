using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vocalia.Ingest.Db;
using Vocalia.Ingest.Hubs;
using Vocalia.Ingest.Image;
using Vocalia.Ingest.Media;
using Vocalia.Ingest.Repositories;
using Vocalia.ServiceBus.Types;
using ObjectBus.Extensions;
using System.Collections.Generic;

namespace Vocalia.Ingest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = "https://vocalia.eu.auth0.com/";
                options.Audience = "https://api.vocalia.co.uk";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            //Configure ingest database context.
            services.AddDbContext<IngestContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IngestDatabase")));

            services.CreateObjectBus<IEnumerable<RecordingChunk>>(p =>
                p.Configure(Configuration["AzureServiceBus:ConnectionString"], Queues.Editor, ObjectBus.BusType.Sender));

            services.CreateObjectBus<ServiceBus.Types.Podcast>(p =>
               p.Configure(Configuration["AzureServiceBus:ConnectionString"], Queues.Editor, ObjectBus.BusType.Sender));

            services.AddScoped<IIngestRepository, IngestRepository>();
            services.AddSingleton<IImageStorage, ImageStorage>();
            services.AddSingleton<IMediaStorage, MediaStorage>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<VocaliaHub>("/voice");
            });
        }
    }
}
