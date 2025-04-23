namespace CodeDesignPlus.Net.Microservice.Users.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
   
}