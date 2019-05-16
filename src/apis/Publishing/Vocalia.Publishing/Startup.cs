using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObjectBus.Extensions;
using Vocalia.Publishing.Db;
using Vocalia.Publishing.Media;
using Vocalia.Publishing.Repository;
using Vocalia.Publishing.ServiceBus;
using Vocalia.ServiceBus.Types;
using Vocalia.ServiceBus.Types.Publishing;
using Vocalia.Streams;

namespace Vocalia.Publishing
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

            //Configure editor database context.
            services.AddDbContext<PublishContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PublishDatabase")));

            services.CreateObjectBus<Vocalia.ServiceBus.Types.Publishing.Podcast, PodcastServiceBus>(p =>
                p.Configure(Configuration["AzureServiceBus:ConnectionString"], Queues.Publishing, ObjectBus.BusType.Reciever));

            services.CreateObjectBus<Timeline, TimelineServiceBus>(p =>
                p.Configure(Configuration["AzureServiceBus:ConnectionString"], Queues.Publishing, ObjectBus.BusType.Reciever));

            services.CreateObjectBus<Vocalia.ServiceBus.Types.Podcast.Podcast>(p =>
                p.Configure(Configuration["AzureServiceBus:ConnectionString"], Queues.Podcast, ObjectBus.BusType.Sender));

            services.AddScoped<IPublishingRepository, PublishingRepository>();
            services.AddSingleton<IStreamBuilder, StreamBuilder>();
            services.AddSingleton<IMediaStorage, MediaStorage>();
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
            app.UseMvc();
        }
    }
}
