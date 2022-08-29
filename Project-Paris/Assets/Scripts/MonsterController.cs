using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public GameObject player;
    public PlayerController plCtrl;
    public float monsterSpeed;
    public Rigidbody monsterRb;

    Vector3 distPlayer = new Vector3();
    // string monsterState;

    private void Start()
    {
        // monsterState = "Spawning";
        monsterRb = GetComponent<Rigidbody>();
        monsterRb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        distPlayer = player.transform.position - transform.position;
        if (distPlayer.magnitude <= 25f)
        {
            //Roda audio do monstro gritando
            // monsterState = "Chase";
            monsterRb.velocity = distPlayer.normalized * monsterSpeed * Time.deltaTime * 25;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            plCtrl.Hit();
        }
    }
}
