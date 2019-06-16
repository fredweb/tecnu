/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using System.Linq;
using System.Web.Mvc;
using XNuvem.Data;

namespace XNuvem.UI.DataTable
{
    public interface IJDataTable<TEntity>
    {
        IRepository<TEntity> DataSource { get; }
        string DefaultOrderColumn { get; set; }
        int Draw { get; set; }
        int Start { get; set; }
        int Length { get; set; }
        string Search { get; set; }
        int OrderColumn { get; set; }
        string OrderDir { get; set; }
        string ColumnsName { get; set; }
        void SetParameters(FormCollection values);
        IJDataTable<TEntity> SearchOn(string column);
        DataTableResult Execute();
        DataTableResult Execute<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> beforeExecute);
    }
}