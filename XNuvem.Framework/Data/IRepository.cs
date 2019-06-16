/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace XNuvem.Data
{
    public interface IRepository<T>
    {
        IQueryable<T> Table { get; }
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Copy(T source, T target);
        void Flush();

        T Get(int id);
        T Get(Expression<Func<T, bool>> predicate);

        int Count(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count);
    }
}