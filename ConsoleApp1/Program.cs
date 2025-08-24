using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using ExpertSystem.EntityFramework.Services;
using System.Reflection.Metadata;

IDataService<User> userService = new GenericDataService<User>(new ExpertSystemDbContextFactory());
userService.Create(new User("admin", "1234", "admin@gmail.com", true)).Wait();
