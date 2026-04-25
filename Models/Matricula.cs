using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class Matricula {
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso? Curso { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public IdentityUser? Usuario { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
}

public enum EstadoMatricula { Pendiente, Confirmada, Cancelada }