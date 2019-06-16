using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XNuvem.Linq
{
    /// <summary>
    ///     See: http://www.albahari.com/nutshell/predicatebuilder.aspx
    ///     Update by George Santos  base on
    ///     http://www.codeproject.com/Articles/493917/Dynamic-Querying-with-LINQ-to-Entities-and-Express
    /// </summary>
    public static class PredicateBuilder
    {
        private static readonly MethodInfo StringStartsWithMethod =
            typeof(string).GetMethod("StartsWith", BindingFlags.Instance |
                                                   BindingFlags.Public, null, new[] {typeof(string)}, null);

        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        ///     See: http://www.codeproject.com/Articles/493917/Dynamic-Querying-with-LINQ-to-Entities-and-Express
        ///     Create an dynamic expression like f=>f.Property.StartsWith("value")
        /// </summary>
        /// <typeparam name="T">Generic type of the object</typeparam>
        /// <param name="expr1">First expression</param>
        /// <param name="propertyName">Property name must be the type of string</param>
        /// <param name="value">Value of the search</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrStartsWith<T>(this Expression<Func<T, bool>> expr1,
            string propertyName, string value)
        {
            // Then "and" it to the predicate.
            // e.g. predicate = predicate.And(f => f.propertyName.StartWith(startValue)); ...
            // Create an "f" as typeParameter
            var typeParameter = Expression.Parameter(typeof(T), "f");
            var memberInfo = typeof(T).GetMember(propertyName,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance).Single();
            var fieldMember = Expression.MakeMemberAccess(typeParameter, memberInfo);
            var criterianConstant = new Expression[] {Expression.Constant(value)};
            var startsWithCall = Expression.Call(fieldMember, StringStartsWithMethod, criterianConstant);
            var lambda = Expression.Lambda(startsWithCall, typeParameter) as Expression<Func<T, bool>>;
            return Or(expr1, lambda);
        }
    }
}