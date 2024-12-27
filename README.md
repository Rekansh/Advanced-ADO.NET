# Advanced ADO.NET

## Overview
The Advanced ADO.NET is a comprehensive library to help performe simpless database operation like ExecuteNonQuery, ExecuteScaler, ExecuteList, ExecuteRecord, ExecuteRecordSet, ExecuteDynamicList, ExecuteDyanamicResultSet etc on any database like MS SQL Server, PostgreSQL, Oracel etc using .NET.

## Features
- **ExecuteNonQuery / ExecuteNonQueryAsync**: This function use to perform insert, update, delete operation on database and return nothing.
- **ExecuteScalar / ExecuteScalarAsync**: This function use to get single value like count, min, max, sum etc from database return as single value.
- **ExecuteDataTable / ExecuteDataTableAsync**: This function get signle table data from database and return as DataTable.
- **ExecuteDataSet / ExecuteDataSetAsync**: This function get multiple tables data from database and return as DataSet.
- **ExecuteEnumerableDataReader / ExecuteEnumerableDataReaderAsync**: This function get single table data from database and return as Enumerable DataReader so make lazy data loading.
- **ExecuteDataReader / ExecuteDataReaderAsync**: This function retrun data as DataReader from database.
- **ExecuteRecord / ExecuteRecordAsync**: This function return single record from database.
- **ExecuteEnumerable<T> / ExecuteEnumerableAsync<T>**: This function get data from single table and return as Enumrable of Entity so make lazy data loading.
- **ExecuteList<T> / ExecuteListAsync<T>**: This function get data from single table and return as List of Entity.
- **ExecuteResultSet / ExecuteResultSetAsync**: This function get data from multiple tables and return as multiple List of Entity.
- **ExecuteDyanamicList / ExecuteDyanamicListAsync**:This function get data from single table and return as List of dynamic so no need to make entity class. This is useful for get data from database but not predicatable data.
- **ExecuteDyanamicResultSet / ExecuteDyanamicResultSetAsync**:This function get data from multiple table and return as List of multiple dynamic result set so no need to make entity class. This is useful for get data from database but not predicatable data.
- **BeginTransaction / BeginTransactionAsync**:This function use to begin transaction for performe multiple transaction so if all trasaction completed successfully then we can commit transaction else rollback transaction.
- **CommitTransaction / CommitTransactionAsync**:This function use to commit transaction for performe multiple transaction so if all trasaction completed successfully.
- **RollbackTransaction / RollbackTransactionAsync**:This function use to rollback transaction for performe multiple transaction so if any trasaction failed.

## Technologies Used
- .NET, C#, ADO.NET

## Installation

## Usage
1. Navigate to `http://localhost:4400` in your web browser.
2. Register a new account or log in with an existing account.
3. Start adding your stock stransactions.
4. Monitor your portfolio performance and view detailed analytics.

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request with your changes.

## License
This project is licensed under the MIT License.

## Contact
For any questions or suggestions, please contact [rekansh.patel@gmail.com](mailto:rekansh.patel@gmail.com).

---

Feel free to modify this template according to your project's specific requirements and details. If you need any more specific information included, just let me know! 📈💼🚀
