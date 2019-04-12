using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObjectBus.Extensions;
using ObjectBus.Options;
using System.Collections.Generic;
using Vocalia.Editor.Db;
using Vocalia.Editor.Media;
using Vocalia.Editor.Repository;
using Vocalia.Editor.ServiceBus;
using Vocalia.Editor.Streams;
using Vocalia.ServiceBus.Types;
using Vocalia.UserFacade;

namespace Vocalia.Editor
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Configure editor database context.
            services.AddDbContext<EditorContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("EditorDatabase")));

            //Configure service bus for objects.
            services.CreateObjectBus<IEnumerable<RecordingChunk>, RecordingChunkServiceBus>(p =>
                p.Configure(Configuration["AzureServiceBus:ConnectionString"], Queues.Editor, ObjectBus.BusType.Reciever));

            services.CreateObjectBus<Vocalia.ServiceBus.Types.Podcast, PodcastServiceBus>(p =>
                p.Configure(Configuration["AzureServiceBus:ConnectionString"], Queues.Editor, ObjectBus.BusType.Reciever));

            services.AddSingleton<IUserFacade, UserFacade.UserFacade>();
            services.AddSingleton<IStreamBuilder, StreamBuilder>();
            services.AddSingleton<IMediaStorage, MediaStorage>();
            services.AddScoped<IEditorRepository, EditorRepository>();
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
        }
    }
}
