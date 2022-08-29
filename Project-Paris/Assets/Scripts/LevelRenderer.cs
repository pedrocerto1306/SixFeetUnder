using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using TMPro;

public class LevelRenderer : MonoBehaviour
{
    //UI Elements
    [Header("Elementos de IU")]
    public TMP_Text lblBattery;
    public TMP_Text lblDepth;
    public TMP_Text lblSkulls;
    public TMP_Text lblNextLevel;
    public Canvas prxPrompt;

    [Header("Especificacoes do Nivel")]
    [SerializeField]
    [Range(1, 50)]
    public int width = 10;

    [SerializeField]
    [Range(1, 50)]
    public int height = 10;

    [SerializeField]
    public float tamParede = 1f;

    [SerializeField]
    [Header("Paredes")]
    public float sizeMultiplier = 1f;

    [SerializeField]
    private Transform prefabParede;
    [SerializeField]
    private Transform prefabChao;

    [Header("Level config")]
    [SerializeField]
    private int level;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject skullPrefab;
    [SerializeField]
    private GameObject batteryPrefab;
    [SerializeField]
    private GameObject monsterPrefab;
    [SerializeField]
    private GameObject passagemPrefab;
    [SerializeField]
    public List<NavMeshSurface> surfaces;

    // Start is called before the first frame update
    void Start()
    {
        var labirinto = LevelGenerator.Gerar(width, height);
        Render(labirinto);
    }

    private void Render(EstadoParede[,] lab)
    {
        System.Random rd = new System.Random();

        //Cria teto e ch�o
        var chao = Instantiate(prefabChao, transform);
        chao.localScale = new Vector3(width + tamParede, 1, height + tamParede);
        chao.position = new Vector3(width * 3.6f, -tamParede / 2.5f, height * 3.6f);
        // surfaces.Add(chao.GetComponent<NavMeshSurface>());

        var teto = Instantiate(prefabChao, transform);
        teto.localScale = new Vector3(width + tamParede, 1, height + tamParede);
        teto.position = new Vector3(width * 3.6f, tamParede * 0.5f, height * 3.6f);
        teto.Rotate(new Vector3(180, 0, 0));
        // surfaces.Add(teto.GetComponent<NavMeshSurface>());

        var player = Instantiate(playerPrefab, transform);
        int playerX = rd.Next(0, width), playerY = rd.Next(0, height);
        player.transform.position = new Vector3(-width / 2 + playerX, 2, -height / 2 + playerY);
        PlayerController plCtrl = player.GetComponent<PlayerController>();
        plCtrl.lblBattery = lblBattery;
        plCtrl.lblDepth = lblDepth;
        lblDepth.text = $"Depth: {level * 500}m";
        plCtrl.lblSkulls = lblSkulls;
        plCtrl.batteryLevel = 5f;

        //Spawna os itens do jogo (bateria, passagem, caveiras, etc.)
        int[] itemPos = { rd.Next(0, width * Mathf.FloorToInt(tamParede)), rd.Next(0, height * Mathf.FloorToInt(tamParede)) };
        var passagem = Instantiate(passagemPrefab, transform);
        passagem.transform.position = new Vector3(-width / 2 + itemPos[0], -tamParede / 3, -height / 2 + itemPos[1]);
        CatacombScript ctrlCat = passagem.GetComponentInChildren<CatacombScript>();
        ctrlCat.proxPrompt = prxPrompt;
        ctrlCat.catacombLevel = level;
        ctrlCat.player = plCtrl;
        ctrlCat.lblNextlevel = lblNextLevel;
        // surfaces.Add(passagem.GetComponent<NavMeshSurface>());


        var monster = Instantiate(monsterPrefab, transform);
        itemPos[0] = rd.Next(0, width * Mathf.FloorToInt(tamParede)); itemPos[1] = rd.Next(0, height * Mathf.FloorToInt(tamParede));
        monster.transform.position = new Vector3(-width / 2 + (itemPos[0] - tamParede / 2), -4.2f, -height / 2 + (itemPos[1] - tamParede / 2));
        monster.transform.localScale = Vector3.one * 2;

        BixaoController tortoCtrl = monster.GetComponent<BixaoController>();
        tortoCtrl.player = player;
        tortoCtrl.speed = 500f;
        tortoCtrl.lvlRenderer = this;

        for (int i = 0; i < level; i++)
        {
            var skull = Instantiate(skullPrefab, transform);
            itemPos[0] = rd.Next(0, width * Mathf.FloorToInt(tamParede)); itemPos[1] = rd.Next(0, height * Mathf.FloorToInt(tamParede));
            skull.transform.position = new Vector3(-width / 2 + (itemPos[0] - tamParede / 2), 0, -height / 2 + (itemPos[1] - tamParede / 2));
            // surfaces.Add(skull);
            itemPos[0] = rd.Next(0, width * Mathf.FloorToInt(tamParede)); itemPos[1] = rd.Next(0, height * Mathf.FloorToInt(tamParede));
            var battery = Instantiate(batteryPrefab, transform);
            battery.transform.position = new Vector3(-width / 2 + (itemPos[0] - tamParede / 2), -tamParede / 4, -height / 2 + (itemPos[1] - tamParede / 2));
            // surfaces.Add(battery);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cell = lab[i, j];
                var posicao = new Vector3(-width / 2 + i * tamParede, 0, -height / 2 + j * tamParede); //Centro do labirinto no meio (0,0)

                if (cell.HasFlag(EstadoParede.UP))
                {
                    var topWall = Instantiate(prefabParede.transform, transform) as Transform;
                    topWall.position = posicao + new Vector3(0, 0, tamParede / 2);
                    topWall.localScale = new Vector3(tamParede, topWall.localScale.y * tamParede, topWall.localScale.z);

                    surfaces.Add(topWall.GetComponent<NavMeshSurface>());
                }

                if (cell.HasFlag(EstadoParede.LEFT))
                {
                    var leftWall = Instantiate(prefabParede.transform, transform) as Transform;
                    leftWall.position = posicao + new Vector3(-tamParede / 2, 0, 0);
                    leftWall.localScale = new Vector3(tamParede, leftWall.localScale.y * tamParede, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);

                    surfaces.Add(leftWall.GetComponent<NavMeshSurface>());
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(EstadoParede.RIGHT))
                    {
                        var rightWall = Instantiate(prefabParede.transform, transform) as Transform;
                        rightWall.position = posicao + new Vector3(+tamParede / 2, 0, 0);
                        rightWall.localScale = new Vector3(tamParede, rightWall.localScale.y * tamParede, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);

                        surfaces.Add(rightWall.GetComponent<NavMeshSurface>());
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(EstadoParede.DOWN))
                    {
                        var bottomWall = Instantiate(prefabParede.transform, transform) as Transform;
                        bottomWall.position = posicao + new Vector3(0, 0, -tamParede / 2);
                        bottomWall.localScale = new Vector3(tamParede, bottomWall.localScale.y * tamParede, bottomWall.localScale.z);

                        surfaces.Add(bottomWall.GetComponent<NavMeshSurface>());
                    }
                }
            }
        }

        //Bake navmesh
        // for (int i = 0; i < surfaces.Count; i++)
        // {
        //     surfaces[i].BuildNavMesh();
        // }

        surfaces[0].BuildNavMesh();
    }

    // private void Update()
    // {
    //     surfaces[0].BuildNavMesh();
    // }
}
