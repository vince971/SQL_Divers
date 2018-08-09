using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace WebApi
{
    public class TraceSourceExceptionLogger : ExceptionLogger
    {
        private readonly TraceSource _traceSource;

        public TraceSourceExceptionLogger(TraceSource traceSource)
        {
            _traceSource = traceSource;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            LoggerHelper.Write(string.Format("Unhandled exception processing " + context.Request.Method + " for " + context.Request.RequestUri + ": " + context.Exception));
        }
    }
}