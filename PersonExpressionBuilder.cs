using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WhereClauseDynamicLinq
{
    public class PersonExpressionBuilder
    {
        public static Func<Person, bool> Build(IList<Filter> filters)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(Person), "t");
            Expression exp = null;
            
            if (filters.Count == 1)
            {
                exp = GetExpression(parameter, filters[0]);
            }
            else if (filters.Count == 2)
            {
                exp = GetExpression(parameter, filters[0], filters[1]);
            }
            else
            {
                foreach (var filter in filters)
                {
                    if(exp == null)
                    {
                        exp = GetExpression(parameter, filter);
                    }
                    else
                    {
                        exp = Expression.AndAlso(exp, GetExpression(parameter, filter));
                    }
                }

            }
            
            return Expression.Lambda<Func<Person, bool>>(exp, parameter).Compile();
        }

        private static Expression GetExpression(ParameterExpression param, Filter filter)
        {
            MemberExpression member = Expression.Property(param, filter.Property);
            ConstantExpression constant = Expression.Constant(filter.Value);
            return Expression.Equal(member, constant);
        }

        private static BinaryExpression GetExpression(ParameterExpression param, Filter filter1, Filter filter2)
        {
            Expression bin1 = GetExpression(param, filter1);
            Expression bin2 = GetExpression(param, filter2);
            return Expression.AndAlso(bin1, bin2);
        }
    }
}