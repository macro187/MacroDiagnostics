using System;
using System.Diagnostics;
using System.IO;
using MacroGuards;


namespace
MacroDiagnostics
{


/// <summary>
/// Maintain the <see cref="Trace.CorrelationManager.LogicalOperationStack"/> 
/// </summary>
///
/// <remarks>
/// <para>
/// Logical operation stack traces can be included with trace messages if enabled using
/// <see cref="TraceListener.TraceOutputOptions"/>.
/// </para>
/// <para>
/// This API also emits <see cref="TraceEventType.Start"/> and <see cref="TraceEventType.Stop"/> trace messages when
/// operations are started and finished, respectively.
/// </para>
/// </remarks>
///
public static class
LogicalOperation
{


/// <summary>
/// Start a logical operation
/// </summary>
///
/// <param name="description">
/// A verb phrase in the present continuous tense describing the operation
/// - OR -
/// An indentifier for the current item (out of some collection) being processed
/// </param>
///
/// <returns>
/// An <see cref="IDisposable"/> that ends the logical operation when disposed
/// </returns>
///
/// <example>
/// using (LogicalOperation.Start("Processing orders")
/// {
///     foreach (var order in orders)
///     {
///         using (LogicalOperation.Start(order.Id)
///         {
///             ProcessOrder(order);
///         }
///     }
/// }
/// </example>
///
public static
IDisposable Start(string description)
{
    Guard.NotNull(description, nameof(description));
    Trace.CorrelationManager.StartLogicalOperation(description);
    TraceEvent(TraceEventType.Start, description, null);
    return new Ender(description);
}


[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Reliability",
    "CA2002:DoNotLockOnObjectsWithWeakIdentity",
    Justification = "Trace does the same thing internally")]
static void
TraceEvent(TraceEventType eventType, string format, params object[] args)
{ 
    var eventCache = new TraceEventCache();

    Action<TraceListener> traceAction;
    if(args != null)
        traceAction = listener => listener.TraceEvent(eventCache, source, eventType, 0, format, args);
    else
        traceAction = listener => listener.TraceEvent(eventCache, source, eventType, 0, format);

    lock(traceLock)
    {
        if (args == null) {
            foreach (TraceListener listener in Trace.Listeners) { 
                if (listener.IsThreadSafe)
                {
                    traceAction(listener);
                    if (Trace.AutoFlush) listener.Flush(); 
                }
                else
                {
                    lock (listener)
                    {
                        traceAction(listener);
                        if (Trace.AutoFlush) listener.Flush(); 
                    }
                }
            } 
        }
    }
}


static string
source = Path.GetFileName(Environment.GetCommandLineArgs()[0]);


//
// We don't have access to Trace's internal global lock, so do the best we can
//
static object
traceLock = new object();


class
Ender
    : IDisposable
{
    public
    Ender(string description)
    {
        this.description = description;
        disposed = false;
    }

    readonly string
    description;

    bool
    disposed;

    public void
    Dispose()
    {
        if (disposed) return;
        disposed = true;
        TraceEvent(TraceEventType.Stop, description, null);
        Trace.CorrelationManager.StopLogicalOperation();
    }
}


}
}
