using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCompassEnemys : MonoBehaviour
{
    public GameObject pointerPrefab;

    // Start is called before the first frame update
    private List<GameObject> enemes;
    private Dictionary<GameObject, GameObject> pointersEnemys;

    private ShipCompass shipCompass;
    void Start()
    {
        shipCompass = GetComponent<ShipCompass>();
        enemes = GameManager.QuestManagerEp1.enemes;
        pointersEnemys = new Dictionary<GameObject, GameObject>();
        foreach (GameObject enemy in enemes)
        {
            GameObject _pointer = Instantiate(pointerPrefab, transform.position, Quaternion.identity, transform);
            _pointer.SetActive(false);
            pointersEnemys.Add(_pointer, enemy);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowEnabledEnemys();
    }

    private void ShowEnabledEnemys()
    {
        foreach (KeyValuePair<GameObject, GameObject> entry in pointersEnemys)
        {
            GameObject _pointer = entry.Key;
            GameObject _enemy = entry.Value;
            EnemyShipAI _shipAI = _enemy.GetComponent<EnemyShipAI>();
            if (_enemy != null && !_shipAI.IsDead)
            {
                if (_shipAI.isActiveAndEnabled)
                {
                    _pointer.SetActive(true);
                    _pointer.transform.position = (_enemy.transform.position - transform.position).normalized * shipCompass.radius + transform.position;
                }
                else
                {
                    _pointer.SetActive(false);
                }
            }
            else
            {
                pointersEnemys.Remove(_pointer);
                Destroy(_pointer);
                continue;
            }
        }
    }
}
