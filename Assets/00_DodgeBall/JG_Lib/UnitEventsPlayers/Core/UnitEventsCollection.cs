using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class UnitEventsCollection : MonoBehaviour
    {
        public event Action onSequenceCompleted = null;
        public bool IsPlaying { private set; get; } = false;

        [SerializeField] bool playOnEnable = false;
        [SerializeField] bool runInSequence = false;

        List<UnitPlayable> playables = new List<UnitPlayable>();
        Coroutine loop = null;
        int completionsCount = 0;

        protected virtual void Awake()
        {
            playables = GetComponents<UnitPlayable>().ToList();
            playables.Sort((x, y) => x.ID.CompareTo(y.ID));
            foreach(UnitPlayable p in playables)
            {
                p.onCompleted += () => completionsCount = completionsCount + 1;
            }
        }
        protected virtual void OnEnable()
        {
            if (playOnEnable)
            {
                PlayEvents();
            }
        }
        public virtual void PlayEvents()
        {
            completionsCount = 0;
            if (!runInSequence)
            {
                foreach (UnitPlayable playable in playables)
                {
                    StartCoroutine(playable.Play());
                }
                return;
            }
            loop = StartCoroutine(PlayEventsInSequence());
        }
        public void StartLoop()
        {
            StopLoop();
            loop = StartCoroutine(PlayEventsInLoop());
        }
        public void StopLoop()
        {
            completionsCount = 0;
            if (loop != null)
            {
                StopCoroutine(loop);
                loop = null;
            }
        }
        private IEnumerator PlayEventsInLoop()
        {
            while (true)
            {
                if (runInSequence)
                {
                    yield return PlayEventsInSequence();
                }
                else
                {
                    PlayEvents();
                    WaitUntil w = new WaitUntil(()=>completionsCount>=playables.Count);

                    yield return w;
                }
            }
        }
        private IEnumerator PlayEventsInSequence()
        {
            IsPlaying = true;
            foreach (UnitPlayable playable in playables)
            {
                yield return StartCoroutine(playable.Play());
            }
            IsPlaying = false;
            onSequenceCompleted?.Invoke();
        }
    }
}