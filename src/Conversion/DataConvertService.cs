using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Enjaxel.TextParser;

namespace Enjaxel.Conversion
{
    /// <summary>
    /// Dataに関する変換機能を提供します
    /// </summary>
    public static class DataConvertService
    {
        #region DataSetからT型Entityを生成
        /// <summary>
        /// DataRow => T型Entity へ変換します（内部向け）
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="dr"> DataRow </param>
        /// <param name="maps"> マッピングオブジェクトのリスト </param>
        /// <returns> T型Entity </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="TargetException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        internal static T AsEntity<T>(DataRow dr, List<MappingObject> maps)
            where T : new()
        {
            // 戻り値用にT型インスタンスを作成
            var resT = Activator.CreateInstance<T>();

            foreach (var map in maps)
            {
                // Property関連変数
                PropertyInfo prop = typeof(T).GetRuntimeProperty(map.Name);
                TypeCode p_type_code = Type.GetTypeCode(map.ObjectType);

                object db_value = dr[map.MappingKeyName];

                if (db_value != DBNull.Value)
                {
                    // カラムの型を取得
                    Type column_type = dr.Table.Columns[map.MappingKeyName].DataType;

                    // カラムの型がNullableなら
                    if (column_type.Name.Contains("Nullable"))
                    {
                        // GetUnderlyingTypeで元の型を取得
                        column_type = Nullable.GetUnderlyingType(column_type);
                    }

                    TypeCode c_type_code = Type.GetTypeCode(column_type);

                    // ↓EntityのPropertyにDataRowの値をセットする↓
                    // Propertyの型がStringだが、カラムの型がString以外の時
                    if (p_type_code == TypeCode.String &&
                        c_type_code != TypeCode.String)
                    {
                        // String型に変換して値を詰める
                        prop.SetValue(resT, dr.Field<string>(map.MappingKeyName));
                    }
                    else if (p_type_code == c_type_code)
                    {
                        // Propertyの型とカラムの型が同一なら無変換
                        prop.SetValue(resT, db_value);
                    }
                    else
                    {
                        // Propertyの型とカラムの型が違うなら変換する
                        prop.SetValue(resT, db_value.ConvertValue(p_type_code));
                    }
                }
                else
                {
                    // DataRowの中から値を取得して、DBNullの時で
                    // Propertyの型がStringのときだけString.Emptyを書き込み
                    if (p_type_code == TypeCode.String)
                    {
                        prop.SetValue(resT, string.Empty);
                    }
                    else if (map.IsNullable)
                    {
                        prop.SetValue(resT, null);
                    }

                    // 上記の2つ以外のときは何も書き込まずにインスタンスの初期値に任せる
                    // 型の初期値を書き込むとDBからの流入値かここで設定した値かわからなくなるため
                }
            }

            return resT;
        }

        /// <summary>
        /// DataRow => T型Entity へ変換します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="dr"> DataRow </param>
        /// <returns> T型Entity </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="TargetException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="FailedMappingException"></exception>
        public static T AsEntity<T>(this DataRow dr)
            where T : new()
        {
            // DataRow(DataTableを一時的に引いて)からカラム名のリストを取得
            var columnNames = dr.Table.Columns.Cast<DataColumn>()
                                              .Select(x => x.ColumnName);
            // Entityから全てのPropertyを取得
            PropertyInfo[] properties = typeof(T).GetProperties();

            List<MappingObject> maps = GetMappingObjects(properties, columnNames);

            return AsEntity<T>(dr, maps);
        }

        /// <summary>
        /// DataTable => IEnumerable&lt;T型Entity&gt; へ変換します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="dt"> DBから取得したDataTable </param>
        /// <returns> Entityのリスト </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="TargetException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="FailedMappingException"></exception>
        public static IEnumerable<T> AsEnumerable<T>(this DataTable dt)
            where T : new()
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                yield return Activator.CreateInstance<T>();
                yield break;
            }

            // DataRow(DataTableを一時的に引いて)からカラム名のリストを取得
            var columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName);
            // Entityから全てのPropertyを取得
            PropertyInfo[] properties = typeof(T).GetProperties();

            List<MappingObject> maps = GetMappingObjects(properties, columnNames);

            // Rowの全行をT型Entityに変換
            foreach (DataRow row in dt.Rows)
            {
                yield return AsEntity<T>(row, maps);
            }
        }
        #endregion

        #region T型EntityからDataSetを生成
        /// <summary>
        /// T型Entity => DataRow へ変換します（内部向け）
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="entity"> Entity </param>
        /// <param name="maps"> MappingObjectのリスト </param>
        /// <param name="dt"> DataTable </param>
        /// <returns> DataRow </returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static DataRow AsDataRow<T>
            (T entity, List<MappingObject> maps, ref DataTable dt) where T : new()
        {
            DataRow row = dt.NewRow();

            foreach (var map in maps)
            {
                PropertyInfo prop = typeof(T).GetRuntimeProperty(map.Name);
                row[map.MappingKeyName] = prop.GetValue(entity) ?? DBNull.Value;
            }

            return row;
        }

        /// <summary>
        /// T型Entity => DataRow へ変換します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="entity"> Entity </param>
        /// <param name="dt"> DataTable </param>
        /// <returns> DataRow </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        public static DataRow AsDataRow<T>(this T entity, ref DataTable dt)
            where T : new()
        {
            DataRow row = dt.NewRow();
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                // Propertyに紐付いた属性値を取得
                ColumnAttribute column_atrb =
                    prop.GetCustomAttribute<ColumnAttribute>();

                // カラム属性値がNullでない場合
                if (column_atrb != null)
                {
                    // カラム属性値のカラムにデータを登録
                    row[column_atrb.Name] = prop.GetValue(entity) ?? DBNull.Value;
                }
                else
                {
                    // そうでない場合はProperty名のカラムにデータを登録
                    row[prop.Name] = prop.GetValue(entity) ?? DBNull.Value;
                }
            }

            return row;
        }

        /// <summary>
        /// Propertyの属性からDataColumunの配列を作成します
        /// </summary>
        /// <param name="properties"> Property情報 </param>
        /// <returns> DataColumn </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        public static DataColumn[] AsDataColumns(this PropertyInfo[] properties)
        {
            // ローカル変数の準備
            var columns = new DataColumn[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                // Propertyに紐付いた属性値を取得
                ColumnAttribute column_atrb =
                    properties[i].GetCustomAttribute<ColumnAttribute>();

                // カラム属性値がNullでない場合、カラム属性値をカラム名にする
                // そうでない場合はProperty名をカラム名にする
                var col = new DataColumn(column_atrb?.Name ?? properties[i].Name,
                                         properties[i].PropertyType);

                // 配列に詰め込み
                columns[i] = col;
            }

            return columns;
        }

        /// <summary>
        /// T型EntityからDataColumunの配列を作成します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="entity"> T型Entity </param>
        /// <returns> DataColumn </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        public static DataColumn[] AsDataColumns<T>(this T entity)
            where T : new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.AsDataColumns();
        }

        /// <summary>
        /// IEnumerable&lt;T型Entity&gt; => DataTable へ変換します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="tList"> Entityのリスト </param>
        /// <returns> DataTable </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="ConstraintException"></exception>
        /// <exception cref="NoNullAllowedException"></exception>
        /// <exception cref="FailedMappingException"></exception>
        public static DataTable AsDataTable<T>(this IEnumerable<T> tList)
            where T : new()
        {
            var table = new DataTable();
            PropertyInfo[] properties = typeof(T).GetProperties();

            // EntityのListに何も入っていなければ終了
            if (tList.Count() == 0)
            {
                return table;
            }

            // TableにDataColumnをセット
            table.Columns.AddRange(properties.AsDataColumns());

            var column_names = table.Columns.Cast<DataColumn>()
                                            .Select(x => x.ColumnName);
            List<MappingObject> maps = GetMappingObjects(properties, column_names);

            foreach (T item in tList)
            {
                // DataTableにDataRowを追加
                table.Rows.Add(AsDataRow(item, maps, ref table));
            }

            return table;
        }
        #endregion

        #region T型EntityからSqlParameterを生成
        /// <summary>
        /// T型EntityからSqlParameterの配列を作成します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="entity"> T型Entity </param>
        /// <returns> SqlCommandのバインド向けSqlParameter配列 </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        public static SqlParameter[] AsSqlParameters<T>(this T entity)
            where T : new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var resPrms = new SqlParameter[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                // Propertyに紐付いた属性値を取得
                ColumnAttribute column_atrb =
                    properties[i].GetCustomAttribute<ColumnAttribute>();

                object value = new object();
                Type v_type = properties[i].PropertyType;
                dynamic prop_value = properties[i].GetValue(entity);

                // Nullableの型の場合
                if (v_type.Name.Contains("Nullable"))
                {
                    // GetUnderlyingTypeで元の型を取得
                    v_type = Nullable.GetUnderlyingType(v_type);
                    value = prop_value.HasValue ? prop_value : DBNull.Value;
                }
                else
                {
                    value = prop_value;
                }

                var param = new SqlParameter()
                {
                    ParameterName = "@" + column_atrb?.Name ?? properties[i].Name,
                    Value = value
                };

                // 配列に詰め込み
                resPrms[i] = param;
            }

            return resPrms;
        }
        #endregion

        #region privateメソッド
        /// <summary>
        /// Property情報とカラム名の一覧からMappingObjectのリストを生成します
        /// </summary>
        /// <param name="properties"> Property情報 </param>
        /// <param name="columnNames"> カラム名の一覧 </param>
        /// <returns> MappingObjectのリスト </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="FailedMappingException"></exception>
        private static List<MappingObject> GetMappingObjects
            (PropertyInfo[] properties, IEnumerable<string> columnNames)
        {
            var res_maps = new List<MappingObject>(properties.Length);

            foreach (var prop in properties)
            {
                var map = new MappingObject();

                // Propertyに紐付いた属性値を取得
                ColumnAttribute column_atrb =
                    prop.GetCustomAttribute<ColumnAttribute>();

                // Nullableの型の時
                if (prop.PropertyType.Name.Contains("Nullable"))
                {
                    // GetUnderlyingTypeから型を取得する
                    map.ObjectType = Nullable.GetUnderlyingType(prop.PropertyType);

                    // Nullableフラグを立てる
                    map.IsNullable = true;
                }
                else
                {
                    // Nullableの型でないときは普通にPropertyTypeから取得
                    map.ObjectType = prop.PropertyType;
                }

                if (column_atrb != null && columnNames.Contains(column_atrb.Name))
                {
                    // カラム属性値が設定されていて、カラム属性値に対応するカラムが存在する場合
                    // カラム属性値の値を採用する
                    map.MappingKeyName = column_atrb.Name;
                    map.Name = prop.Name;
                }
                else if (columnNames.Contains(prop.Name))
                {
                    // それ以外の場合で、Property名がカラム名の中にあるときはProperty名を採用
                    map.MappingKeyName = prop.Name;
                    map.Name = prop.Name;
                }
                else
                {
                    // どちらもなければエラー
                    throw new FailedMappingException
                        ("DataTableのカラム名に対してマッピングできる対象のプロパティが" +
                         "存在しません");
                }

                res_maps.Add(map);
            }

            return res_maps;
        }
        #endregion
    }
}
