using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] public Button btnPlay;
    [SerializeField] public Camera cam;
    [SerializeField] public Vector3 camRotation;

    private void Start()
    {
        btnPlay.onClick.AddListener(() =>
        {
            loadPlayScene("Level_1");
        });
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
