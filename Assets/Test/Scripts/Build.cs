using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{

    private Transform gridsParent;

    public static int[] temp_map;   //传值数组
    public int[] final_map;         //用于终点检测

    public Sprite[] MapSprites;

    public GameObject pfb_Map;
    public GameObject pfb_Player;
    public GameObject pfb_Box;
    public GameObject pfb_FinalBoxPrefab;

    private GameObject g;

    public bool playerDestroy;
    public bool boxDestroy;

    public GameObject[] destroyObj;

    public GameObject pfb_Final;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        BuildMap();
    }

    private void Update()
    {
        //检查是否需要更新玩家和箱子的预制体
        if (playerDestroy)
        {
            if(boxDestroy)
            {
                BoxMove();
            }
            PlayerMove();
        }

        //检查游戏是否结束
        if(GameObject.Find("Box")==null && GameObject.Find("Final")==null)
        {
            g = Instantiate(pfb_Final) as GameObject;
            g.transform.position = new Vector3(4,-4,0);
            g.name = "Final";
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        gridsParent = GameObject.Find("GridsParent").transform;

        final_map = new int[9 * 9];

        temp_map = new int[]
        {
                1, 1, 1, 1, 1, 0, 0, 0, 0,
                1, 2, 0, 0, 1, 0, 0, 0, 0,
                1, 0, 3, 0, 1, 0, 1, 1, 1,
                1, 0, 3, 0, 1, 0, 1, 9, 1,
                1, 1, 1, 3, 1, 1, 1, 9, 1,
                0, 1, 1, 0, 0, 0, 0, 9, 1,
                0, 1, 0, 0, 0, 1, 0, 0, 1,
                0, 1, 0, 0, 0, 1, 1, 1, 1,
                0, 1, 1, 1, 1, 1, 0, 0, 0
        };

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                final_map[j * 9 + i] = temp_map[j * 9 + i];
            }
        }
    }

    /// <summary>
    /// 创建地图
    /// </summary>
    private void BuildMap()
    {
        //用于销毁GameObject
        destroyObj = new GameObject[9 * 9];

        int i = 0;
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                switch (temp_map[i])
                {
                    //0是空地，1是墙壁
                    case 1:
                    case 0:
                        g = Instantiate(pfb_Map) as GameObject;
                        g.transform.position = new Vector3(x, -y, 0);
                        g.name = x.ToString() + y;
                        g.transform.SetParent(gridsParent);

                        destroyObj[y * 9 + x] = g;

                        Sprite icon = MapSprites[temp_map[i]];
                        g.GetComponent<SpriteRenderer>().sprite = icon;
                        break;

                    //2是玩家
                    case 2:
                        g = Instantiate(pfb_Player) as GameObject;
                        g.transform.position = new Vector3(x, -y, 0);
                        g.name = "Player";

                        destroyObj[y * 9 + x] = g;
                        break;

                    //3是箱子
                    case 3:
                        g = Instantiate(pfb_Box) as GameObject; g.transform.position = new Vector3(x, -y, 0);
                        g.name = "Box";
                        g.transform.SetParent(gridsParent);

                        destroyObj[y * 9 + x] = g;
                        break;
                }

                if (i < 80)
                {
                    i++;
                }
            }
        }
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    void PlayerMove()
    {
        //定于局部变量
        int x, y;
        int x_num = 0;
        int y_num = 0;

        //得到角色位置
        Transform playerPos = GameObject.Find("Player").GetComponent<Transform>();
        x = (int)playerPos.position.x;
        y = (int)playerPos.position.y;

        //得到人物移动的x和y的移动距离
        x_num = FindObjectOfType<GameController>().x_num;
        y_num = FindObjectOfType<GameController>().y_num;

        //得到人物移动的动画名称
        string animator_name = FindObjectOfType<GameController>().animator_name;

        //销毁原来玩家的预制体
        Destroy(destroyObj[-y * 9 + x]);

        //实例新的玩家到场景中
        g = Instantiate(pfb_Player) as GameObject;
        g.GetComponent<Animator>().Play(animator_name);
        g.transform.position = new Vector3(x + x_num, y + y_num, 0);
        g.name = "Player";

        //存储新的玩家预制体
        destroyObj[-((y + y_num) * 9) + x + x_num] = g;

        //设置标志位
        playerDestroy = false;
    }

    /// <summary>
    /// 箱子移动
    /// </summary>
    private void BoxMove()
    {
        //定义临时变量
        int x, y;
        int x_num = 0;
        int y_num = 0;

        //获取玩家移动的距离
        x_num = FindObjectOfType<GameController>().x_num;
        y_num = FindObjectOfType<GameController>().y_num;

        //得到玩家的坐标
        Transform playerPos = GameObject.Find("Player").GetComponent<Transform>();

        //得到Box的位置
        x = (int)playerPos.position.x + x_num;
        y = (int)playerPos.position.y + y_num;

        //销毁原有的Box
        Destroy(destroyObj[-y*9+x]);

        //实例化新的Box
        if (final_map[-((y + y_num) * 9) + x + x_num] == 0 || final_map[-((y + y_num) * 9) + x + x_num] == 3)
        {
            g = Instantiate(pfb_Box) as GameObject;
            g.transform.position = new Vector3(x + x_num, y + y_num, 0);
            g.name = "Box";
        }
        else if (final_map[-((y + y_num) * 9) + x + x_num] == 9)
        {
            g = Instantiate(pfb_FinalBoxPrefab) as GameObject;
            g.transform.position = new Vector3(x + x_num, y + y_num, 0);
            g.name = "FinalBox";
        }

        //将新的Box传入数组
        destroyObj[-((y + y_num) * 9) + x + x_num] = g;
        playerDestroy = false;
        boxDestroy = false;
    }
}
