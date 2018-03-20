using ApiForBlog.Business;
using ApiForBlog.Data;
using ApiForBlog.Util;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiForBlog
{
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApiForBlogContext>(options =>
                options.UseInMemoryDatabase("InMemoryDatabase"));
            services.AddScoped<PostService>();
            services.AddScoped<UserService>();

            #region [ token ]
            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                                    policy.RequireClaim(ClaimTypes.Role, "admin"));

                options.AddPolicy("UserPolicy", policy =>
                                    policy.RequireClaim(ClaimTypes.Role, "user"));
            });
            #endregion

            #region [ Swagger ]
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(
                    "Bearer",
                    new ApiKeyScheme()
                    {
                        In = "header",
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = "apiKey"
                    });

                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    //Title = "API FOR BLOG",
                    Description = "Services Layer for api for blog",
                    Contact = new Contact { Name = "Marcelo Dias", Email = "marc29dias@yahoo.com.br", Url = "www.marcdias.com.br" },
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "ApiForBlog.xml");
                c.IncludeXmlComments(xmlPath);
            });

            #endregion

            services.AddAutoMapper();
            services.AddMvc();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.InjectStylesheet("/swagger-ui/custom.css");
                c.InjectOnCompleteJavaScript("/swagger-ui/custom.js");
            });

            app.UseAuthentication();
            app.UseMvc();
            app.UseStaticFiles();

            //para iniciar com a tela do swagger automaticamente
            app.Run(context => {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });
        }
    }
}
