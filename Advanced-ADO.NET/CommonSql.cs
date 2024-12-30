using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace AdvancedADO
{
    //public delegate T MapDataFunction<T>(IDataReader Reader, int ResultSet);
    public delegate void MapDataFunction<T>(int resultSet, T oEntityRef, IDataReader reader);
    public delegate Task MapDataFunctionAsync<T>(int resultSet, T oEntityRef, IDataReader reader);
    public delegate void SqlNotificationOnSend(object sender, System.Data.SqlClient.SqlNotificationEventArgs e);

    public abstract class CommonSql : ISql
    {
        #region Private Variable
        private string _schema;
        private string _connectionString;
        private int _CommandTimeout = 30;
        protected List<SqlParameter> sqlParameters = new List<SqlParameter>();
        #endregion

        #region Public Properties
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public int CommandTimeout
        {
            get
            {
                return _CommandTimeout;
            }
            set
            {
                _CommandTimeout = value;
            }
        }
        
        public string Schema
        {
            get
            {
                if (MyConvert.ToString(_schema) != string.Empty)
                    return _schema + ".";
                else
                    return string.Empty;
            }
            set { _schema = value; }
        }
        #endregion

        #region Abstract Sync Methods

        #region Connection Methods
        protected abstract void OpenConnection();
        protected abstract void CloseConnection();
        protected abstract void SetCommanProperties(string commandText, CommandType commandType);

        #endregion

        #region Transaction Operation Methods
        internal abstract void ExecuteNonQueryWithoutTransaction(string commandText, CommandType commandType);
        internal abstract void ExecuteNonQueryWithSingleTransaction(string commandText, CommandType commandType);
        internal abstract void ExecuteNonQueryWithMultipleTransaction(string commandText, CommandType commandType);

        internal abstract object ExecuteScalarWithoutTransaction(string commandText, CommandType commandType);
        internal abstract object ExecuteScalarWithSingleTransaction(string commandText, CommandType commandType);
        internal abstract object ExecuteScalarWithMultipleTransaction(string commandText, CommandType commandType);

        public abstract void BeginTransaction();
        public abstract void CommitTransaction();
        public abstract void RollbackTransaction();

        #endregion

        #region Data Retrieval Methods
        public abstract DataTable ExecuteDataTable(string commandText, CommandType commandType);
        public abstract DataSet ExecuteDataSet(string commandText, CommandType commandType);

        public abstract IEnumerable<IDataReader> ExecuteEnumerableDataReader(string commandText, CommandType commandType);
        public abstract IDataReader ExecuteDataReader(string commandText, CommandType commandType);

        public abstract T ExecuteRecord<T>(string commandText, CommandType commandType);
        public abstract T ExecuteRecord<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        public abstract IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType);
        public abstract IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        public abstract List<T> ExecuteList<T>(string commandText, CommandType commandType);
        public abstract List<T> ExecuteList<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        public abstract T ExecuteResultSet<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunction<T> mapDataFunctionName);

        public abstract List<dynamic> ExecuteDyanamicList(string commandText, CommandType commandType);
        public abstract List<ResultSet> ExecuteDyanamicResultSet(string commandText, CommandType commandType);

        #endregion

        #endregion

        #region Abstract Async Methods
        #region Connection Methods
        protected abstract Task OpenConnectionAsync();
        protected abstract Task CloseConnectionAsync();

        #endregion

        #region Transaction Operation Methods
        internal abstract Task ExecuteNonQueryWithoutTransactionAsync(string commandText, CommandType commandType);
        internal abstract Task ExecuteNonQueryWithSingleTransactionAsync(string commandText, CommandType commandType);
        internal abstract Task ExecuteNonQueryWithMultipleTransactionAsync(string commandText, CommandType commandType);

        internal abstract Task<object> ExecuteScalarWithoutTransactionAsync(string commandText, CommandType commandType);
        internal abstract Task<object> ExecuteScalarWithSingleTransactionAsync(string commandText, CommandType commandType);
        internal abstract Task<object> ExecuteScalarWithMultipleTransactionAsync(string commandText, CommandType commandType);
        
        public abstract Task BeginTransactionAsync();
        public abstract Task CommitTransactionAsync();
        public abstract Task RollbackTransactionAsync();
        #endregion

        #region Data Retrieval Methods
        public abstract Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType);
        public abstract Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType);

        public abstract IAsyncEnumerable<IDataReader> ExecuteEnumerableDataReaderAsync(string commandText, CommandType commandType);
        public abstract Task<IDataReader> ExecuteDataReaderAsync(string commandText, CommandType commandType);

        public abstract Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType);
        public abstract Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        public abstract IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType);
        public abstract IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        public abstract Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType);
        public abstract Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        public abstract Task<T> ExecuteResultSetAsync<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunctionAsync<T> mapDataFunctionNameAsync);

        public abstract Task<List<dynamic>> ExecuteDyanamicListAsync(string commandText, CommandType commandType);
        public abstract Task<List<ResultSet>> ExecuteDyanamicResultSetAsync(string commandText, CommandType commandType);

        #endregion

        #endregion

        #region Public Methods

        #region Parameter Methods
        public void AddParameter(string parameterName, object parameterValue)
        {
            if (parameterValue == null)
                parameterValue = DBNull.Value;
            sqlParameters.Add(new SqlParameter(parameterName, parameterValue));
        }

        public void AddParameter(string parameterName, DbType dbType, ParameterDirection direction, object parameterValue)
        {
            if (parameterValue == null)
                parameterValue = DBNull.Value;
            sqlParameters.Add(new SqlParameter(parameterName, dbType, direction, parameterValue));
        }

        public void ClearParameter()
        {
            sqlParameters.Clear();
        }

        public Dictionary<string, object> GetParameters()
        {
            Dictionary<string, object> _dict = new Dictionary<string, object>();
            sqlParameters.ForEach(d => _dict.Add(d.ParameterName, d.ParameterValue));
            return _dict;
        }
        #endregion

        #region Mapper Methods
        public T MapData<T>(IDataReader reader)
        {
            object entity = Activator.CreateInstance(typeof(T));

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader[reader.GetName(i)] != DBNull.Value)
                {
                    PropertyInfo propertyInfo = entity.GetType().GetProperty(reader.GetName(i));
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(entity, reader[reader.GetName(i)], null);
                    }
                    else
                    {
                        PropertyInfo propertyInfoInsensitive = entity.GetType().GetProperty(reader.GetName(i), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (propertyInfoInsensitive != null)
                        {
                            propertyInfoInsensitive.SetValue(entity, reader[reader.GetName(i)], null);
                        }
                    }
                }
            }
            return (T)entity;
        }

        public Task<T> MapDataAsync<T>(IDataReader reader)
        {
            object entity = Activator.CreateInstance(typeof(T));

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader[reader.GetName(i)] != DBNull.Value)
                {
                    PropertyInfo propertyInfo = entity.GetType().GetProperty(reader.GetName(i));
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(entity, reader[reader.GetName(i)], null);
                    }
                    else
                    {
                        PropertyInfo propertyInfoInsensitive = entity.GetType().GetProperty(reader.GetName(i), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (propertyInfoInsensitive != null)
                        {
                            propertyInfoInsensitive.SetValue(entity, reader[reader.GetName(i)], null);
                        }
                    }
                }
            }
            return Task.FromResult(((T)entity));
        }

        #endregion

        #region Transaction Operation Methods
        public void ExecuteNonQuery(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None)
        {
            if (transactionType == TransactionType.Single)
                ExecuteNonQueryWithSingleTransaction(commandText, commandType);
            else if (transactionType == TransactionType.Multiple)
                ExecuteNonQueryWithMultipleTransaction(commandText, commandType);
            else
                ExecuteNonQueryWithoutTransaction(commandText, commandType);
        }

        public async Task ExecuteNonQueryAsync(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None)
        {
            if (transactionType == TransactionType.Single)
                await ExecuteNonQueryWithSingleTransactionAsync(commandText, commandType);
            else if (transactionType == TransactionType.Multiple)
                await ExecuteNonQueryWithMultipleTransactionAsync(commandText, commandType);
            else
                await ExecuteNonQueryWithoutTransactionAsync(commandText, commandType);
        }

        public object ExecuteScalar(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None)
        {
            if (transactionType == TransactionType.Single)
                return ExecuteScalarWithSingleTransaction(commandText, commandType);
            else if (transactionType == TransactionType.Multiple)
                return ExecuteScalarWithMultipleTransaction(commandText, commandType);
            else
                return ExecuteScalarWithoutTransaction(commandText, commandType);
        }

        public async Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None)
        {
            if (transactionType == TransactionType.Single)
                return await ExecuteScalarWithSingleTransactionAsync(commandText, commandType);
            else if (transactionType == TransactionType.Multiple)
                return await ExecuteScalarWithMultipleTransactionAsync(commandText, commandType);
            else
                return await ExecuteScalarWithoutTransactionAsync(commandText, commandType);
        }

        #endregion

        #endregion

        #region SqlNotification
        public abstract void SqlNotificationStart();
        public abstract void SqlNotificationEnd();
        public abstract void SqlNotificationDeregisterEvent(SqlNotificationOnSend sqlNotificationOnSend);
        public abstract List<T> SqlNotification<T>(string commandText, CommandType commandType, SqlNotificationOnSend sqlNotificationOnSend);
        public abstract List<T> SqlNotification<T>(string table, string columns, SqlNotificationOnSend sqlNotificationOnSend);
        
        #endregion
    }
}
