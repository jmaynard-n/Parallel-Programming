using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TaskManagerRefact.Core
{
    public class WorkThread
    {
        private readonly AutoResetEvent _waitHandler = new AutoResetEvent(false);
        private readonly Thread _thread;
        private bool _needToTerminate = false;
        private WorkThreadState _state;

        // Объявляем делегат
        public delegate void WorkDoneHandler(WorkThread workThread);
        // Событие, возникающее при выводе денег
        public event WorkDoneHandler WorkDone;

        private Action _action;
        public WorkThread()
        {
            _thread = new Thread(WorkLoop);
            _thread.IsBackground = false;
            _state = WorkThreadState.Idle;
            _thread.Start();
        }

        public void Do(Action action)
        {
            if (_state != WorkThreadState.Idle)
                throw new Exception($"Thread in state {_state}");
            _state = WorkThreadState.Working;
            _action = action;
            _waitHandler.Set();
        }

        public void Terminate()
        {
            if (_state != WorkThreadState.Idle)
                throw new Exception($"Thread in state {_state}");
            _needToTerminate = true;
            _waitHandler.Set();
        }

        public void WorkLoop()
        {
            do
            {
                _waitHandler.WaitOne();
                if (_needToTerminate)
                {
                    _state = WorkThreadState.Terminated;
                    break;
                }
                    

                _action.Invoke();

                _waitHandler.Reset();
                _state = WorkThreadState.Idle;
                WorkDone?.Invoke(this);

            }while(!_needToTerminate);

        }

    }

    public enum WorkThreadState
    {
        Working = 1,
        Idle = 2,
        Terminated = 3
    }
}
