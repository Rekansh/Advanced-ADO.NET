using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdvancedADO
{
    public interface ISql
    {
        int CommandTimeout { get; set; }
        string Schema { get; set; }

        #region Abstract Sync Methods

        #region Transaction Operation Methods
        void ExecuteNonQuery(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None);
        object ExecuteScalar(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None);

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        #endregion

        #region Data Retrieval Methods
        DataTable ExecuteDataTable(string commandText, CommandType commandType);
        DataSet ExecuteDataSet(string commandText, CommandType commandType);

        IDataReader ExecuteDataReader(string commandText, CommandType commandType);
        IEnumerable<IDataReader> ExecuteEnumerableDataReader(string commandText, CommandType commandType);

        T ExecuteRecord<T>(string commandText, CommandType commandType);
        T ExecuteRecord<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType);
        IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        List<T> ExecuteList<T>(string commandText, CommandType commandType);
        List<T> ExecuteList<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        T ExecuteResultSet<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunction<T> mapDataFunctionName);

        List<dynamic> ExecuteDyanamicList(string commandText, CommandType commandType);
        List<ResultSet> ExecuteDyanamicResultSet(string commandText, CommandType commandType);

        #endregion

        #endregion

        #region Abstract Async Methods
        
        #region Transaction Operation Methods
        Task ExecuteNonQueryAsync(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None);
        Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, TransactionType transactionType = TransactionType.None);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        #endregion

        #region Data Retrieval Methods
        Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType);
        Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType);

        Task<IDataReader> ExecuteDataReaderAsync(string commandText, CommandType commandType);
        IAsyncEnumerable<IDataReader> ExecuteEnumerableDataReaderAsync(string commandText, CommandType commandType);

        Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType);
        Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType);
        IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType);
        Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        Task<T> ExecuteResultSetAsync<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunctionAsync<T> mapDataFunctionName);

        Task<List<dynamic>> ExecuteDyanamicListAsync(string commandText, CommandType commandType);
        Task<List<ResultSet>> ExecuteDyanamicResultSetAsync(string commandText, CommandType commandType);

        #endregion

        #endregion

        #region Public Methods

        #region Parameter Methods
        void AddParameter(string parameterName, object parameterValue);
        void AddParameter(string parameterName, DbType dbType, ParameterDirection direction, object parameterValue);
        void ClearParameter();
        Dictionary<string, object> GetParameters();

        #endregion

        #region Mapper Methods
        T MapData<T>(IDataReader reader);
        Task<T> MapDataAsync<T>(IDataReader reader);

        #endregion

        #endregion

        #region SqlNotification
        void SqlNotificationStart();
        void SqlNotificationEnd();
        void SqlNotificationDeregisterEvent(SqlNotificationOnSend sqlNotificationOnSend);
        List<T> SqlNotification<T>(string commandText, CommandType commandType, SqlNotificationOnSend sqlNotificationOnSend);
        List<T> SqlNotification<T>(string table, string columns, SqlNotificationOnSend sqlNotificationOnSend);
        #endregion

    }

    public enum TransactionType
    {
        None = 0,
        Single,
        Multiple
    }
}
