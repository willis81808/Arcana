using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBurst : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToSpawn;

    [SerializeField]
    private bool randomizeDirection, distributeRotation, removeSpawnedOnDestroy;

    [SerializeField]
    private float spawnOverSeconds = 0f, rotationDistribution = 360f;

    [SerializeField]
    private int numToSpawn;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    IEnumerator Start()
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            var obj = Instantiate(objectToSpawn, transform.position, objectToSpawn.transform.rotation);
            spawnedObjects.Add(obj);

            if (randomizeDirection)
            {
                var rot = obj.transform.eulerAngles;
                rot.z = Random.Range(0, 360);
                obj.transform.eulerAngles = rot;
            }

            if (distributeRotation)
            {
                var rot = obj.transform.eulerAngles;
                rot.z = rotationDistribution * (i / (float)numToSpawn);
                obj.transform.eulerAngles = rot;
            }

            yield return new WaitForSeconds(spawnOverSeconds / numToSpawn);
        }
    }

    private void OnDestroy()
    {
        if (!removeSpawnedOnDestroy) return;

        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
        }
    }
}
