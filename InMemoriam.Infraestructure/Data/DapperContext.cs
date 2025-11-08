using Dapper;
using InMemoriam.Core.Enum;
using InMemoriam.Core.Interfaces;
using System.Data;
using System.Data.Common;

namespace InMemoriam.Infraestructure.Data
{
    public sealed class DapperContext : IDapperContext
    {
        private readonly IDbConnectionFactory _factory;
        private readonly AsyncLocal<(IDbConnection? Conn, IDbTransaction? Tx)> _ambient = new();

        public DapperContext(IDbConnectionFactory factory)
        {
            _factory = factory;
           
            SqlMapper.Settings.CommandTimeout = 30;
        }

        public DatabaseProvider Provider => _factory.Provider;

        public void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx)
            => _ambient.Value = (conn, tx);

        public void ClearAmbientConnection()
            => _ambient.Value = (null, null);

        private (IDbConnection conn, IDbTransaction? tx, bool owns) GetConnAndTx()
        {
            var amb = _ambient.Value;
            if (amb.Conn != null) return (amb.Conn, amb.Tx, false);

            var c = _factory.CreateConnection();
            return (c, null, true);
        }

        private static async Task OpenIfNeededAsync(IDbConnection conn)
        {
            if (conn is DbConnection dbc && dbc.State == ConnectionState.Closed)
                await dbc.OpenAsync();
        }

        private static async Task CloseIfOwnedAsync(IDbConnection conn, bool owns)
        {
            if (!owns) return;
            if (conn is DbConnection dbc && dbc.State != ConnectionState.Closed)
                await dbc.CloseAsync();
            conn.Dispose();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryAsync<T>(sql, param, tx, commandType: commandType);
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(
            string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryFirstOrDefaultAsync<T>(sql, param, tx, commandType: commandType);
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }

        public async Task<int> ExecuteAsync(
            string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.ExecuteAsync(sql, param, tx, commandType: commandType);
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(
            string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                // Usa la versión genérica de Dapper; respeta nulls
                var res = await conn.ExecuteScalarAsync<T>(sql, param, tx, commandType: commandType);
                // Mantengo la firma Task<T>; si viene null y T no acepta null, resultará default(T)
                return res!;
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }
    }
}
