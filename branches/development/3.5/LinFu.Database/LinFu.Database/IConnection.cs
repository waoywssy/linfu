using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace LinFu.Database
{
    public interface IConnection : IDisposable
    {                        
        /// <summary>
        /// Sets or gets the connectionstring that is used to create new <see cref="IDbConnection"/> instances.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the providername that is used to locate provider spesific implementations.
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// Sets or gets the <see cref="DbProviderFactory"/> that is used to create provider spesific instances.
        /// </summary>
        DbProviderFactory ProviderFactory { get; set; }
        
        /// <summary>
        /// Gets a <see cref="IProcedures"/>
        /// </summary>
        IProcedures Procedures { get; }
        
        /// <summary>
        /// Sets or gets the <see cref="IsolationLevel"/>.
        /// </summary>
        IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// Sets or gets whether the source schema should be included when returning datatables.        
        /// </summary>
        bool FillSchema { get; set; }

        /// <summary>
        /// Returns <b>true</b> if the <see cref="IConnection"/> supports bulk loading, otherwise <b>false</b>
        /// </summary>
        bool SupportsBulkLoading { get; }
      
        /// <summary>
        /// Executes a <see cref="IDbCommand"/>
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns>A <see cref="DataTable"/> containing the result of the query.</returns>
        DataTable ExecuteDataTable(IDbCommand command);

        /// <summary>
        /// Exectutes a <see cref="IDbCommand"/>
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns>A <see cref="IDataReader"/> containing the result of the query.</returns>
        IDataReader ExecuteReader(IDbCommand command);

        /// <summary>
        /// Executes a <see cref="IDbCommand"/>
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns>The number of rows affected by the query.</returns>
        int ExecuteNonQuery(IDbCommand command);

        /// <summary>
        /// Executes a <see cref="IDbCommand"/>
        /// </summary>
        /// <typeparam name="T">The type of result returned.</typeparam>
        /// <param name="command">The command to be executed</param>
        /// <returns>The value of T returned by the query</returns>
        T ExecuteScalar<T>(IDbCommand command);

        /// <summary>
        /// Executes a query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <param name="command">The command text to be executed</param>
        /// <returns>A <see cref="DataTable"/> containing the result of the query.</returns>
        DataTable ExecuteDataTable(string commandText);
        
        /// <summary>
        /// Executes a query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <param name="command">The command text to be executed</param>
        /// <returns>A <see cref="IDataReader"/> containing the result of the query.</returns>
        IDataReader ExecuteReader(string commandText);

        /// <summary>
        /// Executes a query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <param name="command">The command text to be executed</param>
        /// <returns>The number of rows affected by the query.</returns>
        int ExecuteNonQuery(string commandText);

        /// <summary>
        /// Executes a query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <typeparam name="T">The type of result returned.</typeparam>
        /// <param name="command">The command to be executed</param>
        /// <returns>The value of T returned by the query</returns>
        T ExecuteScalar<T>(string commandText);

        /// <summary>
        /// Executes a parameterized query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <param name="commandText">The command text to be executed</param>
        /// <param name="parameters">A list of <see cref="Paremeter"/> instances to be used with the query</param>
        /// <returns>A <see cref="DataTable"/> containing the result of the query.</returns>
        DataTable ExecuteDataTable(string commandText, params Parameter[] parameters);

        /// <summary>
        /// Executes a parameterized query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <param name="commandText">The command text to be executed</param>
        /// <param name="parameters">A list of <see cref="Paremeter"/> instances to be used with the query</param>
        /// <returns>A <see cref="IDataReader"/> containing the result of the query.</returns>
        IDataReader ExecuteReader(string commandText, params Parameter[] parameters);

        /// <summary>
        /// Executes a parameterized query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <param name="commandText">The command text to be executed</param>
        /// <param name="parameters">A list of <see cref="Paremeter"/> instances to be used with the query</param>
        /// <returns>The number of rows affected by the query.</returns>
        int ExecuteNonQuery(string commandText, params Parameter[] parameters);

        /// <summary>
        /// Executes a parameterized query as represented by <paramref name="commandText"/>
        /// </summary>
        /// <typeparam name="T">The type of result returned.</typeparam>
        /// <param name="commandText">The command text to be executed</param>
        /// <param name="parameters">A list of <see cref="Paremeter"/> instances to be used with the query</param>
        /// <returns>The value of T returned by the query</returns>
        T ExecuteScalar<T>(string commandText, params Parameter[] parameters); 

        /// <summary>
        /// Bulk loads/copies the contents of the <paramref name="dataTable"/>
        /// </summary>
        /// <remarks>
        /// The data is loaded into the table as spesified by the datatable name.
        /// </remarks>
        /// <param name="dataTable"></param>
        void BulkLoad(DataTable dataTable);


        /// <summary>
        /// Creates a new <see cref="IDbCommand"/>
        /// </summary>
        /// <returns><see cref="IDbCommand"/></returns>
        IDbCommand CreateCommand();

        /// <summary>
        /// Creates a new <see cref="IDbCommand"/>
        /// </summary>
        /// <param name="commandText">The command text to be executed</param>
        /// <returns><see cref="IDbCommand"/></returns>
        IDbCommand CreateCommand(string commandText);

    }
}
