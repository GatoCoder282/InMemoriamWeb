using Dapper;
using System;
using System.Data;

namespace InMemoriam.Infraestructure.Data
{
    // Handler para DateOnly (no-nullable)
    public class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            // Guardamos como DateTime (cero horas)
            parameter.DbType = DbType.Date;
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }

        public override DateOnly Parse(object value)
        {
            if (value is DateTime dt) return DateOnly.FromDateTime(dt);
            if (value is string s && DateTime.TryParse(s, out var dt2)) return DateOnly.FromDateTime(dt2);
            throw new DataException($"Cannot convert {value?.GetType().FullName ?? "null"} to DateOnly");
        }
    }

    // Handler para DateOnly nullable
    public class NullableDateOnlyHandler : SqlMapper.TypeHandler<DateOnly?>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly? value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.HasValue ? (object)value.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value;
        }

        public override DateOnly? Parse(object value)
        {
            if (value == null || value is DBNull) return null;
            if (value is DateTime dt) return DateOnly.FromDateTime(dt);
            if (value is string s && DateTime.TryParse(s, out var dt2)) return DateOnly.FromDateTime(dt2);
            throw new DataException($"Cannot convert {value?.GetType().FullName ?? "null"} to DateOnly?");
        }
    }

    // Método auxiliar para registro desde Program.cs
    public static class DapperTypeHandlerRegistration
    {
        public static void Register()
        {
            SqlMapper.AddTypeHandler(new DateOnlyHandler());
            SqlMapper.AddTypeHandler(new NullableDateOnlyHandler());
        }
    }
}