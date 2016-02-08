using UnityEngine;
using System.Collections;

public class Labyrinth : MonoBehaviour 
{
    //Состояние лабиринта описываем следующим образом:
    //нулевой, 17-ый, +16х - элементы описывают тип куба:
    //private string[] objTags = new string[] {
    //    "Empty", //0
    //    "EnemyRespawn", //1
    //    "PlayerRespawn", //2
    //    "Bricks", //3
    //    "Concrete", //4
    //    "Water", //5
    //    "Forest", //6
    //    "Ice", //7
    //    "Fortress" //8
    //};
    //Всего их 13х13=169. В каждом объекте могут быть кирпичики, которых 16 штук. 16*169=2704.
    //Строка будет длиной 2704+169=2873.

    public string state = "";
    private string oldState = "";

	// Use this for initialization
	void Start () 
    {
        for (int i = 0; i < 2873; i++)
        {
            state += "0";
        }
        oldState = state;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (state != oldState)
        {
            OnChangeState();
        }
	}

    public void OnChangeState()
    {
        oldState = state;


    }
}
