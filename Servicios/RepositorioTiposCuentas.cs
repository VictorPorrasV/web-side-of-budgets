using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;

namespace Presupuesto.Servicios
{

    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    }

    //repositorio utilizados en tipos cuentas controller
    public class RepositorioTiposCuentas:IRepositorioTiposCuentas
    {

        //connection string
        private readonly string _connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        
        
        //insert para crear cuentas
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>
                                                        (@"insert into TiposCuentas(Nombre,UsuarioId,Orden)
                                                        values (@Nombre,@Usuarioid,0);
                                                        Select Scope_IDENTITY();",tipoCuenta);
            tipoCuenta.Id= id;  

        }



        //validaccion si existe
        public async Task<bool> Existe(string nombre,int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                @"Select 1 From 
                TiposCuentas Where Nombre =@Nombre 
                AND Usuarioid = @Usuarioid;",
                new { nombre,usuarioId});
                return existe == 1;



        }
        //select
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TipoCuenta>(
                    @"select id, nombre, orden 
                    from TiposCuentas 
                    where UsuarioId = @usuarioid", new { usuarioId });
        }

        //update
        public async Task Actualizar (TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(@"Update TiposCuentas 
                                            Set Nombre = @Nombre 
                                            Where id= @id", 
                                            tipoCuenta);

        }
        //select whit id
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(
                                    @"Select Id,Nombre,Orden 
                                    From Tiposcuentas 
                                    where id= @id and UsuarioId= @usuarioId"
                                   , new {id, usuarioId });
                
        }

        //borrar cuenta

        public async Task Borrar(int id)
        {

            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(@"Delete TiposCuentas Where Id = @Id", new {id});


        }


    }

    
}
