using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput.Events;
using WindowsInput.Native;

namespace ConsolePOE
{
    class KeyLife
    {
        public KeyCode KeyCode;
        public int LifeCycle;
        public int RandomCycle;
        public DateTime NextFireTime;

        public delegate void FireHandler(KeyLife sender, KeyCode key);

        public event FireHandler OnFire;

        public KeyLife(KeyCode key, int lifeMs, int rndMs = 500, FireHandler callback = null)
        {
            this.KeyCode = key;
            this.LifeCycle = lifeMs;
            this.RandomCycle = rndMs;
            this.NextFireTime = DateTime.Now;
            this.OnFire = callback;
        }

        public bool Fire()
        {
            if (DateTime.Now.CompareTo(this.NextFireTime) >= 0)
            {
                var ms = this.LifeCycle + new Random().Next(this.RandomCycle);
                this.NextFireTime = DateTime.Now.AddMilliseconds(ms);
                if (OnFire != null)
                    OnFire(this, this.KeyCode);
                return true;
            }
            else
                return false;
        }
    }
}
