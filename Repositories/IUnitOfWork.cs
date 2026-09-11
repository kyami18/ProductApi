namespace ProductApi.Repositories;

public interface IUnitOfWork
{
    Task SaveChanges();
}