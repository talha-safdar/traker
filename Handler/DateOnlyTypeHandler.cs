using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Handler
{
    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToString("yyyy-MM-dd");
        }

        public override DateOnly Parse(object value)
        {
            return value switch
            {
                DateOnly d => d,
                DateTime dt => DateOnly.FromDateTime(dt),
                string s => DateOnly.FromDateTime(DateTime.Parse(s)),
                _ => throw new InvalidCastException($"Cannot convert {value} to DateOnly")
            };
        }
    }
}