using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    private Color black = Color.black;
    private Color white = Color.white;
    private Color empty = Color.gray;
    [SerializeField]private GameManager.BoardColor thisBoard = GameManager.BoardColor.Empty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeColor(GameManager.BoardColor bc){
        if(bc == GameManager.BoardColor.Black){
            thisBoard = GameManager.BoardColor.Black;
            GetComponent<SpriteRenderer>().color = black;
        }
        else if(bc == GameManager.BoardColor.White){
            thisBoard = GameManager.BoardColor.White;
            GetComponent<SpriteRenderer>().color = white;
        }
        else if(bc == GameManager.BoardColor.Empty){
            thisBoard = GameManager.BoardColor.Empty;
            GetComponent<SpriteRenderer>().color = empty;
        }
    }
}
