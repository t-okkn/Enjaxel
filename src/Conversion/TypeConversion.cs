using System;

namespace Enjaxel.Conversion
{
    /// <summary>
    /// 型変換の機能を提供するクラス
    /// </summary>
    public static class TypeConversion
    {
        /// <summary>
        /// 型名（組み込み型）に即したデータに変換するメソッド
        /// </summary>
        /// <param name="typeCode"> オブジェクト型 </param>
        /// <param name="value"> 変換する値 </param>
        /// <returns> 変換された値 </returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <remarks> DBNullはチェックされています。 </remarks>
        public static object ConvertValue(this object value, TypeCode typeCode)
        {
            // 各型の初期値
            bool v_bool = false;
            byte v_byte = 0b0;
            sbyte v_sbyte = 0b0;
            short v_short = 0;
            ushort v_ushort = 0;
            int v_int = 0;
            uint v_uint = 0U;
            long v_long = 0L;
            ulong v_ulong = 0UL;
            char v_char = '\0';
            double v_double = 0.0D;
            float v_single = 0.0F;
            decimal v_decimal = 0M;
            string v_string = string.Empty;
            DateTime v_date = DateTime.MinValue;

            // DBNull値フラグ
            bool dbnull_flag = false;

            // NullかDBNullならNull値フラグを立てる
            if (value == DBNull.Value)
                dbnull_flag = true;

            try
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                    {
                        return dbnull_flag ? v_bool : Convert.ToBoolean(value);
                    }

                    case TypeCode.Byte:
                    {
                        return dbnull_flag ? v_byte : Convert.ToByte(value);
                    }

                    case TypeCode.SByte:
                    {
                        return dbnull_flag ? v_sbyte : Convert.ToSByte(value);
                    }

                    case TypeCode.Int16:
                    {
                        return dbnull_flag ? v_short : Convert.ToInt16(value);
                    }

                    case TypeCode.UInt16:
                    {
                        return dbnull_flag ? v_ushort : Convert.ToUInt16(value);
                    }

                    case TypeCode.Int32:
                    {
                        return dbnull_flag ? v_int : Convert.ToInt32(value);
                    }

                    case TypeCode.UInt32:
                    {
                        return dbnull_flag ? v_uint : Convert.ToUInt32(value);
                    }

                    case TypeCode.Int64:
                    {
                        return dbnull_flag ? v_long : Convert.ToInt64(value);
                    }

                    case TypeCode.UInt64:
                    {
                        return dbnull_flag ? v_ulong : Convert.ToUInt64(value);
                    }

                    case TypeCode.Char:
                    {
                        return dbnull_flag ? v_char : Convert.ToChar(value);
                    }

                    case TypeCode.Double:
                    {
                        return dbnull_flag ? v_double : Convert.ToDouble(value);
                    }

                    case TypeCode.Single:
                    {
                        return dbnull_flag ? v_single : Convert.ToSingle(value);
                    }

                    case TypeCode.Decimal:
                    {
                        return dbnull_flag ? v_decimal : Convert.ToDecimal(value);
                    }

                    case TypeCode.String:
                    {
                        return dbnull_flag ? v_string : Convert.ToString(value);
                    }

                    case TypeCode.DateTime:
                    {
                        return dbnull_flag ? v_date : Convert.ToDateTime(value);
                    }

                    default:
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 型名（組み込み型）に即したデータに変換するメソッド
        /// </summary>
        /// <typeparam name="T"> 変換先の型（組み込み型） </typeparam>
        /// <param name="value"> 変換する値 </param>
        /// <returns> 変換された値 </returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <remarks> DBNullはチェックされています。 </remarks>
        public static object ConvertValue<T>(this object value)
        {
            T res_t = Activator.CreateInstance<T>();
            Type t_type = typeof(T);

            // DBNull値フラグ
            bool dbnull_flag = false;

            // DBNullならNull値フラグを立てる
            if (value == DBNull.Value)
                dbnull_flag = true;

            try
            {
                if (t_type.IsPrimitive)
                {
                    if (dbnull_flag)
                    {
                        switch (Type.GetTypeCode(t_type))
                        {
                            case TypeCode.Boolean:
                            {
                                return Convert.ToBoolean(value);
                            }

                            case TypeCode.Byte:
                            {
                                return Convert.ToByte(value);
                            }

                            case TypeCode.SByte:
                            {
                                return Convert.ToSByte(value);
                            }

                            case TypeCode.Int16:
                            {
                                return Convert.ToInt16(value);
                            }

                            case TypeCode.UInt16:
                            {
                                return Convert.ToUInt16(value);
                            }

                            case TypeCode.Int32:
                            {
                                return Convert.ToInt32(value);
                            }

                            case TypeCode.UInt32:
                            {
                                return Convert.ToUInt32(value);
                            }

                            case TypeCode.Int64:
                            {
                                return Convert.ToInt64(value);
                            }

                            case TypeCode.UInt64:
                            {
                                return Convert.ToUInt64(value);
                            }

                            case TypeCode.Char:
                            {
                                return Convert.ToChar(value);
                            }

                            case TypeCode.Double:
                            {
                                return Convert.ToDouble(value);
                            }

                            case TypeCode.Single:
                            {
                                return Convert.ToSingle(value);
                            }

                            default:
                            {
                                return res_t;
                            }
                        }
                    }
                    else
                    {
                        return res_t;
                    }
                }
                else
                {
                    switch (Type.GetTypeCode(t_type))
                    {
                        case TypeCode.Decimal:
                        {
                            return dbnull_flag ? 0M : Convert.ToDecimal(value);
                        }

                        case TypeCode.String:
                        {
                            return dbnull_flag ? string.Empty : Convert.ToString(value);
                        }

                        case TypeCode.DateTime:
                        {
                            return dbnull_flag ? DateTime.MinValue : Convert.ToDateTime(value);
                        }

                        default:
                        {
                            return res_t;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Object型を除く組み込み型かどうかを判定するメソッド
        /// </summary>
        /// <param name="researchType"></param>
        /// <returns> 組み込み型かどうかを示すbool値 </returns>
        public static bool IsBuiltinType(this Type researchType)
        {
            bool result = false;

            if (researchType.IsPrimitive)
            {
                // プリミティブ型の中でIntPtrかUIntPtrでなければTrue
                if (!(researchType.Equals(typeof(IntPtr)) &&
                      researchType.Equals(typeof(UIntPtr))))
                    result = true;
            }
            else
            {
                TypeCode r_type_code = Type.GetTypeCode(researchType);

                // StringかDecimalであればTrue
                if (r_type_code == TypeCode.String || r_type_code == TypeCode.Decimal)
                    result = true;
            }

            return result;
        }
    }
}
