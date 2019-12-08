using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public enum Direction
    {
        Up = -9,
        Down = 9,
        Left = -1,
        Right = 1
    }

    private Direction dir;
    private bool isChange;
    public int x_num, y_num;
    public string animator_name;

    void Update() {


        if (Input.GetKeyDown(KeyCode.UpArrow))
            dir = Direction.Up;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            dir = Direction.Down;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            dir = Direction.Left;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            dir = Direction.Right;

        switch (dir)
        {
            case Direction.Up:

                isChange = true;    
                x_num = 0;
                y_num = 1;
                animator_name = "Up";
                break;
            case Direction.Down:

                isChange = true;
                x_num = 0;
                y_num = -1;
                animator_name = "Down";
                break;
            case Direction.Left:

                isChange = true;
                x_num = -1;
                y_num = 0;
                animator_name = "Left";
                break;
            case Direction.Right:

                isChange = true;
                x_num = 1;
                y_num = 0;
                animator_name = "Right";
                break;
            default:
                break;
        }

        IsMove();
    }

    public bool IsMove()
    {
        int x, y;

        //获取Player的坐标
        Transform playerPos = GameObject.Find("Player").GetComponent<Transform>();
        x = (int)playerPos.position.x;
        y = (int)playerPos.position.y;

        GameObject g;

        if (Build.temp_map[-y * 9 + x + (int)dir] == 0 && isChange == true)
        {
            //如果下一次移动的地方是空地(数字0代表)

            //改变数组内人物的位置
            Build.temp_map[-y * 9 + x] = 0;
            Build.temp_map[-y * 9 + x + (int)dir] = 2;

            isChange = false;

            FindObjectOfType<Build>().playerDestroy = true;
            return true;
        }
        else if (Build.temp_map[-y * 9 + x + (int)dir] == 1 && isChange == true)
        {
            //如果下一次移动的地方是墙壁(数字1代表)

            isChange = false;
            return false;
        }
        else if (Build.temp_map[-y * 9 + x + (int)dir] == 3 && Build.temp_map[-y * 9 + x + (int)dir*2] == 0 || Build.temp_map[-y * 9 + x + (int)dir * 2] == 9 && isChange == true)
        {
            //如果下一次移动的地方是箱子，下下次移动的地方是空地

            //改变数组内的人物和箱子
            Build.temp_map[-y * 9 + x] = 0;
            Build.temp_map[-y * 9 + x + (int)dir] = 2;
            Build.temp_map[-y * 9 + x + (int)dir * 2] = 3;

            isChange = false;

            FindObjectOfType<Build>().playerDestroy = true;
            FindObjectOfType<Build>().boxDestroy = true;

            return true;
        }

        return false;
    }
}
