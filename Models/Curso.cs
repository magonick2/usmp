using System.ComponentModel.DataAnnotations;

public class Curso {
    public int Id { get; set; }
    [Required] public string Codigo { get; set; } = string.Empty;
    [Required] public string Nombre { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores a 0")]
    public int Creditos { get; set; }
    public int CupoMaximo { get; set; }
    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }
    public bool Activo { get; set; } = true;
}