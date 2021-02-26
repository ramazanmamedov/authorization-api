using System.Collections.Generic;

namespace Authentication.API.Context
{
    public static class UserDbContext
    {
        //Your UsersDbContext with dbsets here
        public static IEnumerable<User> Users { get; } = new User[]
        {
            new User
            {
                Id = "1",
                UserName = "username",
                Password = "password"
            }
        };
    }
}