using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace CSharp.Helper
{
    public class TryCatchHelper
    {
        public static void ExecuteWithCatch(Expression<Action> actionExpression)
        {
            Action action = actionExpression.Compile();
            try 
            {
                action();
            }
            catch(Exception ex)
            {
                if(LogException(ex, (MethodCallExpression) actionExpression.Body))
                    throw;
            }
        }


        public static TResult ExecuteWithCatch<TResult>(Expression<Func<TResult>> funcExpression) 
        {
            var func = funcExpression.Compile();
            try 
            {
                return func();
            }
            catch(Exception ex)
            {
                if(LogException(ex, (MethodCallExpression) funcExpression.Body))
                    throw;
                return default(TResult);
            }
        }


        public static TResult ExecuteWithCatch<TQueryResult, TResult>(
            Expression<Func<TQueryResult>> queryFuncExpression, 
            Func<TQueryResult, TResult> callbackFunc)
        {
            try 
            {
                var func = queryFuncExpression.Compile();
                var result = func();
                return callbackFunc(result);
            }
            catch(Exception ex)
            {
                if(LogException(ex, (MethodCallExpression)queryFuncExpression.Body))
                    throw;

                return default(TResult);
            }
        }

        private static bool LogException(Exception ex, MethodCallExpression methodCallExpr)
        {
            var argumentNames = methodCallExpr.Method.GetParameters().Select(pi => pi.Name).ToList();
            var argumentValue = methodCallExpr.Arguments.Select(p => GetArgumentValueStr(p)).ToList();
            argumentNames.Add("UserAuxillaryMessage");
            argumentNames.Add(methodCallExpr.Method.Name);
            /**
                Handle exception here
            */
            return false;
        }

        private static string GetArgumentValueStr(Expression argumentExpr)
        {
            if((argumentExpr as ConstantExpression) != null)
            {
                return (argumentExpr as ConstantExpression).Value.ToString();
            }

            if((argumentExpr as MemberExpression) != null)
            {
                var expression = argumentExpr as MemberExpression;
                if((expression.Member as PropertyInfo) != null)
                {
                    var exp = (MemberExpression) expression.Expression;
                    var constant = (ConstantExpression) exp.Expression;
                    var fieldInfoValue = ((FieldInfo)exp.Member).GetValue(constant.Value);
                    var value = ((PropertyInfo)expression.Member).GetValue(fieldInfoValue, null);
                    return value.ToString();
                }
                if((expression.Member as FieldInfo) != null)
                {
                    var filedInfo = expression.Member as FieldInfo;
                    var constantExpression = expression.Expression as ConstantExpression;
                    if(filedInfo != null & constantExpression != null)
                    {
                        var value = filedInfo.GetValue(constantExpression.Value);
                        if((value as XmlDocument)!=null)
                        {
                            return (value as XmlDocument).OuterXml;
                        }

                        return value?.ToString();
                    }
                    return string.Empty;
                }
            }

            throw new NotSupportedException(argumentExpr.ToString());
        }
    }
}