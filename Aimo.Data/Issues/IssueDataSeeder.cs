using Aimo.Domain.Data;
using Aimo.Domain.Issues;

namespace Aimo.Data.Issues;

//TODO:wire up data seeding
public partial class IssueDataSeeder : IDataSeeder<Issue>
{
    private readonly IRepository<Issue> _repository;

    public IssueDataSeeder(IRepository<Issue> repository)
    {
        _repository = repository;
    }

    public async Task Seed()
    {
        if (await _repository.AnyAsync())
            return;

        await _repository.AddBulkAsync(Issues);
        await _repository.CommitAsync();
    }

    private static Issue[] Issues =>
        new[] { new Issue { Title = "C: drive is full, please clean" } }; //TODO: from json file
}