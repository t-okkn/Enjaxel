using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Enjaxel.Conversion
{
    /// <summary>
    /// DataBaseから取得した値に関する変換機能を提供します
    /// </summary>
    public static class DBConversion
    {
        /// <summary>
        /// DataTable => List(Of T型Entity) への変換メソッド
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="dt"> DBから取得したDataTable </param>
        /// <returns> Entityのリスト </returns>
        public static List<T> ToEntityList<T>(this DataTable dt)
            where T : new()
        {
            var resTList = new List<T>();

            try
            {
                // Rowの全行をT型Entityに変換
                foreach (DataRow row in dt.AsEnumerable())
                    resTList.Add(row.ToEntity<T>());
            }
            catch (Exception)
            {
                throw;
            }

            return resTList;
        }

        /// <summary>
        /// DataRow => T型Entity への変換メソッド
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="dr"> DataRow </param>
        /// <returns> T型Entity </returns>
        public static T ToEntity<T>(this DataRow dr)
            where T : new()
        {
            var resT = default(T);
            var columnNames = new List<string>();
            bool nullable_flag = false;

            try
            {
                // 戻り値用にT型インスタンスを作成
                resT = Activator.CreateInstance<T>();

                // DataRow(DataTableを一時的に引いて)からカラム名のリストを取得
                columnNames = dr.Table.Columns.Cast<DataColumn>()
                                              .Select(x => x.ColumnName)
                                              .ToList();

                // Entityから全てのPropertyを取得
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo prop in properties)
                {
                    // Propertyに紐付いた属性値を取得
                    ColumnAttribute column_atrb =
                        prop.GetCustomAttribute<ColumnAttribute>();

                    // PropertyのTypeCodeを入れるためのローカル変数
                    TypeCode p_type_code = TypeCode.Empty;

                    // Nullableの型の時
                    if (prop.PropertyType.Name.Contains("Nullable"))
                    {
                        // GetUnderlyingTypeから型を取得する
                        Type t_type = Nullable.GetUnderlyingType(prop.PropertyType);

                        p_type_code = Type.GetTypeCode(t_type);
                        // Nullableフラグを立てる
                        nullable_flag = true;
                    }
                    else
                        // Nullableの型でないときは普通にPropertyTypeから取得
                        p_type_code = Type.GetTypeCode(prop.PropertyType);

                    // カラム属性値が設定されていない、またはカラム属性値に対応するカラムが存在しない場合
                    // Propertyは、Property名からデータの取得を試みる
                    if (column_atrb == null || (!columnNames.Contains(column_atrb.Name)))
                    {
                        // カラム名の中にProperty名が含まれている時
                        if (columnNames.Contains(prop.Name))
                        {
                            object db_value = dr[prop.Name];

                            // DataRowの中から値を取得して、DBNullでない時
                            if (db_value != DBNull.Value)
                            {
                                object value = db_value.ConvertValue(p_type_code);
                                // 戻り値用のオブジェクトに詰め込む
                                prop.SetValue(resT, value);
                            }
                            else
                                // DataRowの中から値を取得して、DBNullの時で
                                // Propertyの型がStringのときだけString.Emptyを書き込み
                                if (p_type_code == TypeCode.String)
                                prop.SetValue(resT, string.Empty);
                            else if (nullable_flag)
                                prop.SetValue(resT, null);
                        }
                    }
                    else if (dr[column_atrb.Name] != DBNull.Value)
                    {
                        // カラム属性値に対応したフィールドがDBNullでない場合
                        // （カラム属性値はcolumnNamesの中に存在していることは確定済み）

                        // カラムの型を取得
                        Type column_type = dr.Table.Columns[column_atrb.Name].DataType;

                        // カラムの型がNullableなら
                        if (column_type.Name.Contains("Nullable"))
                            // GetUnderlyingTypeで元の型を取得
                            column_type = Nullable.GetUnderlyingType(column_type);

                        // ↓EntityのPropertyにDataRowの値をセットする↓
                        // Propertyの型がStringだが、カラムの型がString以外の時
                        if (p_type_code == TypeCode.String &&
                                 column_type.Name != "String")

                            // String型に変換して値を詰める
                            prop.SetValue(resT, dr.Field<string>(column_atrb.Name));
                        else
                            // それ以外のときは普通に詰め込む
                            prop.SetValue
                                (resT, dr[column_atrb.Name].ConvertValue(p_type_code));
                    }
                    else if (dr[column_atrb.Name] != DBNull.Value)
                    {
                        // DataRowの中から値を取得して、DBNullの時で
                        // Propertyの型がStringのときだけString.Emptyを書き込み
                        if (p_type_code == TypeCode.String)
                            prop.SetValue(resT, string.Empty);
                        else if (nullable_flag)
                            prop.SetValue(resT, null);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return resT;
        }

        /// <summary>
        /// List(Of T型Entity) => DataTable への変換メソッド
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="tList"> Entityのリスト </param>
        /// <returns> DataTable </returns>
        public static DataTable ToDataTable<T>(this List<T> tList)
            where T : new()
        {
            var table = new DataTable();
            PropertyInfo[] properties = typeof(T).GetProperties();

            // EntityのListに何も入っていなければ終了
            if (tList.Count == 0)
                return table;

            try
            {
                // TableにDataColumnをセット
                table.Columns.AddRange(tList[0].ToDataColumns());

                foreach (T item in tList)
                    // DataTableにDataRowを追加
                    table.Rows.Add(item.ToDataRow(ref table));
            }
            catch (Exception)
            {
                throw;
            }

            return table;
        }

        /// <summary>
        /// T型EntityからDataColumunの配列を作成するメソッド
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="entity"> T型Entity </param>
        /// <returns> DataColumn </returns>
        public static DataColumn[] ToDataColumns<T>(this T entity)
            where T : new()
        {
            // ローカル変数の準備
            var columns = new DataColumn[0];
            PropertyInfo[] properties = typeof(T).GetProperties();

            try
            {
                foreach (PropertyInfo prop in properties)
                {
                    // Propertyに紐付いた属性値を取得
                    ColumnAttribute column_atrb = prop.GetCustomAttribute<ColumnAttribute>();

                    // Propertyの型を入れるためのローカル変数
                    Type p_type = null;

                    // Nullableの型の時
                    if (prop.PropertyType.Name.Contains("Nullable"))
                        // GetUnderlyingTypeから型を取得する
                        p_type = Nullable.GetUnderlyingType(prop.PropertyType);
                    else
                        // Nullableの型でないときは普通にPropertyTypeから取得
                        p_type = prop.PropertyType;

                    // カラム属性値がNullでない場合、カラム属性値をカラム名にする
                    // そうでない場合はProperty名をカラム名にする
                    var col = new DataColumn(column_atrb?.Name ?? prop.Name, p_type);

                    // 配列に詰め込み
                    Array.Resize(ref columns, columns.Length + 1);
                    columns[columns.Length - 1] = col;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return columns;
        }

        /// <summary>
        /// T型Entity => DataRow への変換メソッド
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="entity"> Entity </param>
        /// <param name="dt"> DataTable </param>
        /// <returns> DataRow </returns>
        public static DataRow ToDataRow<T>(this T entity, ref DataTable dt)
            where T : new()
        {
            DataRow row = dt.NewRow();
            PropertyInfo[] properties = typeof(T).GetProperties();

            try
            {
                foreach (PropertyInfo prop in properties)
                {
                    // Propertyに紐付いた属性値を取得
                    ColumnAttribute column_atrb = prop.GetCustomAttribute<ColumnAttribute>();

                    // カラム属性値がNullでない場合
                    if (column_atrb != null)
                        // カラム属性値のカラムにデータを登録
                        row[column_atrb.Name] = prop.GetValue(entity) ?? DBNull.Value;
                    else
                        // そうでない場合はProperty名のカラムにデータを登録
                        row[prop.Name] = prop.GetValue(entity) ?? DBNull.Value;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return row;
        }

        /// <summary>
        /// T型EntityからSqlParameterの配列を作成するメソッド
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="entity"> T型Entity </param>
        /// <returns> SqlCommandのバインド向けSqlParameter配列 </returns>
        public static SqlParameter[] ToSqlParameters<T>(this T entity)
            where T : new()
        {
            var resPrms = new SqlParameter[0];
            PropertyInfo[] properties = typeof(T).GetProperties();

            try
            {
                foreach (PropertyInfo prop in properties)
                {
                    // Propertyに紐付いた属性値を取得
                    ColumnAttribute column_atrb = prop.GetCustomAttribute<ColumnAttribute>();

                    object value = new object();
                    Type v_type = prop.PropertyType;
                    dynamic prop_value = prop.GetValue(entity);

                    // Nullableの型の場合
                    if (v_type.Name.Contains("Nullable"))
                    {
                        // GetUnderlyingTypeで元の型を取得
                        v_type = Nullable.GetUnderlyingType(v_type);

                        if (!prop_value.HasValue)
                            value = DBNull.Value;
                        else
                            value = prop_value;
                    }
                    else
                        value = prop_value;

                    var param = new SqlParameter()
                    {
                        ParameterName = "@" + column_atrb?.Name ?? prop.Name,
                        Value = value
                    };

                    // 配列に詰め込み
                    Array.Resize(ref resPrms, resPrms.Length + 1);
                    resPrms[resPrms.Length - 1] = param;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return resPrms;
        }
    }
}
