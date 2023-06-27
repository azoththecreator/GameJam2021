using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools = new List<Pool>();
    public Dictionary<string, Queue<GameObject>> poolDic = new Dictionary<string, Queue<GameObject>>();

    public static ObjectPool instance = null;
    [HideInInspector]
    public bool isCheck = false;

    private void Awake()
    {
        instance = this;

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDic.Add(pool.tag, objectPool);
        }
    }
    public GameObject Spawn(string tag)
    {
        GameObject spawnObj = poolDic[tag].Dequeue();
        spawnObj.SetActive(true);
        spawnObj.transform.rotation = Quaternion.identity;

        poolDic[tag].Enqueue(spawnObj);

        return spawnObj;
    }
    public GameObject SpawnUI(string tag, Transform parent, Vector2 pos, string tagChange)
    {
        GameObject spawnObj = poolDic[tag].Dequeue();
        spawnObj.SetActive(true);
        spawnObj.transform.SetParent(parent);
        spawnObj.transform.localPosition = pos;
        spawnObj.transform.localScale = Vector3.one;
        spawnObj.transform.rotation = Quaternion.identity;
        spawnObj.tag = tagChange;

        poolDic[tag].Enqueue(spawnObj);

        return spawnObj;
    }
}
