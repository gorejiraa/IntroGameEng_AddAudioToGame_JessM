using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject hazard;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public int waveWait;
    public int waveCount = 0;

    public bool spawn = true;
       

    void Start()
    {        
        StartCoroutine( SpawnWaves() );        
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
           
        while (spawn == true) 
        {

            for (int w = 0; w < 100; w++)
            {
                waveCount = w;
                hazardCount = hazardCount + 2;
                float hazardSpeed = -5 + (-1f * waveCount);

                for (int i = 0; i < hazardCount; i++)
                {
                    Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                    Quaternion spawnRotation = Quaternion.identity; // no rotation, rotation is set on the asteroid script itself.
                    GameObject asteroidClone = Instantiate(hazard, spawnPosition, spawnRotation);

                    asteroidClone.transform.SetParent(this.transform); //organises all asteroids under AsteroidSpawner in hierachy.

                    Mover asteroidStats = asteroidClone.GetComponent<Mover>();
                    asteroidStats.speed = hazardSpeed;                    

                    yield return new WaitForSeconds(spawnWait);
                }
                yield return new WaitForSeconds(waveWait);

            }

            

        }
    }

    
}
