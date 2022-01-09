using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application;
using TweetBook.Application.Authorization;
using TweetBook.Application.Data;
using TweetBook.Application.HealthChecks;
using TweetBook.Contracts.Models.ResponseModels;
using TweetBook.Data;

namespace TweetBook
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


            services.AddHealthChecks()
                .AddDbContextCheck<TweetBookDbContext>()
                .AddCheck<RedisHealthCheck>("Redis");

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<TweetBookDbContext>();
            services.AddControllersWithViews();


            services.AddApplicationServices(Configuration);

            var JWTSettings = new TweetBook.Application.Options.JWTSettings();
            Configuration.Bind(nameof(JWTSettings), JWTSettings);
            services.AddSingleton(JWTSettings);

            var TokenParamsValidations = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = false,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSettings.JWTSecret)),
                ValidateLifetime = true
            };
            services.AddSingleton(TokenParamsValidations);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = TokenParamsValidations;

            });

            //services.AddAuthorization(options=> {
            //    options.AddPolicy("postowner", policy =>
            //    {
            //        policy.RequireClaim("delete", "true");
            //    });
            //});
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MicrosoftPolicy", policy =>
                {
                    policy.AddRequirements(new WorkForCompanyRequirement("Microsoft.com"));
                });

            });

            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Tweets Books", Version = "v1" });
                option.ExampleFilters();

                option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authentication header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                }
                );

                option.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                       new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[0] {}
                   }
               });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                option.IncludeXmlComments(xmlPath, true);
            });
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

            services.AddApiVersioning(option =>
            {
                option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.ReportApiVersions = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {

                app.UseHsts();
            }
            
            app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                 {
                     context.Response.ContentType = "application/json";
                     var response = new HealthCheckResponse
                     {
                         Status = report.Status.ToString(),
                         Duration = report.TotalDuration,
                         Checks = report.Entries.Select(ent => new HealthCheck
                         {
                             Component = ent.Key,
                             Status = ent.Value.Status.ToString(),
                             Description = ent.Value.Description
                         })
                     };
                     await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                 }

            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            var swaggerOptions = new TweetBook.Application.Options.SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, "v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
