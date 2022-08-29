using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BixaoController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    AudioSource gritoSrc;
    bool grito = false;
    bool walkMap = false;
    string playerTag = "Player";
    Quaternion dirDireita = Quaternion.Euler(0, 90, 0);
    Quaternion dirEsquerda = Quaternion.Euler(0, -90, 0);

    //Estados
    int isIdleHash;
    int isScreamingHash;
    int isChasingHash;
    int isAgonyHash;

    Rigidbody rb;
    Vector3 distPlayer;
    Vector3 dirCaminhada;
    Vector3 ultPosicao;
    Vector3 pontoAleatório;
    System.Random rd = new System.Random();

    [SerializeField]
    public GameObject player;
    [SerializeField]
    public float speed;
    [SerializeField]
    public float chaseDistance = 25f;
    [SerializeField]
    public AudioClip gritoClip;
    [SerializeField]
    public AudioClip jumpScare;
    [SerializeField]
    public LevelRenderer lvlRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        isScreamingHash = Animator.StringToHash("isScreaming");
        isChasingHash = Animator.StringToHash("isChasing");

        gritoSrc = GetComponent<AudioSource>();

        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        bool isScreaming = animator.GetBool(isScreamingHash);
        bool isChasing = animator.GetBool(isChasingHash);
        RaycastHit hitData;

        distPlayer = (player.transform.position - transform.position);

        agent.enabled = true;

        if (distPlayer.magnitude <= chaseDistance)
        {
            //traca um raio e verifica se ha interseccao com alguma parede. Caso nao haja -> grita e comeca perseguicao
            Ray raioPlayer = new Ray(transform.position, player.transform.position - transform.position);
            Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.magenta);

            if (Physics.Raycast(raioPlayer, out hitData, chaseDistance))
            {
                if (hitData.collider.CompareTag(playerTag))
                {
                    transform.LookAt(player.transform);
                    if (!grito)
                    {
                        gritoSrc.PlayOneShot(gritoClip);
                        grito = true;
                        animator.SetBool(isScreamingHash, true);
                        Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                        WaitCoroutine(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                        animator.SetBool(isScreamingHash, false);
                    }

                    animator.SetBool(isChasingHash, true);

                    agent.enabled = false;
                    rb.velocity = distPlayer.normalized * speed * Time.deltaTime;

                    walkMap = false;
                }
                else
                {
                    agent.enabled = true;
                    agent.SetDestination(player.transform.position);
                    agent.speed = speed / 3;
                    walkMap = true;
                }
                // else if (hitData.collider.CompareTag("Parede"))
                // {
                //     Debug.Log("procurando player");
                //     RaycastHit dirHit;
                //     RaycastHit esqHit;
                //     Ray raioDireita = new Ray(transform.position, dirDireita * distPlayer);
                //     Ray raioEsquerda = new Ray(transform.position, dirEsquerda * distPlayer);

                //     Debug.DrawRay(transform.position, dirDireita * distPlayer, Color.green);
                //     Debug.DrawRay(transform.position, dirEsquerda * distPlayer, Color.green);

                //     Physics.Raycast(raioDireita, out dirHit, chaseDistance * 4);
                //     Physics.Raycast(raioEsquerda, out esqHit, chaseDistance * 4);
                //     int dirMultiplier = dirHit.distance >= esqHit.distance ? 1 : -1;
                //     rb.velocity = (Quaternion.Euler(0, dirMultiplier * 90, 0) * distPlayer.normalized) * speed / 2 * Time.deltaTime;
                // }
            }
        }
        else
        {
            // rb.velocity = dirCaminhada * speed / 4 * Time.deltaTime;
            if (isChasing)
            {
                animator.SetBool(isChasingHash, false);
                rb.velocity = Vector3.zero;
                grito = false;
            }
            else
            {
                if (walkMap)
                {
                    pontoAleatório = new Vector3(
                        -lvlRenderer.width / 2 + (rd.Next(0, lvlRenderer.width * Mathf.FloorToInt(lvlRenderer.tamParede)) - lvlRenderer.tamParede / 2),
                        transform.position.y,
                        -lvlRenderer.height / 2 + (rd.Next(0, lvlRenderer.height * Mathf.FloorToInt(lvlRenderer.tamParede)) - lvlRenderer.tamParede / 2));
                    walkMap = false;
                }
                agent.speed = speed / 10;
                agent.SetDestination(pontoAleatório);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Parede"))
        {
            if (dirCaminhada == transform.forward)
                dirCaminhada = Quaternion.Euler(0, 90, 0) * transform.forward;
            else if (dirCaminhada == Quaternion.Euler(0, 90, 0) * transform.forward)
                dirCaminhada = Quaternion.Euler(0, -90, 0) * transform.forward;
            else
                dirCaminhada = transform.forward;
        }
        else if (other.CompareTag(playerTag))
        {
            SceneManager.LoadScene("JumpScare");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Colisao com bixao");
        //Roda animacao de ataque + som de jumpscare
        // if (collision.collider.CompareTag("Player"))
        // {
        //     SceneManager.LoadScene("Menu");
        // }
    }

    private IEnumerator WaitCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void ListenAnimation(string msg)
    {
        if (msg.Equals("JumpscareEnd"))
        {
            // Debug.Log("Fim jumpscare");
            SceneManager.LoadScene("GameOver");
        }
    }
}
