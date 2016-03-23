﻿using UnityEngine;
using System.Collections;
using SupHero;
using SupHero.Model;

namespace SupHero.Components {
    public class Timer : MonoBehaviour {

        public float time = 30f;
        private bool isRunning = false;

        // Events
        public delegate void startAction();
        public delegate void tickAction();
        public delegate void endAction();
        public event startAction OnStart;
        public event tickAction OnTick;
        public event endAction OnEnd;

        // Update is called once per frame
        void Update() {
            if (isRunning) {
                if (time > 0) {
                    // Ticking the clock
                    if (OnTick != null) OnTick();
                    time -= Time.deltaTime;
                    /*if ((time % 1) == 0) {
                        if (OnTick != null) OnTick();
                    }*/
                }
                else {
                    // Time is ended
                    if (OnEnd != null) OnEnd();
                    isRunning = false;
                    Destroy(this);
                }
            }
        }

        public void refresh() {
            time = 30f;
        }

        public void launch() {
            if (OnStart != null) OnStart();
            isRunning = true;
        }
    }
}
