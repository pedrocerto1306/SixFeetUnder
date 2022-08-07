using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public CatacombScript catacomb;
    public float speedInput;
    public float jumpAmout;
    public float jumpHeight = 3f;
    public int skullAmount;
    public GameObject flashLight;
    public TMP_Text lblBattery;
    public TMP_Text lblDepth;
    public TMP_Text lblSkulls;

    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.04f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool isFlashOn = false;
    float batteryLevel;
    float speed;
    float life = 3f;

    private void Start()
    {
        lblDepth.text = $"Depth: {catacomb.catacombLevel * 500}m";
        batteryLevel = 5f;
        life = 3f;
        skullAmount = 0;
    }

    void Update()
    {
        speed = speedInput;

        if (Input.GetMouseButtonDown(1))
            isFlashOn = !isFlashOn;

        if (isFlashOn && batteryLevel > 0f)
        {
            flashLight.GetComponent<Light>().intensity = 5f;
            batteryLevel = batteryLevel > 0f ? batteryLevel - Time.deltaTime / 20 : 0f;
        }
        else
            flashLight.GetComponent<Light>().intensity = 0f;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //speed = Input.GetKeyDown(KeyCode.LeftShift) ? speedInput * 2 : speedInput;
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = speedInput * 2;
        }

        if(isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        lblBattery.text = "Battery level: " + Mathf.CeilToInt(batteryLevel).ToString();

        if(life <= 0f)
        {
            //Debug.Log("Player is dead!");
            lblBattery.text = "You died!";
            lblBattery.color = Color.red;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Battery"))
        {
            batteryLevel += 5f;
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Skull"))
        {
            skullAmount++;
            lblSkulls.text = $"Skulls\n{skullAmount}";
            Destroy(other.gameObject);
        }
    }

    public void Hit(float amount = 1f)
    {
        //this.life -= amount;
        Debug.Log("hit!");
    }

    public void restoreLife()
    {
        this.life = 3f;
    }
}
