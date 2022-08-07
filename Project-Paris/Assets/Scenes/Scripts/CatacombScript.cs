using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CatacombScript : MonoBehaviour
{
    public Canvas proxPrompt;
    public int catacombLevel;
    public PlayerController player;
    public TMP_Text lblNextlevel;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            proxPrompt.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            proxPrompt.enabled = false;
        }
    }

    private void Start()
    {
        proxPrompt.enabled = false;
    }

    private void Update()
    {
        if(proxPrompt.enabled)
        {
            if (player.skullAmount >= catacombLevel)
            {
                lblNextlevel.text = "Press 'E' to go down to the next chamber";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene($"Level_{catacombLevel + 1}");
                    player.skullAmount = 0;
                    player.restoreLife();
                }
            }
            else
            {
                lblNextlevel.text = $"You need {catacombLevel - player.skullAmount} more skulls to open the gate for the next chamber.";
            }
        }
    }
}
