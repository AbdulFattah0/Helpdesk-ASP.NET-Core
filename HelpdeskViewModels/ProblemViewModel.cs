using HelpdeskDAL;
using Microsoft.EntityFrameworkCore;

public class ProblemViewModel
{
    private readonly  ProblemDAO _dao;

    public int? Id { get; set; }
    public string? Description { get; set; } // Initialize to avoid CS8618

    public ProblemViewModel(DbContextOptions<HelpdeskContext> options)
    {
        _dao = new ProblemDAO(options);
    }

    public ProblemViewModel()
    {
        var options = new DbContextOptionsBuilder<HelpdeskContext>().Options;
        _dao = new ProblemDAO(options); // Default initialization
    }

    public ProblemViewModel? GetByDescription(string description)
    {
        var problem = _dao.GetByDescription(description);
        if (problem == null) return null;

        return new ProblemViewModel
        {
            Id = problem.Id,
            Description = problem.Description
        };
    }


    public List<ProblemViewModel> GetAll()
    {
        var problems = _dao.GetAll();
        return problems.Select(problem => new ProblemViewModel
        {
            Id = problem.Id,
            Description = problem.Description
        }).ToList();
    }

}
