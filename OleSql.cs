﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Dynamic;
using System.Threading.Tasks;

namespace AdvancedADO
{
    class OleSql : CommonSql
    {
        #region Private Variable 
        OleDbConnection sqlConn;
        OleDbCommand sqlCmd;
        OleDbTransaction sqlTran;
        #endregion

        #region Constructor
        public OleSql()
        {
        }

        public OleSql(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public OleSql(string connectionString, string schema)
        {
            ConnectionString = connectionString;
            Schema = schema;
        }
        #endregion

        #region Connection Methods
        protected override void OpenConnection()
        {
            try
            {
                if (sqlConn == null || sqlConn.State != ConnectionState.Open)
                {
                    sqlConn = new OleDbConnection();
                    sqlConn.ConnectionString = ConnectionString;
                    sqlConn.Open();
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                throw ex;
            }
        }

        protected override void CloseConnection()
        {
            try
            {
                if (sqlConn != null && sqlConn.State != ConnectionState.Closed)
                {
                    sqlConn.Close();
                }
            }
            finally
            {
                sqlConn.Dispose();
                sqlConn = null;
            }
        }

        protected override void SetCommanProperties(string commandText, CommandType commandType)
        {
            string Parameter = string.Empty;
            if (commandType != CommandType.Text)
                sqlCmd.CommandText = Schema + commandText;
            else
                sqlCmd.CommandText = commandText;
            sqlCmd.CommandType = commandType;
            sqlCmd.CommandTimeout = CommandTimeout;
            foreach (SqlParameter parameter in sqlParameters)
            {
                Parameter += "@" + parameter.ParameterName + " = '" + parameter.ParameterValue + ",";
                OleDbParameter sqlParameter = new OleDbParameter(parameter.ParameterName, parameter.ParameterValue);
                sqlParameter.DbType = parameter.DBType;
                sqlParameter.Direction = parameter.Direction;
                sqlCmd.Parameters.Add(sqlParameter);
            }
            sqlParameters.Clear();
        }
        #endregion

        #region Single Trasaction Methods
        public override void ExecuteNonQuery(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                sqlCmd.ExecuteNonQuery();
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override void ExecuteNonQueryWithTransaction(string commandText, CommandType commandType)
        {
            int i = 0;
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlTran = sqlConn.BeginTransaction();
                sqlCmd.Connection = sqlConn;
                sqlCmd.Transaction = sqlTran;
                i = 1;
                sqlCmd.ExecuteNonQuery();
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                if (i == 1)
                    sqlTran.Rollback();
                throw ex;
            }
            finally
            {
                if (sqlTran != null)
                    sqlTran.Dispose();
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override object ExecuteScalar(string commandText, CommandType commandType)
        {
            object result = null;
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                result = sqlCmd.ExecuteScalar();
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return result;
        }

        public override object ExecuteScalarWithTransaction(string commandText, CommandType commandType)
        {
            object result = null;
            int i = 0;
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlTran = sqlConn.BeginTransaction();
                sqlCmd.Connection = sqlConn;
                sqlCmd.Transaction = sqlTran;
                i = 1;
                result = sqlCmd.ExecuteScalar();
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                if (i == 1)
                    sqlTran.Rollback();
                throw ex;
            }
            finally
            {
                if (sqlTran != null)
                    sqlTran.Dispose();
                sqlCmd.Dispose();
                CloseConnection();
            }
            return result;
        }

        public override DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            DataTable dtReturn = new DataTable();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                OleDbDataAdapter sqlAdp = new OleDbDataAdapter(sqlCmd);
                sqlAdp.Fill(dtReturn);
                sqlAdp.Dispose();
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return dtReturn;
        }

        public override DataSet ExecuteDataSet(string commandText, CommandType commandType)
        {
            DataSet dsReturn = new DataSet();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                OleDbDataAdapter sqlAdp = new OleDbDataAdapter(sqlCmd);
                sqlAdp.Fill(dsReturn);
                sqlAdp.Dispose();
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return dsReturn;
        }

        public override IEnumerable<IDataReader> ExecuteEnumerableDataReader(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                        yield return reader;
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override IDataReader ExecuteDataReader(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            finally
            {
                sqlCmd.Dispose();
            }
        }

        public override List<ResultSet> ExecuteDyanamicResultSet(string commandText, CommandType commandType)
        {
            List<ResultSet> Results = new List<ResultSet>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader())
                {
                    int index = 0;
                    do
                    {
                        ResultSet Result = new ResultSet();
                        Result.ResultIndex = index;
                        while (reader.Read())
                        {
                            IDictionary<string, object> expando = new ExpandoObject();
                            for (int i = 0; i < reader.FieldCount; i++)
                                expando.Add(reader.GetName(i), reader.GetValue(i));
                            Result.ResultData.Add(expando);
                        }
                        index++;
                        Results.Add(Result);
                    }
                    while (reader.NextResult());
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return Results;
        }

        public override List<dynamic> ExecuteDyanamicList(string commandText, CommandType commandType)
        {
            List<dynamic> Results = new List<dynamic>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        IDictionary<string, object> expando = new ExpandoObject();
                        for (int i = 0; i < reader.FieldCount; i++)
                            expando.Add(reader.GetName(i), reader.GetValue(i));
                        Results.Add(expando);
                    }
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return Results;
        }

        public override T ExecuteRecord<T>(string commandText, CommandType commandType)
        {
            object entity = Activator.CreateInstance(typeof(T));
            try
            {
                int i = 0;
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read() && i < 1)
                    {
                        entity = MapDataDynamically<T>(reader);
                        i++;
                    }
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return (T)entity;
        }

        public override T ExecuteRecord<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName)
        {
            object entity = Activator.CreateInstance(typeof(T));
            try
            {
                int i = 0;
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read() && i < 1)
                    {
                        entity = mapDataFunctionName(reader);
                        i++;
                    }
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return (T)entity;
        }

        public override IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                        yield return MapDataDynamically<T>(reader);
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override IEnumerable<T> ExecuteEnumerable<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                        yield return mapDataFunctionName(reader);
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override T ExecuteResultSet<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunction<T> mapDataFunctionName)
        {
            try
            {
                var entity = Activator.CreateInstance<T>();
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader())
                {
                    for (int resultSet = 0; resultSet < resultSetCount; resultSet++)
                    {
                        while (reader.Read())
                            mapDataFunctionName(resultSet, entity, reader);
                        reader.NextResult();
                    }
                }
                return entity;
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override List<T> ExecuteList<T>(string commandText, CommandType commandType)
        {
            List<T> oLists = new List<T>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                        oLists.Add(MapDataDynamically<T>(reader));
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return oLists;
        }

        public override List<T> ExecuteList<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName)
        {
            List<T> oLists = new List<T>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                OpenConnection();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                        oLists.Add(mapDataFunctionName(reader));
                }
            }
            finally
            {
                sqlCmd.Dispose();
                CloseConnection();
            }
            return oLists;
        }

        #endregion

        #region Multiple Transaction Methods
        public override void BeginTransaction()
        {
            OpenConnection();
            sqlTran = sqlConn.BeginTransaction();
            sqlCmd = new OleDbCommand();
            sqlCmd.Connection = sqlConn;
            sqlCmd.Transaction = sqlTran;
        }

        public override void CommitTransaction()
        {
            try
            {
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw ex;
            }
            finally
            {
                sqlTran.Dispose();
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override void RollbackTransaction()
        {
            try
            {
                sqlTran.Rollback();
            }
            finally
            {
                sqlTran.Dispose();
                sqlCmd.Dispose();
                CloseConnection();
            }
        }

        public override void ExecuteNonQueryMultipleTransaction(string commandText, CommandType commandType)
        {
            SetCommanProperties(commandText, commandType);
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Parameters.Clear();
        }

        public override object ExecuteScalarMultipleTransaction(string commandText, CommandType commandType)
        {
            object result = null;
            SetCommanProperties(commandText, commandType);
            result = sqlCmd.ExecuteScalar();
            sqlCmd.Parameters.Clear();
            return result;
        }
        #endregion

        #region Asynch Methods
        #region Connection Methods
        protected override async Task OpenConnectionAsync()
        {
            try
            {
                if (sqlConn == null || sqlConn.State != ConnectionState.Open)
                {
                    sqlConn = new OleDbConnection();
                    sqlConn.ConnectionString = ConnectionString;
                    await sqlConn.OpenAsync();
                }
            }
            catch (Exception ex)
            {
                await sqlConn.CloseAsync();
                throw ex;
            }
        }

        protected override async Task CloseConnectionAsync()
        {
            try
            {
                if (sqlConn != null && sqlConn.State != ConnectionState.Closed)
                {
                    await sqlConn.CloseAsync();
                }
            }
            finally
            {
                await sqlConn.DisposeAsync();
                sqlConn = null;
            }
        }

        #endregion

        #region Single Trasaction Methods
        public override async Task ExecuteNonQueryAsync(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                await sqlCmd.ExecuteNonQueryAsync();
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async Task ExecuteNonQueryWithTransactionAsync(string commandText, CommandType commandType)
        {
            int i = 0;
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlTran = (OleDbTransaction)await sqlConn.BeginTransactionAsync();
                sqlCmd.Connection = sqlConn;
                sqlCmd.Transaction = sqlTran;
                i = 1;
                await sqlCmd.ExecuteNonQueryAsync();
                await sqlTran.CommitAsync();
            }
            catch (Exception ex)
            {
                if (i == 1)
                    await sqlTran.RollbackAsync();
                throw ex;
            }
            finally
            {
                if (sqlTran != null)
                    await sqlTran.DisposeAsync();
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async Task<object> ExecuteScalarAsync(string commandText, CommandType commandType)
        {
            object result = null;
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                result = await sqlCmd.ExecuteScalarAsync();
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return result;
        }

        public override async Task<object> ExecuteScalarWithTransactionAsync(string commandText, CommandType commandType)
        {
            object result = null;
            int i = 0;
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlTran = (OleDbTransaction)await sqlConn.BeginTransactionAsync();
                sqlCmd.Connection = sqlConn;
                sqlCmd.Transaction = sqlTran;
                i = 1;
                result = await sqlCmd.ExecuteScalarAsync();
                await sqlTran.CommitAsync();
            }
            catch (Exception ex)
            {
                if (i == 1)
                    await sqlTran.RollbackAsync();
                throw ex;
            }
            finally
            {
                if (sqlTran != null)
                    await sqlTran.DisposeAsync();
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return result;
        }

        public override async Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType)
        {
            DataTable dtReturn = new DataTable();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                OleDbDataAdapter sqlAdp = new OleDbDataAdapter(sqlCmd);
                sqlAdp.Fill(dtReturn);
                sqlAdp.Dispose();
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return dtReturn;
        }

        public override async Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType)
        {
            DataSet dsReturn = new DataSet();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                OleDbDataAdapter sqlAdp = new OleDbDataAdapter(sqlCmd);
                sqlAdp.Fill(dsReturn);
                sqlAdp.Dispose();
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return dsReturn;
        }

        public override async IAsyncEnumerable<IDataReader> ExecuteEnumerableDataReaderAsync(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                        yield return reader;
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async Task<IDataReader> ExecuteDataReaderAsync(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                return await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            finally
            {
                await sqlCmd.DisposeAsync();
            }
        }

        public override async Task<List<ResultSet>> ExecuteDyanamicResultSetAsync(string commandText, CommandType commandType)
        {
            List<ResultSet> Results = new List<ResultSet>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync())
                {
                    int index = 0;
                    do
                    {
                        ResultSet Result = new ResultSet();
                        Result.ResultIndex = index;
                        while (reader.Read())
                        {
                            IDictionary<string, object> expando = new ExpandoObject();
                            for (int i = 0; i < reader.FieldCount; i++)
                                expando.Add(reader.GetName(i), reader.GetValue(i));
                            Result.ResultData.Add(expando);
                        }
                        index++;
                        Results.Add(Result);
                    }
                    while (reader.NextResult());
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return Results;
        }

        public override async Task<List<dynamic>> ExecuteDyanamicListAsync(string commandText, CommandType commandType)
        {
            List<dynamic> Results = new List<dynamic>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        IDictionary<string, object> expando = new ExpandoObject();
                        for (int i = 0; i < reader.FieldCount; i++)
                            expando.Add(reader.GetName(i), reader.GetValue(i));
                        Results.Add(expando);
                    }
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return Results;
        }

        public override async Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType)
        {
            object entity = Activator.CreateInstance(typeof(T));
            try
            {
                int i = 0;
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read() && i < 1)
                    {
                        entity = MapDataDynamicallyAsync<T>(reader);
                        i++;
                    }
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return (T)entity;
        }

        public override async Task<T> ExecuteRecordAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName)
        {
            object entity = Activator.CreateInstance(typeof(T));
            try
            {
                int i = 0;
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read() && i < 1)
                    {
                        entity = mapDataFunctionName(reader);
                        i++;
                    }
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return (T)entity;
        }

        public override async IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                        yield return await MapDataDynamicallyAsync<T>(reader);
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async IAsyncEnumerable<T> ExecuteEnumerableAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName)
        {
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        yield return mapDataFunctionName(reader);
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async Task<T> ExecuteResultSetAsync<T>(string commandText, CommandType commandType, int resultSetCount, MapDataFunctionAsync<T> mapDataFunctionName)
        {
            try
            {
                var entity = Activator.CreateInstance<T>();
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync())
                {
                    for (int resultSet = 0; resultSet < resultSetCount; resultSet++)
                    {
                        while (reader.Read())
                            await mapDataFunctionName(resultSet, entity, reader);
                        reader.NextResult();
                    }
                }
                return entity;
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType)
        {
            List<T> oLists = new List<T>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                        oLists.Add(await MapDataDynamicallyAsync<T>(reader));
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return oLists;
        }

        public override async Task<List<T>> ExecuteListAsync<T>(string commandText, CommandType commandType, Func<IDataReader, T> mapDataFunctionName)
        {
            List<T> oLists = new List<T>();
            try
            {
                sqlCmd = new OleDbCommand();
                SetCommanProperties(commandText, commandType);
                await OpenConnectionAsync();
                sqlCmd.Connection = sqlConn;
                using (IDataReader reader = await sqlCmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        oLists.Add(mapDataFunctionName(reader));
                }
            }
            finally
            {
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
            return oLists;
        }

        #endregion

        #region Multiple Transaction Methods
        public override async Task BeginTransactionAsync()
        {
            await OpenConnectionAsync();
            sqlTran = (OleDbTransaction)await sqlConn.BeginTransactionAsync();
            sqlCmd = new OleDbCommand();
            sqlCmd.Connection = sqlConn;
            sqlCmd.Transaction = sqlTran;
        }

        public override async Task CommitTransactionAsync()
        {
            try
            {
                await sqlTran.CommitAsync();
            }
            catch (Exception ex)
            {
                await RollbackTransactionAsync();
                throw ex;
            }
            finally
            {
                await sqlTran.DisposeAsync();
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async Task RollbackTransactionAsync()
        {
            try
            {
                await sqlTran.RollbackAsync();
            }
            finally
            {
                await sqlTran.DisposeAsync();
                await sqlCmd.DisposeAsync();
                await CloseConnectionAsync();
            }
        }

        public override async Task ExecuteNonQueryMultipleTransactionAsync(string commandText, CommandType commandType)
        {
            SetCommanProperties(commandText, commandType);
            await sqlCmd.ExecuteNonQueryAsync();
            sqlCmd.Parameters.Clear();
        }

        public override async Task<object> ExecuteScalarMultipleTransactionAsync(string commandText, CommandType commandType)
        {
            object result = null;
            SetCommanProperties(commandText, commandType);
            result = await sqlCmd.ExecuteScalarAsync();
            sqlCmd.Parameters.Clear();
            return result;
        }
        #endregion
        #endregion

        #region SqlNotification
        public override void SqlNotificationStart()
        {
            throw new Exception(string.Format("Sql notification is not supporting for oledb database."));
        }

        public override void SqlNotificationEnd()
        {
            throw new Exception(string.Format("Sql notification is not supporting for oledb database."));
        }

        public override void SqlNotificationDeregisterEvent(SqlNotificationOnSend sqlNotificationOnSend)
        {
            throw new Exception(string.Format("Sql notification is not supporting for oledb database."));
        }

        public override List<T> SqlNotification<T>(string commandText, CommandType commandType, SqlNotificationOnSend sqlNotificationOnSend)
        {
            throw new Exception(string.Format("Sql notification is not supporting for oledb database."));
        }

        public override List<T> SqlNotification<T>(string table, string columns, SqlNotificationOnSend sqlNotificationOnSend)
        {
            throw new Exception(string.Format("Sql notification is not supporting for oledb database."));
        }
        #endregion
    }
}
