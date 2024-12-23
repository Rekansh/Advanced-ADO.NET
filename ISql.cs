﻿using System;
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
        void ExecuteNonQuery(string commandText, CommandType commandType);
        void ExecuteNonQueryWithTransaction(string commandText, CommandType commandType);
        object ExecuteScalar(string commandText, CommandType commandType);
        object ExecuteScalarWithTransaction(string commandText, CommandType commandType);

        DataTable ExecuteDataTable(string commandText, CommandType commandType);
        DataSet ExecuteDataSet(string commandText, CommandType commandType);
        IEnumerable<IDataReader> ExecuteEnumerableDataReader(string commandText, CommandType commandType);
        IDataReader ExecuteDataReader(string commandText, CommandType commandType);
        List<ResultSet> ExecuteDyanamicResultSet(string commandText, CommandType commandType);
        List<dynamic> ExecuteDyanamicList(string commandText, CommandType commandType);
        T ExecuteRecord<T>(string commandText, CommandType commandType);
        T ExecuteRecord<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType);
        IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);
        T ExecuteResultSet<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunction<T> mapDataFunctionName);

        List<T> ExecuteList<T>(string commandText, CommandType commandType);
        List<T> ExecuteList<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        #region multiple transaction handle
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        void ExecuteNonQueryMultipleTransaction(string commandText, CommandType commandType);
        object ExecuteScalarMultipleTransaction(string commandText, CommandType commandType);
        #endregion
        #endregion

        #region Abstract Async Methods
        Task ExecuteNonQueryAsync(string commandText, CommandType commandType);

        Task ExecuteNonQueryWithTransactionAsync(string commandText, CommandType commandType);

        Task<object> ExecuteScalarAsync(string commandText, CommandType commandType);

        Task<object> ExecuteScalarWithTransactionAsync(string commandText, CommandType commandType);

        Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType);

        Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType);

        IAsyncEnumerable<IDataReader> ExecuteEnumerableDataReaderAsync(string commandText, CommandType commandType);

        Task<IDataReader> ExecuteDataReaderAsync(string commandText, CommandType commandType);

        Task<List<ResultSet>> ExecuteDyanamicResultSetAsync(string commandText, CommandType commandType);

        Task<List<dynamic>> ExecuteDyanamicListAsync(string commandText, CommandType commandType);

        Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType);

        Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType);

        IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        Task<T> ExecuteResultSetAsync<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunctionAsync<T> mapDataFunctionName);

        Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType);

        Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName);

        #region multiple transaction handle
        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();

        Task ExecuteNonQueryMultipleTransactionAsync(string commandText, CommandType commandType);

        Task<object> ExecuteScalarMultipleTransactionAsync(string commandText, CommandType commandType);
        #endregion
        #endregion

        #region Public Methods
        void AddParameter(string parameterName, object parameterValue);
        void AddParameter(string parameterName, DbType dbType, ParameterDirection direction, object parameterValue);
        void ClearParameter();
        Dictionary<string, object> GetParameters();
        T MapDataDynamically<T>(IDataReader reader);
        Task<T> MapDataDynamicallyAsync<T>(IDataReader reader);
        #endregion

        #region SqlNotification
        void SqlNotificationStart();
        void SqlNotificationEnd();
        void SqlNotificationDeregisterEvent(SqlNotificationOnSend sqlNotificationOnSend);
        List<T> SqlNotification<T>(string commandText, CommandType commandType, SqlNotificationOnSend sqlNotificationOnSend);
        List<T> SqlNotification<T>(string table, string columns, SqlNotificationOnSend sqlNotificationOnSend);
        #endregion

    }
}
