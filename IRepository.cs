using MyDotNet9Api.Entities;

namespace MyDotNet9Api;

public interface IRepository
{
    bool Exist(string name);
    Task<List<Genre>> GetById(int id);
    List<Genre> GetAll();
    
}