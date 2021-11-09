using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
namespace Invector.vCharacterController.AI
{
#if UNITY_EDITOR
    public class ReadOnlyAttribute : PropertyAttribute
    {

    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
    public class NPCSpawner_ : MonoBehaviour
    {
        [System.Serializable]
        public class Wave
        {
            //#if UNITY_EDITOR
            public string Name;
            //#endif
            public Transform spawnPoint;
            public GameObject prefab;
            public int order;
            public vWaypointArea waypointArea;
            public int npcCount;
            public float delayBetweenSpawn;
            public float waveFinishedDelay = 10;

#if UNITY_EDITOR
            [ReadOnly]
#endif
            public int activeNpcCount;

            [HideInInspector]
            public GameObject[] npcList;
            [HideInInspector]
            public bool waveStarted;
            [HideInInspector]
            public bool waveFinished;
#if UNITY_EDITOR
            [ReadOnly]
#endif  
            public float timerBetweenSpawn;
#if UNITY_EDITOR
            [ReadOnly]
#endif
            public float timerWaveFinishedDelay;

            public UnityEvent onSpawn;

            private int currentBotNumber = 0;

            public void CreateNPCs()
            {
                npcList = new GameObject[npcCount];
                for(int i = 0; i < npcCount; i++)
                {
                    GameObject npc = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                    npc.name = Name + "_" + i;
                    npc.SetActive(false);
                    vControlAIShooter ai = npc.GetComponent<vControlAIShooter>();
                    ai.waypointArea = waypointArea;
                    npcList[i] = npc;
                }
            }

            public void Update(int order)
            {
                if (!waveStarted) return;
                if(this.order == order) {
                    if (currentBotNumber >= npcCount)
                    {
                        if(timerWaveFinishedDelay >= waveFinishedDelay)
                        {
                            waveFinished = true;
                        }
                        else
                        {
                            timerWaveFinishedDelay += Time.deltaTime;
                        }
                        
                        return;
                    }
                    if(currentBotNumber == 0)
                    {
                        npcList[0].SetActive(true);
                        onSpawn.Invoke();
                        currentBotNumber++;
                        return;
                    }

                    if(timerBetweenSpawn >= delayBetweenSpawn)
                    {
                        npcList[currentBotNumber].SetActive(true);
                        onSpawn.Invoke();
                        currentBotNumber++;
                        timerBetweenSpawn = 0;
                    }
                    
                    timerBetweenSpawn += Time.deltaTime;
                }
                else
                {
                    return;
                }

            }
            public void ckeckNpc()
            {
                activeNpcCount = 0;
                foreach(GameObject npc in npcList)
                {
                    if (!npc.active) continue;
                    vControlAIShooter ai = npc.GetComponent<vControlAIShooter>();
                    
                    if(ai != null)
                    {
                        activeNpcCount++;
                    }
                }
            }
        }

        public bool startSpawn;
        private int currentWave = -1;
        
        public Wave[] waves;

        


        // Start is called before the first frame update
        void Start()
        {
            GameManager.QuestManagerEp2.OnActionTowerCrane_Grab += OnActionTowerCrane_Grab;
            foreach (Wave w in waves)
            {
                w.CreateNPCs();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (startSpawn)
            {
                currentWave = 0;
                foreach (Wave w in waves)
                {
                    if (w.order == 0) w.waveStarted = true;
                }
                startSpawn = false;
            }
            if (currentWave >= waves.Length || currentWave < 0) return;

            bool allWaveFinished = false;
            foreach (Wave w in waves)
            {
                if (w.order == currentWave)
                {
                    allWaveFinished = w.waveFinished;
                }

                w.Update(currentWave);
            }

            if (allWaveFinished)
            {
                currentWave++;
                foreach (Wave w in waves)
                {
                    if (w.order == currentWave) w.waveStarted = true;
                }
            }
        }

        private bool isWaveFinished()
        {
            bool allWaveFinished = false;
            foreach(Wave w in waves)
            {
                if(w.order == currentWave)
                {
                    allWaveFinished = w.waveFinished;
                }
            }

            return allWaveFinished;
        }

        private void OnActionTowerCrane_Grab()
        {
            startSpawn = true;
            GameManager.QuestManagerEp2.OnActionTowerCrane_Grab -= OnActionTowerCrane_Grab;
        }

    }
}
