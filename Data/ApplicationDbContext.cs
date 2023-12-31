using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zapp.Models;

namespace Zapp.Data;

public class ApplicationDbContext : IdentityDbContext<Employee, IdentityRole, string>
{
    private readonly string connectionString = "Server=localhost;Database=Zapp;Uid=ZappUser;Pwd=xhXNl)Lel)FKRT7];";

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Appointments)
            .WithOne(e => e.Employee)
            .HasForeignKey(e => e.EmployeeId)
            .IsRequired();

        modelBuilder.Entity<Customer>()
            .HasMany(e => e.Appointments)
            .WithOne(e => e.Customer)
            .HasForeignKey(e => e.CustomerId)
            .IsRequired();

        modelBuilder.Entity<Customer>()
            .HasMany(e => e.CustomerTasks)
            .WithOne(e => e.Customer)
            .HasForeignKey(e => e.CustomerId)
            .IsRequired();

        modelBuilder.Entity<TaskItem>()
            .HasMany(e => e.CustomerTasks)
            .WithOne(e => e.Task)
            .HasForeignKey(e => e.TaskId)
            .IsRequired();

        modelBuilder.Entity<TaskItem>()
            .HasMany(e => e.AppointmentTasks)
            .WithOne(e => e.Task)
            .HasForeignKey(e => e.TaskId)
            .IsRequired();

        modelBuilder.Entity<Appointment>()
            .HasMany(e => e.AppointmentTasks)
            .WithOne(e => e.Appointment)
            .HasForeignKey(e => e.AppointmentId)
            .IsRequired();
    }

    public virtual DbSet<Zapp.Models.Appointment> Appointment { get; set; } 
    public virtual DbSet<Zapp.Models.Customer> Customer { get; set; } 
    public virtual DbSet<Zapp.Models.TaskItem> TaskItem { get; set; }
    public virtual DbSet<Zapp.Models.CustomerTask> CustomerTask { get; set; }
    public virtual DbSet<Zapp.Models.AppointmentTask> AppointmentTask { get; set; }
}

