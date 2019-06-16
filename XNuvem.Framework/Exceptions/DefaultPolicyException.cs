using System;
using XNuvem.Data;
using XNuvem.Logging;

namespace XNuvem.Exceptions
{
    public class DefaultPolicyException : IPolicyException
    {
        private readonly ITransactionManager _transactionManager;

        public DefaultPolicyException(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool HandleException(Exception ex)
        {
            try
            {
                Logger.Error(ex, "Erro inesperado ao executar uma operação");
                Logger.Warning("Canceling transaction due to an error");
                _transactionManager.Cancel();
                if (ex.IsFatal()) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}