using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


//オセロを作るよ
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject boardPrefab;
    const int size = 4;
    const int width = 8;
    const int height = 8;
    BoardColor[,] boardMap = new BoardColor[width,height];
    public enum BoardColor
    {
        Black,
        White,
        Empty,
    }
    bool isBlackTurn = true;
    Color black = Color.black;
    Color white = Color.white;
    Color empty = Color.gray;
    [SerializeField] private Vector3 initPos = new Vector3(0, 0, 0);
    [SerializeField] private Text turnText;
    private int[] moveQuantity = new int[8]; //移動量
    private int[] stoneCount = new int[8]; //石の数
    private bool[] isFlip = new bool[8]; //ひっくり返せるかどうか
    Vector2[] direction = new Vector2[8]
    {new Vector2(0,1),new Vector2(1,1), //上 右上 右 右下 下 右下 左下 左 左上
    new Vector2(1,0),new Vector2(1,-1), 
    new Vector2(0,-1),new Vector2(-1,-1), 
    new Vector2(0,-1),new Vector2(1,-1)}; 

    void Start(){
        InitBoard();
        ChangeTurnText(isBlackTurn);
        //配列初期化
        for (int i = 0; i < 8; i++)
        {
            moveQuantity[i] = 0;
            stoneCount[i] = 0;
            isFlip[i] = false;
        }
    }

    void Update(){
        if (Input.GetMouseButtonDown(0))
        {
            //押したとこにレイを飛ばしてマスを検知
            //Debug.Log(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
            if (hit.collider != null)
            {
                //マスを検知したらそのマスに石を置く
                int x = (int)hit.collider.gameObject.transform.position.x;
                int y = (int)hit.collider.gameObject.transform.position.y;
                Debug.Log(x + " " + y);
                Debug.Log(IsPuttable(x, y, boardMap[x, y]));
                if (boardMap[x, y] == BoardColor.Empty && IsPuttable(x, y, boardMap[x, y]))
                {
                    //色を変える
                    if (isBlackTurn)
                    {
                        boardMap[x, y] = BoardColor.Black;
                    }
                    else
                    {
                        boardMap[x, y] = BoardColor.White;
                    }
                    //ひっくり返す
                    StonePut(x, y, boardMap[x, y]);

                    //ターンを変える
                    isBlackTurn = !isBlackTurn;
                    ChangeTurnText(isBlackTurn);
                    
                }
                
            }
            else
            {
                Debug.Log("null");
            }
            
        }
    }

    void InitBoard(){
        for (int i = 0; i < height; i++){
            for (int j = 0; j < width; j++){
                boardMap[i, j] = BoardColor.Empty;
            }
        }
        boardMap[3, 3] = BoardColor.White;
        boardMap[3, 4] = BoardColor.Black;
        boardMap[4, 3] = BoardColor.Black;
        boardMap[4, 4] = BoardColor.White;

        for (int i = 0; i < height; i++){
            for (int j = 0; j < width; j++){
                InstanceBoard(i, j, boardMap[i, j]);
            }
        }
    }

    void InstanceBoard(int x, int y, BoardColor bc){
        Color tmp = new Color(0, 0, 0);
        if (bc == BoardColor.White)
        {
            tmp = white;
        }
        else if (bc == BoardColor.Black)
        {
            tmp = black;
        }
        else if (bc == BoardColor.Empty)
        {
            tmp = empty;
        }
        var a = Instantiate(boardPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity, this.transform);
        a.GetComponent<BoardScript>().ChangeColor(bc);
        //Debug.Log(x + " " + y);
    }

    /// <summary>
    /// ボードの色を変える
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="bc"></param>
    void ChangeBoardColor(int x, int y, BoardColor bc){
        // マスのオブジェクトを見つける
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(new Vector3(x + 0.5f, y + 0.5f, 0)));
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
        GameObject obj = hit.collider.gameObject;
        
        //色を変える
        obj.GetComponent<BoardScript>().ChangeColor(bc);
    }

    void ChangeTurnText(bool isBlackTurn){
        if (isBlackTurn)
        {
            turnText.text = "黒のターン";
        }
        else
        {
            turnText.text = "白のターン";
        }
    }

    bool IsPuttable(int x, int y, BoardColor bc){
        BoardColor other = isBlackTurn ? BoardColor.White : BoardColor.Black;

        //全方向に対してひっくり返せるかどうかを調べる
        for(int dir =0;dir<8;dir++){
            //ひっくり返せるかどうかを初期化
            isFlip[dir]=false;
            //ひっくり返せる石の数を初期化
            stoneCount[dir]=0;
            //移動量を初期化
            moveQuantity[dir]=0;

            //隣が自分の色ならひっくり返せない
            int tX = x + (int)direction[dir].x;
            int tY = y + (int)direction[dir].y;
            
            //範囲外ならひっくり返せない
            if(tX<0||tX>=width||tY<0||tY>=height){
                isFlip[dir]=false;
                continue;
            }

            if(boardMap[tX,tY]==bc){
                isFlip[dir]=false;
                continue;
            }

            //隣が空白ならひっくり返せない
            if(boardMap[tX,tY]==BoardColor.Empty){
                isFlip[dir]=false;
                continue;
            }

            
            //ひっくり返せるかどうかを調べる
            for(int i=1;i<8;i++){
                moveQuantity[dir]++;
                int tmpX = x + (int)direction[dir].x * i;
                int tmpY = y + (int)direction[dir].y * i;

                //範囲外ならひっくり返せない
                if(tmpX<0||tmpX>=width||tmpY<0||tmpY>=height){
                    isFlip[dir]=false;
                    stoneCount[dir]=0;
                    break;
                }
                
                //自分の色ならひっくり返せる
                if(boardMap[tmpX,tmpY]==bc){
                    if(stoneCount[dir]!=0){
                        Debug.Log("ひっくり返せる"+dir+" "+stoneCount[dir]+" "+moveQuantity[dir]);
                        isFlip[dir]=true;
                    }
                    break;
                }

                //空白ならひっくり返せない
                if(boardMap[tmpX,tmpY]==BoardColor.Empty){
                    isFlip[dir]=false;
                    stoneCount[dir]=0;
                    break;
                }
                
                //ひっくり返せるならカウントを増やす
                if(boardMap[tmpX,tmpY]==other){
                    stoneCount[dir]++;
                }
            }

        }

        int count = 0;
        for(int i=0;i<8;i++){
            if(isFlip[i]){
                count++;
            }
        }
        Debug.Log("count"+count);
        if(count==0){
            return false;
        }
        return true;
    }


    /// <summary>
    /// 石を置く
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="bc"></param>
    void StonePut(int x, int y, BoardColor bc){
        boardMap[x, y] = bc;
        ChangeBoardColor(x, y, bc);
        
        //ひっくり返す
        for (int dir = 0; dir < 8; dir++){
            if (isFlip[dir]==true&&stoneCount[dir]!=0){
                for (int i = 1; i <= stoneCount[dir] ; i++){
                    int tmpX = x + (int)direction[dir].x * i;
                    int tmpY = y + (int)direction[dir].y * i;
                    boardMap[tmpX, tmpY] = bc;
                    Debug.Log(tmpX + " " + tmpY);
                    ChangeBoardColor(tmpX, tmpY, bc);
                }
            }
        }
        //isFilip吐かせる
        for(int i=0;i<8;i++){
            Debug.Log(i+" "+isFlip[i]+" "+stoneCount[i]);
        }

    }


}