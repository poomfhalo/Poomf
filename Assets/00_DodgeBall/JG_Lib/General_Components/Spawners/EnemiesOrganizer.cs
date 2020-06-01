using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    [Serializable]
    public class Wave
    {
        enum ProgressMode{Ordered,Loop,Random}
        enum WaveType{Enabler,Spawner}
        [Header("Events")]
        [SerializeField] UnitEventsCollection onWaveFinishedSpawning = null;
        [SerializeField] UnitEventsCollection onWaveCleared = null;

        [Header("Core Settings")]
        [SerializeField] WaveType waveType = WaveType.Enabler;
        [SerializeField] List<GameObject> points = new List<GameObject>();
        [SerializeField] MinMaxRange timeBetweenEnemies = MinMaxRange.DefaultPositive;

        [SerializeField] ProgressMode progressMode = ProgressMode.Random;

        [Header("Spawner Settings")]
        [SerializeField] GameObject enemyPrefab = null;
        [SerializeField] int enemiesToSpawnCount = 3;

        [Header("Spawn Effects")]
        [SerializeField] EnemySpawner uniqueSpawner = null;

        [Header("Read Only")]
        [SerializeField] int enemiesLost = 0;

        //Timer Variables
        float waveTimeCounter = 0;
        float currentTimeBetweenEnemies = 0;
        int currentPoint = 0;
        
        //Spawner Variables
        int currentSpawnedCount = 0;
        
        //Enabler Variables
        const int randomMaxTries = 30;
        List<int> unVisitedPoints = new List<int>();
        EnemySpawner organizerSpawner = null;
        EnemySpawner enemySpawner
        {
            get
            {
                if(uniqueSpawner == null)
                {
                    return organizerSpawner;
                }
                return uniqueSpawner;
            }
        }


        public void Init(EnemySpawner organizerSpawner)
        {
            this.organizerSpawner = organizerSpawner;
            currentTimeBetweenEnemies = timeBetweenEnemies.GetValue();
            for (int i = 0;i<points.Count;i++)
            {
                unVisitedPoints.Add(i);
                if(waveType == WaveType.Enabler)
                {
                    GameObject point = points[i];
                    point.SetActive(false);
                }
            }
        }
        public bool Update()
        {
            waveTimeCounter = waveTimeCounter + Time.deltaTime/currentTimeBetweenEnemies;
            if(waveTimeCounter<1)
            {
                return false;
            }

            waveTimeCounter = 0;
            currentTimeBetweenEnemies = timeBetweenEnemies.GetValue();

            if(IsAviliable() == false)
            {
                return false;
            }

            if (waveType == WaveType.Spawner)
            {
                ProgressSpawner();
            }
            else if(waveType == WaveType.Enabler)
            {
                ProgressEnabler();
            }
            return true;
        }
        public bool AreAllEnemiesDead()
        {
            int totalEnemiesCount = 0;
            if (waveType == WaveType.Enabler)
            {
                totalEnemiesCount = points.Count;
            }
            else if(waveType == WaveType.Spawner)
            {
                totalEnemiesCount = enemiesToSpawnCount;
            }
            return enemiesLost>=totalEnemiesCount;
        }
        private bool IsAviliable()
        {
            if(waveType == WaveType.Spawner)
            {
                return IsSpawnerAviliable();
            }
            else if(waveType == WaveType.Enabler)
            {
                return IsEnablerAviliable();
            }
            return false;
        }

        private void ProgressSpawner()
        {
            Transform p = GetCurrentPoint();
            GameObject enemy = enemySpawner.SpawnGameObject(enemyPrefab,p);

            enemiesToSpawnCount = enemiesToSpawnCount + 1;

            //TODO: connect to health
            //Health health = enemy.GetComponent<Health>();
            //health.OnZeroEvent += OnEnemyDied;
            //enemy.GetComponent<vHealthController>().onDead.AddListener((g)=> OnEnemyDied());

            if (IsSpawnerAviliable() == false)
            {
                onWaveFinishedSpawning?.PlayEvents();
            }
        }

        private void ProgressEnabler()
        {
            Transform p = GetCurrentPoint();
            GameObject enemy = enemySpawner.EnableGameObject(p.gameObject,p);

            int pointIndex = points.IndexOf(p.gameObject);
            unVisitedPoints.Remove(pointIndex);

            //TODO: connect to health
            //Health health = enemy.GetComponent<Health>();
            //health.OnZeroEvent += OnEnemyDied;
            //enemy.GetComponent<vHealthController>().onDead.AddListener((g)=>OnEnemyDied());

            if(IsEnablerAviliable()==false)
            {
                onWaveFinishedSpawning?.PlayEvents();
            }
        }

        private bool IsSpawnerAviliable()
        {
            return currentSpawnedCount<enemiesToSpawnCount;
        }
        private bool IsEnablerAviliable()
        {
            return unVisitedPoints.Count != 0;
        }
        private Transform GetCurrentPoint()
        {
            GameObject p =  points[currentPoint];

            if (progressMode == ProgressMode.Ordered)
            {
                currentPoint = currentPoint + 1;
            }
            else if(progressMode == ProgressMode.Loop)
            {
                currentPoint = (currentPoint + 1) % points.Count;
            }
            else if(progressMode == ProgressMode.Random)
            {
                currentPoint = GetNewRandomPointIndex();
            }
            currentPoint = Mathf.Clamp(currentPoint,0,points.Count);

            return p.transform;
        }

        private int GetNewRandomPointIndex()
        {
            int randPointIndex = UnityEngine.Random.Range(0,points.Count);
            if (waveType == WaveType.Enabler)
            {
                int triesCounter = 0;
                while(unVisitedPoints.IndexOf(randPointIndex) == -1 && triesCounter<randomMaxTries)
                {
                    int unVisitedPointRandIndex = UnityEngine.Random.Range(0,unVisitedPoints.Count);
                    randPointIndex = unVisitedPoints[unVisitedPointRandIndex];
                    triesCounter = triesCounter + 1;
                }
            }
            return randPointIndex;
        }

        private void OnEnemyDied()
        {
            enemiesLost = enemiesLost + 1;

            if(AreAllEnemiesDead())
            {
                onWaveCleared?.PlayEvents();
            }
        }
    }

    [RequireComponent(typeof(EnemySpawner))]
    public class EnemiesOrganizer : MonoBehaviour
    {
        public int CurrentWave=>currentWave;

        [Header("Events")]
        [SerializeField] UnitEventsCollection onOrganizerFinished = null;
        public event Action OnOrganizerFinished = null;

        [Header("Core Settings")]
        [SerializeField] Wave[] waves = new Wave[0];
        [SerializeField] MinMaxRange timeBetweenWaves = MinMaxRange.DefaultPositive;
        [SerializeField] bool runOnStart = false;

        [Header("Read Only")]
        [SerializeField] int currentWave = 0;
        [SerializeField] bool isActive = false;
        [SerializeField] bool autoAdvance = false;

        float counter = 0;
        float currentTimeBetweenWaves = 0;
        bool movingToNextWave = false;
        bool isOutOfWaves => currentWave < 0 || currentWave >= waves.Length;

        public bool IsActive => isActive;
        public bool AutoAdvance => autoAdvance;

        public bool IsCurrentWaveCompleted => GetCurrentWave()==null?false:GetCurrentWave().AreAllEnemiesDead();
        EnemySpawner spawner = null;

        void Awake()
        {
            spawner = GetComponent<EnemySpawner>();
        }
        IEnumerator Start()
        {
            yield return 0;//Wait untill the enemy is fully initialized (its start has ran).
            //TODO: only initialize, if we are not in the process of loading data to the level, otherwise, use Restore Data
            InitializeWaves();
        }

        public void RestoreData(int currentWave,bool isActive,bool autoAdvance,bool isCurrWaveCleared)
        {
            this.currentWave = currentWave;
            if (isCurrWaveCleared && isActive)
            {
                this.currentWave = this.currentWave + 1;
            }
            this.isActive = isActive;
            this.autoAdvance = autoAdvance;
            InitializeWaves();
        }
        private void InitializeWaves()
        {
            for (int i = currentWave; i < waves.Length;i++)
            {
                Wave w = waves[i];
                w.Init(spawner);
            }

            currentTimeBetweenWaves = timeBetweenWaves.GetValue();
            if(runOnStart)
            {
                ActivateOrganizer(autoAdvance);
            }
        }

        private void Update()
        {
            if(isActive == false || GetCurrentWave() == null)
            {
                return;
            }

            if (autoAdvance && GetCurrentWave().AreAllEnemiesDead())
            {
                movingToNextWave = true;
            }

            if(movingToNextWave)
            {
                counter = counter + Time.deltaTime/currentTimeBetweenWaves;
                if (counter<1)
                {
                    return;
                }

                counter = 0;
                currentTimeBetweenWaves = timeBetweenWaves.GetValue();
                currentWave = currentWave + 1;
                movingToNextWave = false;

                if(isOutOfWaves)
                {
                    onOrganizerFinished?.PlayEvents();
                    OnOrganizerFinished?.Invoke();
                }
            }
            else
            {
                GetCurrentWave().Update();
            }
        }

        private Wave GetCurrentWave()
        {
            if (isOutOfWaves)
            {
                return null;
            }
            return waves[currentWave];
        }
        public void CallMoveToNextWave()
        {
            movingToNextWave = true;
        }
        public void ActivateOrganizer(bool autoAdvance)
        {
            isActive = true;
            this.autoAdvance = autoAdvance;
        }
        public void DeActivateOrganizer()
        {
            isActive = false;
        }
    }
}