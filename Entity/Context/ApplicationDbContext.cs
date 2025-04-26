using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using Entity.Model;
using System.Reflection;
using System.Reflection.Emit;
using User = Entity.Model.User;


namespace Entity.Context
{
    /// <summary>
    /// Representa el contexto de la base de datos de la aplicación, proporcionando configuraciones y métodos
    /// para la gestión de entidades y consultas personalizadas con Dapper.
    /// </summary>....................................................,,mmm-
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del contexto de la base de datos.
        /// </summary>
        /// <param name="options">Opciones de configuración para el contexto de base de datos.</param>
        /// <param name="configuration">Instancia de IConfiguration para acceder a la configuración de la aplicación.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }

        ///
        /// DB SETS
        ///
        public DbSet<Rol> Rol { get; set; }
        public DbSet<MovimientInventory> MovimientInventory { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ImageItem> ImageItem { get; set; }
        public DbSet<Buyout> Buyout { get; set; }
        public DbSet<SaleDetail> SaleDetail { get; set; }
        public DbSet<Sale> Sale { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Form> Form { get; set; }
        public DbSet<RolForm> RolForm { get; set; }
        public DbSet<Sede> Sede { get; set; }
        public DbSet<User> User { get; set; }

        /// <summary>
        /// Configura los modelos de la base de datos aplicando configuraciones desde ensamblados.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo de base de datos.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolForm>()
                .Property(n => n.Permision)
                .HasConversion<string>();

            modelBuilder.Entity<Notification>()
                .Property(n => n.TypeAction)
                .HasConversion<string>();

            modelBuilder.Entity<Person>()
                .Property(n => n.TypeIdentification)
                .HasConversion<string>();

            modelBuilder.Entity<MovimientInventory>()
                .Property(n => n.TypeMovement)
                .HasConversion<string>();


            // Configuración de la relación de uno a muchos entre Inventory y MovimientInventory
            modelBuilder.Entity<MovimientInventory>()
                .HasOne(mi => mi.Inventory)
                .WithMany(i => i.MovimientInventories) 
                .HasForeignKey(mi => mi.IdInventory);

            // Configuración de la relación de uno a muchos entre Product y MovimientInventory
            modelBuilder.Entity<MovimientInventory>()
                .HasOne(mi => mi.Product) // Un MovimientInventory tiene un Product
                .WithMany(p => p.MovimientInventories) // Un Product tiene muchos MovimientInventory
                .HasForeignKey(mi => mi.IdProduct); // Clave foránea en MovimientInventory

            // Configuración de la relación de uno a muchos entre Inventory y Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Inventory) 
                .WithMany(i => i.Products) 
                .HasForeignKey(p => p.IdInventory);

            // Configuración de la relación de uno a uno entre Product y Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category) 
                .WithOne(c => c.Product) 
                .HasForeignKey<Product>(p => p.IdCategory); 

            // Configuración de la relación de uno a uno entre Product y ImageItem
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ImageItems) 
                .WithOne(ii => ii.Product) 
                .HasForeignKey<Product>(p => p.IdImageItem); 

            // Configuración de la relación de uno a muchos entre Product y Buyout
            modelBuilder.Entity<Buyout>()
                .HasOne(b => b.Product) 
                .WithMany(p => p.Buyouts)
                .HasForeignKey(b => b.IdProduct);

            // Configuración de la relación de muchos a uno entre Buyout y User
            modelBuilder.Entity<Buyout>()
                .HasOne(b => b.User) 
                .WithMany(u => u.Buyouts) 
                .HasForeignKey(b => b.IdUser);

            // Configuración de la relación de uno a muchos entre Product y SeleDetail
            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Product) 
                .WithMany(p => p.SeleDetails) 
                .HasForeignKey(sd => sd.IdProduct);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.IdPerson);


            // Configuración de la relación de uno a muchos entre User y Sele
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User) 
                .WithMany(u => u.Seles) 
                .HasForeignKey(s => s.IdUser);

            // Configuración de la relación de uno a muchos entre Sele y SeleDetail
            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Sele) 
                .WithMany(s => s.SeleDetails) 
                .HasForeignKey(sd => sd.IdSele);
            
            // Configuración de la relación de uno a muchos entre User y Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User) 
                .WithMany(u => u.Notifications) 
                .HasForeignKey(n => n.IdUser);

            //Configuración de la relación de uno a uno entre User y Rol
            
                modelBuilder.Entity<Rol>()
                    .HasOne(r => r.User)
                    .WithOne(u => u.Rol)
                    .HasForeignKey<Rol>(u => u.IdUser);
            

            // Configuración de la relación de muchos a muchos entre Rol y Form
            modelBuilder.Entity<RolForm>()
                .HasKey(rf => new { rf.IdRol, rf.IdForm }); // Clave compuesta

            modelBuilder.Entity<RolForm>()
                .HasOne(rf => rf.Rol) // Un RolForm tiene un Rol
                .WithMany(r => r.RolForms) // Un Rol tiene muchos RolForm
                .HasForeignKey(rf => rf.IdRol); // Clave foránea en RolForm

            modelBuilder.Entity<RolForm>()
                .HasOne(rf => rf.Form) // Un RolForm tiene un Form
                .WithMany(f => f.RolForms) // Un Form tiene muchos RolForm
                .HasForeignKey(rf => rf.IdForm); // Clave foránea en RolForm

            // Configuración de la relación de uno a muchos entre Module y Form
            modelBuilder.Entity<Form>()
                .HasOne(f => f.Module) 
                .WithMany(m => m.Forms) 
                .HasForeignKey(f => f.IdModule);

            // Configuración de la relación de uno a muchos entre Company y Sede
            modelBuilder.Entity<Sede>()
                .HasOne(s => s.Company) // Una Sede tiene una Company
                .WithMany(c => c.Sede) // Una Company tiene muchas Sede
                .HasForeignKey(s => s.IdCompany); // Clave foránea en Sede

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Configura opciones adicionales del contexto, como el registro de datos sensibles.
        /// </summary>
        /// <param name="optionsBuilder">Constructor de opciones de configuración del contexto.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            // Otras configuraciones adicionales pueden ir aquí
        }

        /// <summary>
        /// Configura convenciones de tipos de datos, estableciendo la precisión por defecto de los valores decimales.
        /// </summary>
        /// <param name="configurationBuilder">Constructor de configuración de modelos.</param>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos, asegurando la auditoría antes de persistir los datos.
        /// </summary>
        /// <returns>Número de filas afectadas.</returns>
        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();
        }

        /// <summary>
        /// Guarda los cambios en la base de datos de manera asíncrona, asegurando la auditoría antes de la persistencia.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indica si se deben aceptar todos los cambios en caso de éxito.</param>
        /// <param name="cancellationToken">Token de cancelación para abortar la operación.</param>
        /// <returns>Número de filas afectadas de forma asíncrona.</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve una colección de resultados de tipo genérico.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Una colección de objetos del tipo especificado.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve un solo resultado o el valor predeterminado si no hay resultados.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Un objeto del tipo especificado o su valor predeterminado.</returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

        //SobreCarga
        //public async Task<int> QueryFirstOrDefaultAsync(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        //{
        //    using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
        //    var connection = this.Database.GetDbConnection();
        //    return await connection.ExecuteAsync(command.Definition);
        //}

        public async Task<int> ExecuteAsync(String text, object parametres = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parametres, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.ExecuteAsync(command.Definition);
        }

        //Debolver Objeto
        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, query, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.ExecuteScalarAsync<T>(command.Definition);
        }

        /// <summary>
        /// Método interno para garantizar la auditoría de los cambios en las entidades.
        /// </summary>
        private void EnsureAudit()
        {
            ChangeTracker.DetectChanges();
        }

        /// <summary>
        /// Estructura para ejecutar comandos SQL con Dapper en Entity Framework Core.
        /// </summary>
        public readonly struct DapperEFCoreCommand : IDisposable
        {
            /// <summary>
            /// Constructor del comando Dapper.
            /// </summary>
            /// <param name="context">Contexto de la base de datos.</param>
            /// <param name="text">Consulta SQL.</param>
            /// <param name="parameters">Parámetros opcionales.</param>
            /// <param name="timeout">Tiempo de espera opcional.</param>
            /// <param name="type">Tipo de comando SQL opcional.</param>
            /// <param name="ct">Token de cancelación.</param>
            public DapperEFCoreCommand(DbContext context, string text, object parameters, int? timeout, CommandType? type, CancellationToken ct)
            {
                var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
                var commandType = type ?? CommandType.Text;
                var commandTimeout = timeout ?? context.Database.GetCommandTimeout() ?? 30;

                Definition = new CommandDefinition(
                    text,
                    parameters,
                    transaction,
                    commandTimeout,
                    commandType,
                    cancellationToken: ct
                );
            }

            /// <summary>
            /// Define los parámetros del comando SQL.
            /// </summary>
            public CommandDefinition Definition { get; }

            /// <summary>
            /// Método para liberar los recursos.
            /// </summary>
            public void Dispose()
            {
            }
        }
    }
}