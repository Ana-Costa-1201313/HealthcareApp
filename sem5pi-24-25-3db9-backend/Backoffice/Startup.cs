using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Backoffice.Infraestructure;
using Backoffice.Infraestructure.Users;
using Backoffice.Infraestructure.Staffs;
using Backoffice.Infraestructure.OperationTypes;
using Backoffice.Infraestructure.Shared;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Users;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Specializations;
using Backoffice.Infraestructure.Specializations;
using Backoffice.Domain.Logs;
using Backoffice.Infraestructure.Logs;
using System.Text.Json.Serialization;
using Backoffice.Domain.OperationRequests;
using Backoffice.Infraestructure.OperationRequests;
using Backoffice.Domain.Patients;
using Backoffice.Infraestructure.Patients;
using Backoffice.Domain.Appointments;
using Backoffice.Infraestructure.Appointments;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Backoffice.Infraestructure.SurgeryRooms;
using Backoffice.Infrastructure.SurgeryRooms;


namespace Backoffice
{
    public class Startup
    {
        public string DbPath { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "BD.db");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // services.AddDbContext<BDContext>(opt =>
            // opt.UseSqlServer("Server=vsgate-s1.dei.isep.ipp.pt,10513;Initial Catalog=BD;User Id=sa;Password=rscxDifxGw==Xa5;encrypt=true;TrustServerCertificate=True;")
            // .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());

            services.AddDbContext<BDContext>(opt =>
                opt.UseSqlite($"Data Source={DbPath}")
                .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());

            ConfigureMyServices(services);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "http://localhost:4201")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CORSPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<BDContext>();
                var dbBootstrap = serviceScope.ServiceProvider.GetRequiredService<DbBootstrap>();

                if (!dbContext.Users.Any())
                {
                    dbBootstrap.UserBootstrap();
                }

                if (!dbContext.Specializations.Any())
                {
                    dbBootstrap.SpecializationBootstrap();
                }

                if (!dbContext.Staff.Any())
                {
                    dbBootstrap.StaffBootstrap();
                }

                if (!dbContext.OperationTypes.Any())
                {
                    dbBootstrap.OperationTypeBootstrap();
                }

                if (!dbContext.Patients.Any())
                {
                    dbBootstrap.PatientBootstrap();
                }

                if (!dbContext.OperationRequests.Any())
                {
                    dbBootstrap.OperationRequestBootstrap();
                }

                if (!dbContext.SurgeryRoomTypes.Any())
                {
                    dbBootstrap.SurgeryRoomTypeBootstrap();
                }

                if (!dbContext.SurgeryRooms.Any())
                {
                    dbBootstrap.SurgeryRoomBootstrap();
                }

                if (!dbContext.Appointments.Any())
                {
                    dbBootstrap.AppointmentBootstrap();
                }
            }
        }

        public void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IOperationTypeRepository, OperationTypeRepository>();
            services.AddTransient<OperationTypeService>();

            services.AddTransient<ISpecializationRepository, SpecializationRepository>();
            services.AddTransient<SpecializationService>();

            services.AddTransient<ILogRepository, LogRepository>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<UserService>();

            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<PatientService>();

            services.AddTransient<IStaffRepository, StaffRepository>();
            services.AddTransient<StaffService>();
            services.AddTransient<StaffMapper>();
            services.AddTransient<OperationTypeMapper>();
            services.AddTransient<SpecializationMapper>();

            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<PatientService>();
            services.AddTransient<PatientMapper>();

            services.AddTransient<IOperationRequestRepository, OperationRequestRepository>();
            services.AddTransient<OperationRequestService>();

            services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<AppointmentService>();

            services.AddTransient<ISurgeryRoomTypeRepository, SurgeryRoomTypeRepository>();
            services.AddTransient<ISurgeryRoomRepository, SurgeryRoomRepository>();
            services.AddTransient<SurgeryRoomService>();

            services.AddTransient<IExternalApiServices, ExternalApiServices>();

            services.AddTransient<AuthService>();
            services.AddTransient<LogInServices>();
            services.AddTransient<ExternalApiServices>();
            services.AddTransient<TokenService>();
            services.AddTransient<ICD11Service>();
            services.AddHttpClient();
            services.AddHttpLogging(o => { });

            services.AddTransient<DbBootstrap>();
        }
    }
}