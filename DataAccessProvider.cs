using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.VisualBasic;
using Microsoft.Data.SqlClient;




namespace DLR.BOM.Providers
{
    /// <summary>
    /// Class DataAccessProvider.
    /// Implements the <see cref="DLR.BOM.Contracts.IDataAccessProvider" />
    /// </summary>
    /// <seealso cref="DLR.BOM.Contracts.IDataAccessProvider" />
    public class DataAccessProvider 
    {
        private string _connectionString;
        private IConfiguration _configuration;


        private readonly ILogger<DataAccessProvider> _Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessProvider"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public DataAccessProvider()
        {
            _connectionString = "data source=tcp:syn-dlr-eda-dev.sql.azuresynapse.net,1433;initial catalog=syndw;user id=svc_az_pe@digitalrealty.com;password=Wel2024come?;authentication=active directory password;connection timeout=999999";
        }


        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetItems<T>(string query, object parameters, bool isStoredProcedure = false) where T : class
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var items = connection.Query<T>(query, parameters, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text, commandTimeout: 360000);
                    return items;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(1, ex, ex.Message);
                //return Enumerable.Empty<T>();
                throw;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="obj">The object.</param>
        /// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
        /// <returns>T.</returns>
        public T GetItem<T>(string query, object obj, bool isStoredProcedure = false)
        {
            T item = default(T);
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var items = connection.Query<T>(query, obj, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
                    item = items != null ? items.FirstOrDefault() : default(T);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(1, ex, ex.Message);
                throw;
            }

            return item;
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public int AddItem(string query, object item)
        {
            //Do not include try catch, as the actual exception is used to show in the UI.
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                string sql = string.Concat("DECLARE @ID int; ", query, "; SET @ID = SCOPE_IDENTITY(); SELECT @ID");
                var response = connection.Query<int>(sql, item);
                return response != null ? response.Single() : 0;
            }
        }

        #region Update Method

        /// <summary>
        /// Updates the item.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="item">The item.</param>
        /// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
        /// <returns>System.Int32.</returns>
        public int UpdateItem(string query, object item, bool isStoredProcedure = false)
        {
            //Do not include try catch, as the actual exception is used to show in the UI.
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var response = connection.Query(query, item, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
                return response != null && response.Count() > 0 ? response.Single() : 0;
            }
        }

        #endregion

        #region Execute Method

        /// <summary>
        /// Executes the specified database object.
        /// </summary>
        /// <param name="dbObject">The database object.</param>
        /// <param name="item">The item.</param>
        /// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
        /// <returns>System.Int32.</returns>
        public int Execute(string dbObject, object item, bool isStoredProcedure = false)
        {
            //Do not include try catch, as the actual exception is used to show in the UI.
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var res = connection.Query(dbObject, item, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text, commandTimeout: 360000);
                return res != null && res.Count() > 0 ? res.SingleOrDefault() : 0;
            }
        }

        public IEnumerable<T> GetDataItems<T>(string query, object parameters, bool isStoredProcedure = false) where T : class
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var items = connection.Query<T>(query, parameters, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text, commandTimeout: 360000);
                    return items;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(1, ex, ex.Message);
                throw ex;
            }
        }

        public T GetDataItem<T>(string query, object obj, bool isStoredProcedure = false)
        {
            T item = default(T);
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var items = connection.Query<T>(query, obj, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
                    item = items != null ? items.FirstOrDefault() : default(T);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(1, ex, ex.Message);
                throw ex;
            }

            return item;
        }


        #endregion

        #region QueryMultiples Method
        /// <summary>
        /// Queries the multiples.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
        /// <param name="readerFuncs">The reader funcs.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        public List<object> QueryMultiples(string queries, object parameters = null, bool isStoredProcedure = false, params Func<dynamic, object>[] readerFuncs)
        {
            var results = new List<object>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var resultSets = connection.Query(queries, parameters, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);

                    foreach (var readerFunc in readerFuncs)
                    {
                        var res = readerFunc(resultSets);
                        results.Add(res);
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(1, ex, ex.Message);
            }

            return results;
        }

        #endregion

        #region QueryMultiples Method
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="item">The item.</param>
        /// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
        /// <returns>T.</returns>
        public T ExecuteScalar<T>(string query, object item, bool isStoredProcedure = false)
        {
            //Do not include try catch, as the actual exception is used to show in the UI.
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.ExecuteScalar<T>(query, item, commandType: isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text, commandTimeout: 360000);
            }
        }

        #endregion

    }
}