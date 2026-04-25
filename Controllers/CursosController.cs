using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using usmp.Data;
using usmp.Models; // Asegúrate de que apunte a tu namespace de modelos

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Cursos
    // Implementa el listado de cursos activos y los filtros requeridos
    public async Task<IActionResult> Index(string nombre, int? minCreditos, TimeSpan? horario)
    {
        // Solo cursos activos (Requerimiento Pregunta 2)
        var query = _context.Cursos.Where(c => c.Activo);

        // Filtro por nombre
        if (!string.IsNullOrEmpty(nombre))
        {
            query = query.Where(c => c.Nombre.Contains(nombre));
        }

        // Filtro por rango de créditos
        if (minCreditos.HasValue)
        {
            query = query.Where(c => c.Creditos >= minCreditos.Value);
        }

        // Filtro por horario
        if (horario.HasValue)
        {
            query = query.Where(c => c.HorarioInicio >= horario.Value);
        }

        return View(await query.ToListAsync());
    }

    // GET: Cursos/Details/5
    // Vista de detalle con el botón de inscripción
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var curso = await _context.Cursos
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (curso == null) return NotFound();

        return View(curso);
    }
}