using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{
    using Objects;
    public class Timer
    {
        public Timer(float interval,int repeatcount,Action func)
        {
            _interval = interval;
            _repeatcount = repeatcount;
            _schedule_func = func;
            if (_repeatcount == 0)
                _runforever = true;
        }
        private float _interval;
        private float _elspase;
        private int _repeatcount;
        private bool _runforever;
        private bool _paused;
        private Action _schedule_func;
        private bool _unschedule;
        public bool Paused { get => _paused; set => _paused = value; }
        public float Elapse { get => _elspase; set => _elspase = value; }
        public float Interval => _interval;
        public bool Runforever => _runforever;
        public Action Func => _schedule_func;
        public int RepeatCount { get => _repeatcount; set => _repeatcount = value; }
        public bool Unschedule { get => _unschedule; set => _unschedule = value; }
    }
    public class Scheduler : ReferPlayer
    {
        private int _keyindex;
        private Dictionary<int, Timer> _willscheduletimers = new Dictionary<int, Timer>();
        private Dictionary<int, Timer> _timers = new Dictionary<int, Timer>();
        private Dictionary<int, Timer> _umscheduletimers = new Dictionary<int, Timer>();
        private int InnerKeyMaker()
        {
            _keyindex++;
            return _keyindex;
        }
        private Timer InnerMakeTimer(Action func,float interval,int repeatcount)
        {
            var timer = new Timer(interval,repeatcount,func);
            return timer;
        }
        private void InnterScheduler(Action func,float interval,int repeatcount)
        {
            var v = InnerMakeTimer(func, interval, repeatcount);
            var key = InnerKeyMaker();
            _willscheduletimers.Add(key, v);
        }
        private void InnterRunTimer(Timer timer,float dt)
        {
            if (!timer.Paused)
            {
                timer.Elapse += dt;
                if (timer.Interval<=timer.Elapse)
                {
                    timer.Func?.Invoke();
                    timer.Elapse = 0;
                    if (!timer.Runforever)
                    {
                        timer.RepeatCount -= 1;
                    }
                }
            }
        }
        private void InnerUnSchedule(int key)
        {
            if (_timers.ContainsKey(key))
                _timers[key].Unschedule = true;
        }
        private void InnerCancelCheckTimner(int key,Timer timer)
        {
            if (!timer.Runforever)
                if (timer.RepeatCount <= 0)
                    InnerUnSchedule(key);
        }
        public void Schedule(Action func)
        {
            InnterScheduler(func, 0.0f, 0);
        }
        public void ScheduleOnce(Action func)
        {
            InnterScheduler(func, 0.0f, 1);
        }
        public void ScheduleInterval(Action func,float interval)
        {
            InnterScheduler(func, interval, 0);
        }
        public void ScheduleDetail(Action func,float interval,int repeat)
        {
            InnterScheduler(func, interval, repeat);
        }
        public void Update(float delta)
        {
            foreach(var wtimer in _willscheduletimers)
            {
                _timers.Add(wtimer.Key, wtimer.Value);
            }
            _willscheduletimers.Clear();
            foreach (var timer in _timers)
            {
                InnterRunTimer(timer.Value, delta);
                InnerCancelCheckTimner(timer.Key, timer.Value);
                if (timer.Value.Unschedule)
                    _umscheduletimers.Add(timer.Key, timer.Value);
            }
            foreach(var unschedule in _umscheduletimers)
            {
                _timers.Remove(unschedule.Key);
            }
            _umscheduletimers.Clear();
        }
    }
}