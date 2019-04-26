using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace QueueProcess
{
    public sealed class QueueProcess<T>
    {
        public QueueProcess(Action<T> action)
        {
            _action = action;
            this._queue = new ConcurrentQueue<T>();
        }

        private readonly Action<T> _action;

        private readonly ConcurrentQueue<T> _queue;

        private Task _task;

        private readonly object _lock = new object();

        private void Run()
        {
            while (this._queue.TryDequeue(out T item))
            {
                _action(item);
            }
        }

        public async Task AddAsync(T item) => await Task.Factory.StartNew(() => Add(item));

        public void Add(T item)
        {
            this._queue.Enqueue(item);

            if (_task == null || this._task.Status != TaskStatus.Running)
            {
                lock (_lock)
                {
                    if (_task == null || this._task.Status != TaskStatus.Running)
                    {
                        _task = new Task(this.Run);
                        _task.Start();
                    }
                }
            }
        }
    }
}
