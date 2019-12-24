using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TaskManagerRefact.Core
{
    public class TaskManager
    {
        private readonly ConcurrentQueue<Action> _actionsQueue = new ConcurrentQueue<Action>();
        private readonly AutoResetEvent _waitHandler = new AutoResetEvent(false);
        private readonly Thread _mainThread;

        private List<WorkThread> _idleThreads;
        private List<WorkThread> _workingThreads;

        private object _locker = new object();

        // Объявляем делегат
        public delegate void TaskManagerWorkDoneHandler();
        // Событие, возникающее при выводе денег
        public event TaskManagerWorkDoneHandler TaskManagerWorkDone;

        public TaskManager(int threadsNum)
        {
            _idleThreads = new List<WorkThread>(threadsNum);
            _workingThreads = new List<WorkThread>(threadsNum);

            for (int i = 0; i < threadsNum; i++)
            {
                var wt = new WorkThread();
                wt.WorkDone += SubThreadWorkDoneHandler;
                _idleThreads.Add(wt);
            }
        }

        private void SubThreadWorkDoneHandler(WorkThread workThread)
        {
            if (_actionsQueue.TryDequeue(out Action result))
            {
                workThread.Do(result);
                return;
            }

            lock (_locker)
            {
                _workingThreads.Remove(workThread);
                _idleThreads.Add(workThread);
                if(_actionsQueue.IsEmpty && _workingThreads.Count == 0)
                    TaskManagerWorkDone?.Invoke();
            }
        }


        public void AddTask(Action action)
        {
            lock (_locker)
            {
                if (_idleThreads.Count > 0)
                {
                    WorkThread wt = _idleThreads[_idleThreads.Count - 1];
                    _idleThreads.Remove(wt);
                    _workingThreads.Add(wt);
                    wt.Do(action);
                }
                else
                {
                    _actionsQueue.Enqueue(action);
                }

            }
        }
    }
}
