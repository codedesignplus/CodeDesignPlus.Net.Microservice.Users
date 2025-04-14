namespace CodeDesignPlus.Net.Microservice.Users.Infrastructure.Repositories;

public class UsersRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UsersRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUsersRepository
{
   
}