using System;
using System.Collections.Generic;
using System.Threading;
using WindowsInput;
using WindowsInput.Events;
using WindowsInput.Events.Sources;

namespace ConsolePOE
{
    internal class Bot
    {
        IKeyboardEventSource kb;
        bool AllowFire = false;

        Thread ManaJob;
        Thread SpeedJob;
        Thread KeyboardQJob;
        List<Thread> Jobs;

        public Bot()
        {
            kb = Capture.Global.KeyboardAsync();
            kb.KeyDown += Kb_KeyDown;
            kb.KeyUp += Kb_KeyUp;

            ManaJob = new Thread(DrinkManaFlask) { IsBackground = true };
            SpeedJob = new Thread(DrinkSpeedFlask) { IsBackground = true };
            KeyboardQJob = new Thread(ExecuteSkillQ) { IsBackground = true };
            Jobs = new List<Thread>() { ManaJob, SpeedJob, KeyboardQJob };
        }


        private void Kb_KeyDown(object sender, WindowsInput.Events.Sources.EventSourceEventArgs<WindowsInput.Events.KeyDown> e)
        {
            if (e.Data.Key == KeyCode.Oemtilde)
                AllowFire = true;
        }
        private void Kb_KeyUp(object sender, WindowsInput.Events.Sources.EventSourceEventArgs<WindowsInput.Events.KeyUp> e)
        {
            if (e.Data.Key == KeyCode.Oemtilde)
                AllowFire = false;
        }

        internal virtual void Start()
        {
            foreach (var t in Jobs)
                t.Start();
        }

        internal virtual void Stop()
        {
            kb.Dispose();
        }

        private void DrinkManaFlask()
        {
            var keys = new List<KeyLife>() {
                new KeyLife(KeyCode.D2, 6000, callback: KeyLife_OnFire),
                new KeyLife(KeyCode.D3, 7000, callback: KeyLife_OnFire),
                new KeyLife(KeyCode.D4, 8000, callback: KeyLife_OnFire),
                new KeyLife(KeyCode.D5, 9000, callback: KeyLife_OnFire),
            };
            while (true)
            {
                foreach (var key in keys)
                {
                    if (!AllowFire) break;
                    if (key.Fire())
                        Thread.Sleep(1000);
                }
                Thread.Sleep(100);
            }
        }

        private void DrinkSpeedFlask()
        {

        }

        private void ExecuteSkillQ()
        {
            var keys = new List<KeyLife>() {
                new KeyLife(KeyCode.Q, 30000, callback: KeyLife_OnFire),
            };
            while (true)
            {
                foreach (var key in keys)
                {
                    if (!AllowFire) break;
                    if (key.Fire())
                        Thread.Sleep(1000);
                }
                Thread.Sleep(100);
            }
        }

        private void KeyLife_OnFire(KeyLife sender, KeyCode key)
        {
            Simulate.Events().Click(key).Invoke();
        }
    }
}