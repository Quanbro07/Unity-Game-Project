using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints;
    public List<GameObject> propPrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnProps();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            int rand = Random.Range(0, propPrefabs.Count);
            GameObject obj =Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity);
            obj.transform.parent = sp.transform;
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Object"; // thay "Props" bằng layer bạn cần
                sr.sortingOrder = 2; // chỉnh số lớn hơn background
            }
        }
    }
}
