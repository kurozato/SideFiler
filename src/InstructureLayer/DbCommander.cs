using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Repository
{
    public interface IDbCommander
    {
        string ConnectionString(string filePath);
        IEnumerable<TModel> Get<TModel>(string commandText, object? param, string connectionString);
        int Execute(string commandText, object? param, string connectionString);
        Task<IEnumerable<TModel>> GetAsync<TModel>(string commandText, object? param, string connectionString);
        Task<int> ExecuteAsync(string commandText, object? param, string connectionString);
    }
    public class DbCommander : IDbCommander
    {
        public string ConnectionString(string filePath)
        {
            return new SQLiteConnectionStringBuilder { DataSource = filePath }.ToString();
        }

        public IEnumerable<TModel> Get<TModel>(string commandText, object? param, string connectionString)
        {
            IEnumerable<TModel> results;

            using (var cn = new SQLiteConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        results = cn.Query<TModel>(commandText, param, tx);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }

            }
            return results;
        }

        public int Execute(string commandText, object? param, string connectionString)
        {
            int result;
            using (var cn = new SQLiteConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        result = cn.Execute(commandText, param, tx);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                    
                }
            }

            return result;
        }


        public async Task<IEnumerable<TModel>> GetAsync<TModel>(string commandText, object? param, string connectionString)
        {
            IEnumerable<TModel> results;

            using (var cn = new SQLiteConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        results = await cn.QueryAsync<TModel>(commandText, param, tx);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }

            }
            return results;
        }

        public async Task<int> ExecuteAsync(string commandText, object? param, string connectionString)
        {
            int result;
            using (var cn = new SQLiteConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        result = await cn.ExecuteAsync(commandText, param, tx);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }

                }
            }

            return result;
        }

    }
}
