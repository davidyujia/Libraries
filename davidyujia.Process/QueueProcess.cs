using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace davidyujia.Process.QueueProcess
{
    public sealed class QueueProcess<T>
    {
        public QueueProcess(Action<T> action)
        {
            _action = action;
            _queue = new ConcurrentQueue<T>();
        }

        private readonly Action<T> _action;

        private readonly ConcurrentQueue<T> _queue;

        private Task _task;

        private readonly object _lock = new object();

        private void Run()
        {
            while (_queue.TryDequeue(out var item))
            {
                _action(item);
            }
        }

        public async Task AddAsync(T item) => await Task.Factory.StartNew(() => Add(item));

        private bool IsTaskRunning => _task == null || _task.Status != TaskStatus.Running;

        public void Add(T item)
        {
            _queue.Enqueue(item);

            if (!IsTaskRunning)
            {
                return;
            }

            lock (_lock)
            {
                if (!IsTaskRunning)
                {
                    return;
                }

                _task = new Task(this.Run);
                _task.Start();
            }
        }
    }
}
