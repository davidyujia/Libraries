using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace davidyujia.Unknow
{

    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class ColumnNameAttribute : System.Attribute
    {
        public string ColumnName { get; internal set; }

        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    /// <summary>
    /// DAO擴充功能
    /// </summary>
    public static class DaoExtension
    {
        /// <summary>
        /// 取得DataReader取回的欄位
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static HashSet<string> GetDataReaderColumnNames(IDataRecord reader)
        {
            var columnNames = new HashSet<string>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            return columnNames;
        }

        /// <summary>
        /// 取得需要對應的欄位
        /// </summary>
        /// <typeparam name="T">ORM</typeparam>
        /// <param name="dataReaderColumnNames">The data reader column names.</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetExistColumnNames<T>(ICollection<string> dataReaderColumnNames)
        {
            var list = new Dictionary<string, string>();
            foreach (var obj in typeof(T).GetProperties())
            {
                var columnName = obj.Name;
                var attribute = (ColumnNameAttribute)obj
                    .GetCustomAttributes(typeof(ColumnNameAttribute), false)
                    .FirstOrDefault();

                if (attribute != null)
                {
                    columnName = attribute.ColumnName;
                }

                if (!dataReaderColumnNames.Contains(columnName))
                {
                    continue;
                }
                list.Add(obj.Name, columnName);
            }

            return list;
        }

        /// <summary>
        /// 執行並取回查詢結果
        /// </summary>
        /// <typeparam name="T">ORM</typeparam>
        /// <param name="command">DB Command</param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteResult<T>(this IDbCommand command) where T : new()
        {
            var reader = command.ExecuteReader();

            var list = new List<T>();

            var dataReaderColumnNames = GetDataReaderColumnNames(reader);

            var columnNames = GetExistColumnNames<T>(dataReaderColumnNames);

            var type = typeof(T);

            while (reader.Read())
            {
                var obj = new T();
                foreach (var columnName in columnNames)
                {
                    var property = type.GetProperty(columnName.Key);

                    if (property == null)
                    {
                        continue;
                    }

                    if (reader[columnName.Value] == DBNull.Value)
                    {
                        continue;
                    }

                    var value = reader[columnName.Value];

                    property.SetValue(obj, value);
                }

                list.Add(obj);
            }

            reader.Close();

            return list;
        }

        /// <summary>
        /// 取得Model需要轉換的欄位
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseSqlColumns(this Type type)
        {
            var list = new Dictionary<string, string>();

            foreach (var property in type.GetProperties())
            {
                var columnName = property.Name;
                var attribute = (ColumnNameAttribute)property
                    .GetCustomAttributes(typeof(ColumnNameAttribute), false)
                    .FirstOrDefault();

                if (attribute != null)
                {
                    columnName = attribute.ColumnName;
                }

                list.Add(columnName, property.Name);
            }

            return list;
        }
    }
}