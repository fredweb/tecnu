using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace XNuvem.Exceptions
{
    public class ErrorJsonResult : JsonResult
    {
        public ErrorJsonResult(Exception exception)
        {
            var errors = new List<string>();
            var ex = exception;
            do
            {
                errors.Add(ex.Message);
                ex = ex.InnerException;
            } while (ex != null);
            Data = new {IsError = true, Messages = errors};
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
    }
}