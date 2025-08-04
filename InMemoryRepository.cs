using MyDotNet9Api.Entities;

namespace MyDotNet9Api;

public class InMemoryRepository:IRepository
{
    private List<Genre> _genres;

    public InMemoryRepository()
    {
        _genres = new List<Genre>
        {
            new Genre { Id = 1, Name = "Action" },
            new Genre { Id = 2, Name = "Drama" },
        };
    }

    public List<Genre> GetAll()
    {
        return _genres;
    }

    public async Task<List<Genre>> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public bool Exist(string name)
    {
        return _genres.Any(g => g.Name == name);
    }
}