using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace usmp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder) {
    base.OnModelCreating(builder);

    builder.Entity<Curso>().HasIndex(c => c.Codigo).IsUnique();

    // Restricción: Un usuario solo se matricula una vez por curso
    builder.Entity<Matricula>()
        .HasIndex(m => new { m.CursoId, m.UsuarioId }).IsUnique();
}

public DbSet<Curso> Cursos { get; set; }
public DbSet<Matricula> Matriculas { get; set; }
}
