﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProCSharpCode.MultiThreading
{

    #region Limitations of Threads
    // ------------------------ Tasks -------------------------
    // A thread is a low-level tool for creating concurrency, and as such it has limitations.
    // In particular:
    // • While it’s easy to pass data into a thread that you start, there’s no easy way to
    //   get a “return value” back from a thread that you Join.You have to set up some
    //   kind of shared field. And if the operation throws an exception, catching and
    //   propagating that exception is equally painful.
    // • You can’t tell a thread to start something else when it’s finished; instead you
    //   must Join it(blocking your own thread in the process).

    // These limitations discourage fine-grained concurrency; in other words, they make it
    // hard to compose larger concurrent operations by combining smaller ones(some‐
    // thing essential for the asynchronous programming that we’ll look at in following
    // sections). This in turn leads to greater reliance on manual synchronization(locking,
    // signaling, and so on) and the problems that go with it.

    // The direct use of threads also has performance implications that we discussed in
    // “The Thread Pool” on page 575. And should you need to run hundreds or thou‐
    // sands of concurrent I/O-bound operations, a thread-based approach consumes
    // hundreds or thousands of MB of memory purely in thread overhead.

    // Moamen summary:
    // limitations are:
    //  - no way to return values except with shared state 
    //  - probelm of lake of thread safety
    //  - problem of deadlocks if you avoid thread safety
    //  - catching exceptions and silent faliure 
    //  - hard to compose large concurrency operations
    //  - you can't run multiple methods within the same thread, like the case of async 
    // --------------------------------------------------------------
    #endregion

    #region Tasks
    // ------------------------ Tasks -------------------------
    // The Task class helps with all of these problems.Compared to a thread, a Task is
    // higher-level abstraction—it represents a concurrent operation that may or may not
    // be backed by a thread.Tasks are compositional(you can chain them together
    // through the use of continuations). 
    //
    // They can use the thread pool to lessen startup
    // latency, and with a TaskCompletionSource, they can leverage a callback approach
    // that avoids threads altogether while waiting on I/O-bound operations.
    //
    // The Task types were introduced in Framework 4.0 as part of the parallel program‐
    // ming library.However, they have since been enhanced (through the use of awaiters)
    // to play equally well in more general concurrency scenarios, and are backing types
    // for C#’s asynchronous functions.
    // -------------------------------------------------------------------------
    #endregion

    #region Starting a Task
    // ------------------------ Starting a Task -------------------------
    //From Framework 4.5, the easiest way to start a Task backed by a thread is with the
    //static method Task.Run(the Task class is in the System.Threading.Tasks name‐
    //space). Simply pass in an Action delegate.

    //The Task.Run method was introduced in Framework 4.5. In Framework 4.0, you
    //can accomplish the same thing by calling Task.Factory.StartNew. (The former is
    //mostly a shortcut for the latter.)

    // Task.Run returns a Task object that we can use to monitor its progress

    // Notice, however, that we didn’t call Start after calling Task.Run
    // because this method creates “hot” tasks; you can instead use Task’s constructor to
    // create “cold” tasks, although this is rarely done in practice.

    // You can track a task’s execution status via its Status property.

    // Tasks use pooled threads by default, which are background threads.
    public class StartTask
    {
        // Test Method
        public static void Test()
        {
            // Simply pass in an Action delegate:
            Task.Run(() => Console.WriteLine("Foo1"));

            // wait task to finish as it uses pooled background thread, if main thread ended next
            //Console.ReadLine();


            // Calling Task.Run in this manner is similar to starting a thread as follows (except for
            // the thread pooling implications)
            new Thread(() => Console.WriteLine("Foo2")).Start(); // not in the pool

            // In Framework 4.0, you can accomplish the same thing by calling 
            // Task.Factory.StartNew.
            Task.Factory.StartNew(() => Console.WriteLine("Foo3"));


            // Note that Task.Run returns a Task object
            Task task = Task.Run(() => Console.WriteLine("Hello World"));
            // then, call Task instance members.



        }


    }

    // --------------------------------------------------------------
    #endregion

    #region Wait a Task
    // ------------------------ Wait a Task -------------------------
    // Calling Wait on a task blocks until it completes and is the equivalent of calling Join
    // on a thread

    // Wait lets you optionally specify a timeout and a cancellation token to end the wait early.

    public class WaitTask
    {
        // Test Method
        public static void Test()
        {
            // Calling Wait on a task blocks until it completes and is the
            // equivalent of calling Join on a thread
            Task task = Task.Run(() =>
            {
                Thread.Sleep(3000); // sleep for 3 second
                Console.WriteLine("End of 2 sec Sleep");

            });


            Console.WriteLine("task.IsCompleted: {0}", task.IsCompleted); // False
            Console.WriteLine("Wait task to finish...");

            // block that thread until task finish it's work
            task.Wait();

            Console.WriteLine("task.IsCompleted: {0}", task.IsCompleted); // True

        }


    }

    // --------------------------------------------------------------
    #endregion

    #region  Long - running tasks
    // ------------------------  Long - running tasks -------------------------
    // Running one long-running task on a pooled thread won’t
    // cause trouble; it’s when you run multiple long-running tasks
    // in parallel(particularly ones that block) that performance can suffer.
    // so one solution is to use TaskCreationOptions.LongRunning to prevent run 
    // task in Pool.

    // there are usually better solutions than TaskCreationOptions.LongRunning:
    //
    // • If the tasks are I/O-bound, TaskCompletionSource and
    //   asynchronous functions let you implement concurrency
    //   with callbacks(continuations) instead of threads.
    //
    // • If the tasks are compute-bound, a producer/consumer
    //   queue lets you throttle the concurrency for those tasks,
    //   avoiding starvation for other threads and processes(see
    //   “Writing a Producer/Consumer Queue” on page 950 in
    //   Chapter 23).
    public class LongRunningTask
    {

        // Test Method
        public static void Test()
        {

            // For longer-running and blocking operations, you can prevent 
            // use of a pooled thread as follows:
            Task task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Long Task Run...");
                Thread.Sleep(5000);
            }, TaskCreationOptions.LongRunning);
        }


    }

    // --------------------------------------------------------------
    #endregion

    #region Task Return Value
    // ------------------------ Return Value from a Task -------------------------
    // Returning Values
    // Task has a generic subclass called Task<TResult> that allows a task to emit a return
    // value.You can obtain a Task<TResult> by calling Task.Run with a Func<TResult>
    // delegate (or a compatible lambda expression) instead of an Action:
    // Task<int> task = Task.Run(() => { Console.WriteLine("Foo"); return 3; });
    // // ...
    // You can obtain the result later by querying the Result property.If the task hasn’t
    // yet finished, accessing this property will block the current thread until the task
    // finishes:
    // int result = task.Result; // Blocks if not already finished
    // Console.WriteLine (result); // 3
    public class TaskReturnValue
    {
        // Test Method
        public static void Test()
        {
            Task<int> primeNumberTask = Task.Run(() =>  // Func<int> delegate
            {
                return Enumerable.Range(2, 3000000).Count(n
                    => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
            });


            Console.WriteLine("Task running...");
            Console.WriteLine("The answer is " + primeNumberTask.Result);

        }


    }

    // --------------------------------------------------------------
    #endregion

    #region Exceptions
    // ------------------------ Exceptions -------------------------
    // Unlike with threads, tasks conveniently propagate exceptions.So, if the code in
    // your task throws an unhandled exception (in other words, if your task faults), that
    // exception is automatically re-thrown to whoever calls Wait()—or accesses the
    // Result property of a Task<TResult>

    // You can test for a faulted task without re-throwing the exception via the IsFaulted
    // and IsCanceled properties of the Task.If both properties return false, no error
    // occurred; if IsCanceled is true, an OperationCanceledException was thrown for
    // that task(see “Cancellation” on page 606); if IsFaulted is true, another type of excep‐
    // tion was thrown and the Exception property will indicate the error.

    public class TaskExceptions
    {
        // Test Method
        public static void Test()
        {
            Task task = Task.Run(() =>
            {
                throw null;
            });

            Task<int> task2 = Task.Run(() =>
            {
                bool a = false;
                if (a)
                    return 0;
                else
                    throw null;
            });

            try
            {
                // exception is automatically re - thrown to whoever calls Wait()
                task.Wait();
            }
            catch (AggregateException aggexp)
            {
                // The CLR wraps the exception in an AggregateException in order to play well with
                // parallel programming scenarios
                Exception exp = aggexp.InnerException;
                Console.WriteLine(exp);
            }

            try
            {
                // exception is automatically re - thrown to whoever accesses the
                // Result property of a Task<TResult>
                Console.WriteLine($"Result: {task2.Result}");
            }
            catch (AggregateException aggexp)
            {
                // The CLR wraps the exception in an AggregateException in order to play well with
                // parallel programming scenarios
                if (aggexp.InnerException is NullReferenceException)
                    Console.WriteLine("Null!");
                else
                {
                    Exception exp = aggexp.InnerException;
                    Console.WriteLine(exp);
                }


            }



        }


    }

    // --------------------------------------------------------------
    #endregion

    #region Exceptions and autonomous tasks
    // ------------------------ Exceptions and autonomous tasks -------------------------
    // With autonomous “set-and-forget” tasks(those for which you don’t rendezvous via
    // Wait() or Result, or a continuation that does the same), it’s good practice to
    // explicitly exception-handle the task code to avoid silent failure, just as you would
    // with a thread.
    //
    // 
    // Unhandled exceptions on autonomous tasks are called unobserved exceptions and in
    // CLR 4.0, they would actually terminate your program(the CLR would re-throw the
    // exception on the finalizer thread when the task dropped out of scope and was
    // garbage collected). This was helpful in indicating that a problem had occurred that
    // you might not have been aware of; however the timing of the error could be decep‐
    // tive in that the garbage collector can lag significantly behind the offending task.
    // Hence, when it was discovered that this behavior complicated certain patterns of
    // asynchrony (see “Parallelism” on page 600 and “WhenAll” on page 611), it was dropped
    // in CLR 4.5.

    // Ignoring exceptions is fine when an exception solely indicates
    // a failure to obtain a result that you’re no longer interested in.
    // For example, if a user cancels a request to download a web
    // page, we wouldn’t care if it turns out that the web page didn’t
    // exist.
    //
    // 
    // Ignoring exceptions is problematic when an exception indi‐
    // cates a bug in your program, for two reasons:
    // 
    //   • The bug may have left your program in an invalid state.
    //   
    //   • More exceptions may occur later as a result of the bug,
    //     and failure to log the initial error can make diagnosis dif‐
    //     ficult.
    //
    //
    // There are a couple of interesting nuances on what counts as unobserved:
    // 
    //    • Tasks waited upon with a timeout will generate an unobserved exception if the
    //      faults occurs after the timeout interval.
    //
    //    • The act of checking a task’s Exception property after it has faulted makes the
    //      exception “observed.
    //
    //


    class UnobservedExceptions1
    {
        public static void Test()
        {
            // autonomous Task (Set-and-Forget): not waited or return result
            Task.Run(() =>
            {
                Console.WriteLine("Task Running...");
                // unobserved Exception
                throw new NullReferenceException();
            });

            Thread.Sleep(1000);
            Console.WriteLine("I am The Main Thread");
        }

    }

    // after wait timeout
    class UnobservedExceptions2
    {
        public static void Test()
        {
            // autonomous Task (Set-and-Forget): not waited or return result
            Task task = Task.Run(() =>
            {
                Console.WriteLine("Task Running...");
                Thread.Sleep(2000);
                // unobserved exception if the faults occurs after the timeout interval.
                throw new NullReferenceException();
            });

            try
            {
                // unobserved exception if the faults occurs after the timeout interval.
                task.Wait(1000);
                Console.WriteLine("Wait Timeout Ended");
                Thread.Sleep(1500);
                Console.WriteLine("I am The Main Thread");
            }
            catch (AggregateException aggexp)
            {
                Console.WriteLine(aggexp.InnerException);
            }
        }

    }

    // subscribe to unobserved exceptions at a global level via the static event
    // TaskScheduler.UnobservedTaskException;
    class LoggingUnobservedExceptions
    {
        public static void Test()
        {
            TaskScheduler.UnobservedTaskException += HandleUnobservedException;


            // autonomous Task (Set-and-Forget): not waited or return result
            Task.Run(() =>
            {
                Console.WriteLine("Task Running...");
                // unobserved Exception
                throw new NullReferenceException();
            });

        }

        private static void HandleUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Logging an Unobserved Exceptions
            // ...
        }
    }

    // wait
    class ObservedExceptions1
    {
        public static void Test()
        {
            // autonomous Task (Set-and-Forget): not waited or return result
            Task task = Task.Run(() =>
            {
                Console.WriteLine("Task Running...");
                // unobserved exception if the faults occurs after the timeout interval.
                throw new NullReferenceException();
            });

            // unobserved exception if the faults occurs after the timeout interval.
            task.Wait();


        }

    }

    // Result property
    class ObservedExceptions2
    {
        public static void Test()
        {

            Task<int> task = Task.Run(() =>
           {
               Console.WriteLine("Task Running...");
                // unobserved exception if the faults occurs after the timeout interval.
                throw new NullReferenceException();

               return 10;
           });

            Console.WriteLine("Result{0}", task.Result);


        }

    }

    // Checking Exception property of an autonomous Task
    class ObservedExceptions3
    {
        public static void Test()
        {

            Task task = Task.Run(() =>
            {
                Console.WriteLine("Task Running...");
                // unobserved exception if the faults occurs after the timeout interval.
                throw new NullReferenceException();
            });

            Thread.Sleep(1000);
            // The act of checking a task’s Exception property after it 
            // has faulted makes the exception "observed".
            AggregateException agg = task.Exception;
            Console.WriteLine(agg.InnerException);


        }

    }

    // -------------------------------------------------------------------------
    #endregion

    #region Continuations With TaskAwaiter<TResult>
    // ------------------------ Continuations With TaskAwaiter<TResult> ------------------------- 
    // A continuation says to a task, “when you’ve finished, continue by doing something
    // else.” A continuation is usually implemented by a callback that executes once upon
    // completion of an operation.There are two ways to attach a continuation to a task.
    // The first was introduced in Framework 4.5 and is particularly significant because
    // it’s used by C#’s asynchronous functions.


    // 

    public class Continuations
    {
        // Test Method
        public static void Test()
        {
            Task<int> primeNumberTask = Task.Run(() =>
                Enumerable.Range(2, 3000000).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));

            // -------------------------------------------------------------------------
            // System.Runtime.CompilerServices.TaskAwaiter<TResult> awaiter
            var awaiter = primeNumberTask.GetAwaiter();

            awaiter.OnCompleted(() =>
            {

                Console.WriteLine("Task Completed.");
                int result = awaiter.GetResult();
                Console.WriteLine("Result: {0}", result);
            });

            Console.WriteLine("I am Free not tied with task ^_^");

        }

        public static string CurrentInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Current Thread: {Thread.CurrentThread.ManagedThreadId}");
            builder.Append($"Current Context Prop: ");
            var prop = Thread.CurrentContext.ContextProperties;
            foreach (var item in prop)
            {
                builder.Append($" {item.Name}");

            }
            builder.AppendLine();

            return builder.ToString();
        }

    }

    // Benefits of awaiter.GetResult();
    // --------------------------------
    // If an antecedent task faults, the exception is re-thrown when the continuation code
    // calls awaiter.GetResult(). Rather than calling GetResult, we could simply access
    // the Result property of the antecedent.The benefit of calling GetResult is that if the
    // antecedent faults, the exception is thrown directly without being wrapped in Aggre
    // gateException, allowing for simpler and cleaner catch blocks.
    //
    // For nongeneric tasks, GetResult() has a void return value. Its useful function is
    // then solely to rethrow exceptions.
    //
    //
    // awaiter.OnCompleted() And synchronization context 
    // -------------------------------------------------
    // If a synchronization context is present, OnCompleted automatically captures it and
    // posts the continuation to that context.
    // This is very useful in rich-client applications as it bounces the 
    // continuation back to the UI thread.



    // --------------------------------------------------------------
    #endregion

    #region Continuation With TaskAwaiter<TResult> and ConfigureAwait Method
    // ------------------------ Continuation With TaskAwaiter<TResult> and ConfigureAwait Method -------------------------

    // If a synchronization context is present, OnCompleted automatically captures it and
    // posts the continuation to that context.This is very useful in rich-client applications,
    // as it bounces the continuation back to the UI thread.In writing libraries, however,
    // it’s not usually desirable because the relatively expensive UI-thread-bounce should
    // occur just once upon leaving the library, rather than between method calls.Hence
    // you can defeat it the ConfigureAwait method:
    //  
    //   var awaiter = primeNumberTask.ConfigureAwait(false).GetAwaiter();
    //  
    // If no synchronization context is present—or you use ConfigureAwait(false)—the
    // continuation will(in general) execute on the same thread as the antecedent, avoiding 
    // unnecessary overhead.

    public class ContinuationsAndConfigureAwait
    {
        // Test Method
        public static void Test()
        {
            Console.WriteLine(CurrentInfo());

            Task<int> primeNumberTask = Task.Run(() =>
            {
                Console.WriteLine(CurrentInfo());
                return Enumerable.Range(2, 3000000).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
            });


            // -------------------------------------------------------------------------
            // ConfigureAwait(false):
            //     - make continuation in the same thread of Task, avoiding unnecessary overhead
            // ConfigureAwait(true): default // or SyncronizationContext Recognized
            //     - make continuation in thread that calls Task.
            var awaiter = primeNumberTask.ConfigureAwait(false).GetAwaiter();

            awaiter.OnCompleted(() =>
            {
                Console.WriteLine("Task Completed.");
                Console.WriteLine(CurrentInfo());
                int result = awaiter.GetResult();
                Console.WriteLine("Result: {0}", result);
            });


            Console.WriteLine("I am Free not tied with task ^_^");

        }

        public static string CurrentInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Current Thread: {Thread.CurrentThread.ManagedThreadId}");
            builder.Append($"Current Context Prop: ");
            var prop = Thread.CurrentContext.ContextProperties;
            foreach (var item in prop)
            {
                builder.Append($" {item.Name}");

            }
            builder.AppendLine();

            return builder.ToString();
        }

    }

    // --------------------------------------------------------------
    #endregion

    #region Continuations With ContinueWith Method
    // ------------------------ Continuations With ContinueWith Method ------------------------- 
    // The other way to attach a continuation is by calling the task’s ContinueWith method.
    // ContinueWith itself returns a Task, which is useful if you want 
    // to attach further continuations.
    // 
    // However, you must deal directly with AggregateException if the
    // task faults, and write extra code to marshal the continuation in UI applications(see
    // “Task Schedulers” on page 943 in Chapter 23). And in non-UI contexts, you must
    // specify TaskContinuationOptions.ExecuteSynchronously if you want the continuation
    // to execute on the same thread; otherwise it will bounce to the thread pool.
    // ContinueWith is particularly useful in parallel programming scenarios; we cover it
    // in detail in “Continuations” on page 938 in Chapter 23.


    public class ContinuationsWithContinueWith
    {
        // Test Method
        public static void Test()
        {
            Task<int> primeNumberTask = Task.Run(() =>  // Func<int> delegate
            {
                return Enumerable.Range(2, 3000000).Count(n
                    => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
            });

            // The other way to attach a continuation is by calling the task’s ContinueWith method
            // ContinueWith itself returns a Task, which is useful if you want 
            // to attach further continuations.
            // -------------------------------------------------------------------------

            // note that antecedent is the same task primeNumberTask
            Task<double> theNewTask = primeNumberTask.ContinueWith<double>(antecedent =>
            {
                Console.WriteLine("Task Completed.");
                int result = antecedent.Result;
                Console.WriteLine("Result: {0}", result);
                return result * 0.1;
            });


            // -------------------------------------------------------------------------
            // note that this task doen't have a return type
            Task lastTask = theNewTask.ContinueWith(antecedent =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("Result: {0}", antecedent.Result);
                Console.WriteLine("The Last Task Complete.");
            });


        }

    }

    // --------------------------------------------------------------
    #endregion

    #region TaskCompletionSource
    // ------------------------ TaskCompletionSource -------------------------
    // Another way to create a task is with TaskCompletionSource.
    // TaskCompletionSource lets you create a task out of any operation that starts and
    // finishes some time later.It works by giving you a “slave” task that you manually
    // drive—by indicating when the operation finishes or faults. This is ideal for I/Obound work:
    // you get all the benefits of tasks (with their ability to propagate return
    // values, exceptions, and continuations) without blocking a thread for the duration of
    // the operation.

    // The task, however, is controlled entirely by the TaskCompletionSource object 
    // via the following methods:
    // public class TaskCompletionSource<TResult>
    // {
    //     public void SetResult(TResult result);
    //     public void SetException(Exception exception);
    //     public void SetCanceled();
    //     public bool TrySetResult(TResult result);
    //     public bool TrySetException(Exception exception);
    //     public bool TrySetCanceled();
    //     public bool TrySetCanceled(CancellationToken cancellationToken);
    // ...
    // }
    // 
    // Calling any of these methods signals the task, putting it into a completed, faulted, or
    // canceled state(we’ll cover the latter in the section “Cancellation” on page 606). You’re
    // supposed to call one of these methods exactly once: if called again, SetResult,
    // SetException, or SetCanceled will throw an exception, whereas the Try* methods
    // return false.
    public class WorkingWithTaskCompletionSource
    {
        // Test Method
        public static void Test()
        {


        }

        void PrintAfter2Sec(int val)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            new Thread(() =>
            {
                Thread.Sleep(2000);
                tcs.SetResult(val);
            }).Start();

            Task<int> task = tcs.Task;          // out slave task
            Console.WriteLine(task.Result);     // our result
        }

    }

    // --------------------------------------------------------------
    #endregion

    #region Run With TaskCompletionSource
    // ------------------------ Run With TaskCompletionSource -------------------------
    // Calling The next Run method is equivalent to calling Task.Factory.StartNew with the Task
    // CreationOptions.LongRunning option to request a nonpooled thread.

    public class RunWithTaskCompletionSource
    {
        // Test Method
        public static void Test()
        {
            Task<int> task = Run(() => { Thread.Sleep(5000); return 42; });
            Console.WriteLine("Task Result: {0}", task.Result);

        }

        // Calling this method is equivalent to calling Task.Factory.StartNew with the Task
        // CreationOptions.LongRunning option to request a nonpooled thread.
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try
                {
                    tcs.SetResult(function());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }).Start();
            return tcs.Task;
        }


    }

    // --------------------------------------------------------------
    #endregion

    #region Delay With TaskCompletionSource
    // ------------------------ Delay With TaskCompletionSource -------------------------

    public class DelayWithTaskCompletionSource
    {
        // Test Method
        public static void Test()
        {
            Delay(1000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));
        }

        public static Task<int> GetAnswerAfter5Sec(int val)
        {
            var rcs = new TaskCompletionSource<int>();

            var timer = new System.Timers.Timer(5000) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); rcs.SetResult(val); };
            timer.Start();
            return rcs.Task;
        }

        public static Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
            timer.Start();
            return tcs.Task;
        }


    }

    // --------------------------------------------------------------
    #endregion

    #region Task.Delay Method
    // ------------------------ Task.Delay Method -------------------------
    // Task.Delay is the asynchronous equivalent of Thread.Sleep

    // The Delay method that we just wrote is sufficiently useful that it’s available as a
    // static method on the Task class

    public class DelayMethod
    {
        // Test Method
        public static void Test()
        {
            Task.Delay(2000).GetAwaiter().OnCompleted(() => Console.WriteLine(12345));

            // or
            Task.Delay(3000).ContinueWith((ant) => Console.WriteLine(67890));

        }


    }

    // --------------------------------------------------------------
    #endregion


}
