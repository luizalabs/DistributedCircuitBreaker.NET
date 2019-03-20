using System;
using System.Threading.Tasks;
using DistributedCircuitBreaker.Core;

namespace DistributedCircuitBreaker
{
    public interface ICircuitBreaker
    {
        CircuitState State { get; }

        void ExecuteAction(Action action);
        TResult ExecuteAction<TResult>(Func<TResult> action);
        Task ExecuteActionAsync(Func<Task> action);
        Task<TResult> ExecuteActionAsync<TResult>(Func<Task<TResult>> action);
        bool IsOpen();
    }
}