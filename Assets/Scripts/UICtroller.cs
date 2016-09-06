using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICtroller : MonoBehaviour {
    public GameObject pause;
    public GameObject gameover;
    public GameObject time;
    public GameObject next;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Application.loadedLevel != 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if ((gameover.active == false))
                {
                    if (pause.active)
                    {
                        pause.SetActive(false);
                        Time.timeScale = 1.0f;
                    }
                    else
                    {
                        pause.SetActive(true);
                        Time.timeScale = 0;
                    }

                }
            }
            time.GetComponent<Text>().text = "Time: " + ((int)GameObject.Find("GamePlay").GetComponent<GamePlay>().getGameTime()).ToString();
        }
	}
    public void start() {
        Application.LoadLevel(1);

    }
    public void quit() {
        Application.Quit();
        
    }
    public void restart() {
        int i = Application.loadedLevel;
        Time.timeScale = 1.0f;
        Application.LoadLevel(i);
    }
    public void gameisover() {
        gameover.SetActive(true);
        Time.timeScale = 0;
    }
    public void nextlv() {
        
        int j = Application.loadedLevel;
        print(j);
        j++;
        print(j);
        Time.timeScale = 1.0f;
        Application.LoadLevel(j);
        print(j);
    }
    public void win() {
        next.SetActive(true);
    }
}
