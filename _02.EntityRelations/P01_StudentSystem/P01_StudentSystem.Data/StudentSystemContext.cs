using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Common;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        { }

        public StudentSystemContext(DbContextOptions options) : base(options)
        {
           
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
                optionsBuilder.UseSqlServer(Config.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Student
            
            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentId);

            modelBuilder.Entity<Student>()
                .Property(s => s.Name).HasMaxLength(GlobalConstants.StudentNameMaxLength);

            modelBuilder.Entity<Student>()
                .Property(s => s.PhoneNumber)
                .HasMaxLength(GlobalConstants.StudentPhoneNumberMaxLength)
                .IsRequired(false)
                .IsUnicode(false);
            
            modelBuilder.Entity<Student>()
                .Property(s => s.Birthday).IsRequired(false);
            
            //Course

            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);

            modelBuilder.Entity<Course>()
                .Property(c => c.Name).HasMaxLength(GlobalConstants.CourseNameMaxLength);

            modelBuilder.Entity<Course>()
                .Property(c => c.Description).IsRequired(false);

            //Resource

            modelBuilder.Entity<Resource>()
                .HasKey(r => r.ResourceId);

            modelBuilder.Entity<Resource>()
                .Property(r => r.Name).HasMaxLength(GlobalConstants.ResourceNameMaxLength);

            modelBuilder.Entity<Resource>()
                .Property(r => r.Url).IsUnicode(false);
            
            //Homework
            
            modelBuilder.Entity<Homework>()
                .HasKey(h => h.HomeworkId);

            modelBuilder.Entity<Homework>()
                .Property(h => h.Content).IsUnicode(false);

            //StudentCourse
            modelBuilder.Entity<StudentCourse>().HasKey(sc => new { sc.StudentId, sc.CourseId });
            
            //Relations

            modelBuilder.Entity<StudentCourse>()
                .HasOne(c => c.Course)
                .WithMany(sc => sc.StudentsEnrolled)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(s => s.Student)
                .WithMany(sc => sc.CourseEnrollments)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Homework>()
                .HasOne(h => h.Student)
                .WithMany(s => s.HomeworkSubmissions)
                .HasForeignKey(h => h.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Homework>()
                .HasOne(h => h.Course)
                .WithMany(c => c.HomeworkSubmissions)
                .HasForeignKey(h => h.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Resources)
                .WithOne(r => r.Course)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}