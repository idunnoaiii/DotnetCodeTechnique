using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace WhereClauseDynamicLinq
{
    public class ExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");

        private static MethodInfo startsWithMethod =
            typeof(string).GetMethod("StartsWith", new Type[] {typeof(string)});

        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] {typeof(string)});


        public static Func<T, bool> Build<T>(IList<Filter> filters)
        {
            if (filters.Count == 0)
            {
                return null;
            }

            ParameterExpression parameter = Expression.Parameter(typeof(Person), "t");
            Expression exp = null;

            if (filters.Count == 1)
            {
                exp = GetExpression<T>(parameter, filters[0]);
            }
            else if (filters.Count == 2)
            {
                exp = GetExpression<T>(parameter, filters[0], filters[1]);
            }
            else
            {
                foreach (var filter in filters)
                {
                    exp = exp == null
                        ? GetExpression<T>(parameter, filter)
                        : Expression.AndAlso(exp, GetExpression<T>(parameter, filter));
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, parameter).Compile();
        }

        private static Expression GetExpression<T>(ParameterExpression param, Filter filter)
        {
            MemberExpression member = Expression.Property(param, filter.Property);
            ConstantExpression constant = Expression.Constant(filter.Value);

            return filter.Operation switch
            {
                Op.Equals => Expression.Equal(member, constant),
                Op.GreaterThan => Expression.GreaterThan(member, constant),
                Op.LessThan => Expression.GreaterThan(member, constant),
                Op.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
                Op.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
                Op.Contains => Expression.Call(member,containsMethod, constant),
                Op.StartsWith => Expression.Call(member, startsWithMethod, constant),
                Op.EndsWith => Expression.Call(member, endsWithMethod, constant),
                _ => null
            };
        }

        private static BinaryExpression GetExpression<T>(ParameterExpression param, Filter filter1, Filter filter2)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);
            return Expression.AndAlso(bin1, bin2);
        }
    }
}