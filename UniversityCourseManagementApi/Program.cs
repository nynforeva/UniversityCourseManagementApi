using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Mapping;
using UniversityCourseManagementApi.Middleware;
using UniversityCourseManagementApi.Repositories.Interfaces;
using UniversityCourseManagementApi.Repositories.Implementations;
using UniversityCourseManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = ReturnResult<object>.Failure(
            400,
            "VALIDATION_ERROR",
            string.Join("; ", errors),
            context.HttpContext.TraceIdentifier);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(cfg =>
{
}, typeof(MappingProfile));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();