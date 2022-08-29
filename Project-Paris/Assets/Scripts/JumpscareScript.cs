using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpscareScript : MonoBehaviour
{
    [SerializeField]
    public GameObject monster;

    // Start is called before the first frame update
    void Start()
    {
        monster.GetComponent<Animator>().SetBool("isScaring", true);
        monster.GetComponent<AudioSource>().PlayOneShot(monster.GetComponent<AudioClip>());
    }
}
