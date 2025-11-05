using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}


public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    public float playerTiltAngle;
    public Boundary boundary;
    public Transform bulletManager;

    public Material[] material;
    public GameObject shipMesh;


    public Renderer shipRenderer;

    //public GameObject asteroidExplosion;
    public GameObject playerExplosion;

    public GameObject bullet;
    public Transform bulletSpawn;
    public float fireRate;
        
    private float nextFire;

    private Rigidbody playerRB;

    private SFXManager sfxManager;
    public GameManager gameManager;

    void Awake()
    {
        sfxManager = (GameObject.Find("SFXManager").GetComponent<SFXManager>());
        playerRB = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        shipRenderer = shipMesh.GetComponent<Renderer>();
        shipRenderer.enabled = true;
        shipRenderer.sharedMaterial = material[0];
    }

    void Update()
    {
        //shoot

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bulletClone = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
            bulletClone.transform.SetParent(bulletManager);
            sfxManager.PlayerShoot();            
        }
    }
    
    void FixedUpdate()
    {
        //movement

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        playerRB.linearVelocity = movement * playerSpeed;

        playerRB.position = new Vector3
        (
            Mathf.Clamp(playerRB.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(playerRB.position.z, boundary.zMin, boundary.zMax)
        );        

        playerRB.rotation = Quaternion.Euler(Vector3.forward * moveHorizontal * -playerTiltAngle);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hazard")
        {
            gameManager.shield -= 1;
            if (gameManager.shield > -1)
            {
                sfxManager.PlayerDamage();
                StartCoroutine(Flasher());
            }
        }
    }

    IEnumerator Flasher()
    {
        for (int i = 0; i < 4; i++)
        {
            shipRenderer.sharedMaterial = material[1];
            yield return new WaitForSeconds(0.05f);
            shipRenderer.sharedMaterial = material[0];
            yield return new WaitForSeconds(0.05f);
        }
    }
    

    public void PlayerDestroy()
    {
        gameObject.SetActive(false);
        Instantiate(playerExplosion, this.transform.position, this.transform.rotation);
        sfxManager.PlayerExplosion();
        shipRenderer.sharedMaterial = material[0];
    }

    


}
