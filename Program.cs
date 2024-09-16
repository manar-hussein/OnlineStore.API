using Microsoft.OpenApi.Models;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.MailSendServices;
using OnlineStore.Application.Mappign;
using OnlineStore.Application.Services;
using OnlineStore.Domain.ServicesInterfaces;
using OnlineStore.Infrastructure.MailSendServices;


namespace OnlineStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            RegisterServices.AddRegisterServices(builder.Services);

            //  builder.Services.AddAuthorization();
            //Config ModelState Configuration
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issu"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Aud"],
                    IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
                };
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));


            // Db Config
            builder.Services
                .AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<OnlineStoreDbContext>()
                .AddDefaultTokenProviders();

          
            builder.Services.AddDbContext<OnlineStoreDbContext>
                (
                    option => option.UseSqlServer(builder.Configuration.GetConnectionString("Dev"))
                );



          
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen();

            //EmailSenderAddConfiguration
            builder.Services.Configure<EmailBaseConfiguration>(builder.Configuration.GetSection("MailSetting"));
            builder.Services.AddTransient<ISendEmail, EmailSender>();

            builder.Services.AddScoped<IProductServices, ProductServices>();

            #region Swagger REgion
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 5 Web API",
                    Description = " ITI Projrcy"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
            });
            #endregion


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("mypolicy", policy => policy.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseCors("mypolicy");
            //app.UseRouting();
          //  app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
