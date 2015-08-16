using System;


namespace Extender.Main.Helpers
{
    internal static class ExceptionWrapper
    {
        internal static bool TrySafe<TException>(Action methodToExecute, Action<TException> methodOnError = null)
            where TException : Exception
        {
            try
            {
                methodToExecute.Invoke();
                return true;
            }
            catch (TException ex)
            {
                if (methodOnError != null)
                {
                    TrySafe<TException>(() => methodOnError.Invoke(ex));
                }
                return false;
            }
        }


        internal static bool TrySafe<TException>(Action methodToExecute, Action methodOnError)
            where TException : Exception
        {
            Action<TException> toExecSecond = e => { };
            if (methodOnError != null)
            {
                toExecSecond = e => methodOnError();
            }
            return TrySafe(methodToExecute, toExecSecond);
        }


        internal static TReturn TrySafe<TException, TReturn>(Func<TReturn> methodToExecute,
                                                             Func<TReturn> methodOnError = null)
            where TException : Exception
        {
            try
            {
                return methodToExecute.Invoke();
            }
            catch (TException)
            {
                if (methodOnError != null)
                {
                    return TrySafe<TException, TReturn>(methodOnError.Invoke);
                }
                return default(TReturn);
            }
        }
    }
}
