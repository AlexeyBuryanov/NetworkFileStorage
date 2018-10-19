using System.Data.Entity;

namespace Client.Model
{
    /// <summary>
    /// Контекст данных для взаимодействия с БД
    /// </summary>
    public class LoginContext : DbContext
    {
        public LoginContext() : base("DefaultConnection") {} // LoginContext
        public DbSet<Login> Logins { get; set; }
    } // LoginContext
} // Client.Model