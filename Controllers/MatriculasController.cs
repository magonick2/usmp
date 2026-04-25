using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using usmp.Data;
using usmp.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize] // Requisito: Usuario autenticado 
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribirse(int cursoId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var curso = await _context.Cursos.FindAsync(cursoId);
        if (curso == null || !curso.Activo) return NotFound();

        // 1. Validar Cupo Máximo 
        int inscritos = await _context.Matriculas
            .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);
        
        if (inscritos >= curso.CupoMaximo)
        {
            TempData["Error"] = "No hay cupos disponibles.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }

        // 2. Validar que no esté ya matriculado 
        bool yaMatriculado = await _context.Matriculas
            .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada);

        if (yaMatriculado)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }

        // 3. CORRECCIÓN PARA SQLITE: Validar solapamiento de horario
        // Traemos las matrículas actuales a memoria (.ToListAsync()) para poder comparar TimeSpan
        var misMatriculasActivas = await _context.Matriculas
            .Include(m => m.Curso)
            .Where(m => m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada)
            .ToListAsync();

        bool choqueHorario = misMatriculasActivas.Any(m => 
            curso.HorarioInicio < m.Curso!.HorarioFin && 
            m.Curso.HorarioInicio < curso.HorarioFin);

        if (choqueHorario)
        {
            TempData["Error"] = "Cruce de horario detectado con otra matrícula.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }

        // 4. Crear registro en estado Pendiente 
        var matricula = new Matricula
        {
            CursoId = cursoId,
            UsuarioId = userId!,
            FechaRegistro = DateTime.Now,
            Estado = EstadoMatricula.Pendiente
        };

        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Inscripción registrada exitosamente.";
        return RedirectToAction("Index", "Cursos");
    }
}