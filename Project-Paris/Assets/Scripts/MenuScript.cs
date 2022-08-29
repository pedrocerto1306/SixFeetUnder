using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public int win;

    [SerializeField] public Button btnPlay;
    [SerializeField] public Button btnMenu;
    [SerializeField] public Camera cam;
    [SerializeField] public Vector3 camRotation;
    [SerializeField] public GameObject monster;

    private void Start()
    {
        btnPlay.onClick.AddListener(() =>
        {
            loadPlayScene("Level_1_Procedural");
        });

        if (btnMenu != null)
            btnMenu.onClick.AddListener(() => { loadPlayScene("Menu"); });

        if (win == 1)
        {
            if (monster != null)
                monster.GetComponent<Animator>().SetBool("isDancing", true);
        }
        else if (win == 2)
        {
            if (monster != null)
                monster.GetComponent<Animator>().SetBool("isEating", true);
        }
    }

    public void loadPlayScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void Update()
    {
        cam.transform.Rotate(camRotation * Time.deltaTime);
    }
}
