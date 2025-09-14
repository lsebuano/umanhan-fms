using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Shared
{
    public class EfCoreDiagnosticObserver : IObserver<DiagnosticListener>
    {
        private readonly EfCoreQueryLogger _queryLogger;

        public EfCoreDiagnosticObserver(EfCoreQueryLogger queryLogger)
        {
            _queryLogger = queryLogger;
        }

        public void OnCompleted()
        {
            // Not needed for EF Core
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"[EF Observer Error] {error.Message}");
        }

        public void OnNext(DiagnosticListener listener)
        {
            // Subscribe only to EF Core events
            if (listener.Name == "Microsoft.EntityFrameworkCore")
            {
                listener.Subscribe(_queryLogger);
            }
        }
    }

}
