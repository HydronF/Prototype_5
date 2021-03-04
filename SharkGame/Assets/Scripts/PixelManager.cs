using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelManager : MonoBehaviour
{
    [Header("Dimensions")]
    public uint totalRow;
    public uint totalCol;
    public float pixelSize;

    public Vector2 topLeftPos;

    public Pixel[,] pixelArray;
    
    [Header("Prefab")]
    public GameObject pixelPrefab;

    [Header("Colors")]
    public Color32 waterColor;
    public Color32 emptyColor;

    [Header("Physics")]
    public float simTimeStep;

 


    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
//        StartCoroutine(PhysicsSim());
        RenderGrid();
    }


    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            for(uint i = 0; i < totalRow; ++i) {
                for (uint j = 0; j < totalCol; ++j) {
                    if (j > 122 && j < 128 && i + j < 200 && i + j > 145) {
                        pixelArray[i, j].dir = PixelDirection.Up;
                    }
                    else if (j > 115 && j < 135 && i > 15)  {
                        pixelArray[i, j].dir = PixelDirection.None;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        int rand = Random.Range(0, 4);
        if (rand / 2 == 0) {
            for (uint i = 0; i < totalRow; i++) {
                if (rand % 2 == 0) {
                    for (uint j = 0; j < totalCol; j++) {
                        PixelMovement(pixelArray[i, j]);
                    }
                }
                else {
                    for (uint j = totalCol - 1; j > 0; j--) {
                        PixelMovement(pixelArray[i, j]);
                    }
                }
            }
        }
        else {
            for (uint i = totalRow - 1; i > 0; i--) {
                if (rand % 2 == 0) {
                    for (uint j = 0; j < totalCol; j++) {
                        PixelMovement(pixelArray[i, j]);
                    }
                }
                else {
                    for (uint j = totalCol - 1; j > 0; j--) {
                        PixelMovement(pixelArray[i, j]);
                    }
                }
            }
        }
    }

    public void InitializeGrid() {
        pixelArray = new Pixel[totalRow, totalCol];
        // Initialize pixel position
        float xPos = topLeftPos.x + 0.5f * pixelSize;
        float yPos = topLeftPos.y - 0.5f * pixelSize;
        for(uint i = 0; i < totalRow; ++i) {
            for (uint j = 0; j < totalCol; ++j) {
                GameObject newPixel = Instantiate(pixelPrefab, new Vector3(xPos, yPos, transform.position.z), Quaternion.identity, gameObject.transform);
                pixelArray[i, j] = newPixel.GetComponent<Pixel>();
                xPos += pixelSize;
                pixelArray[i, j].grid = this;
                pixelArray[i, j].row = i;
                pixelArray[i, j].col = j;
                if (j % 2 == 0) {
                    pixelArray[i, j].content = PixelContent.Water;
                }
                else {
                    pixelArray[i, j].content = PixelContent.Empty;

                }
                
                pixelArray[i, j].dir = PixelDirection.Down;
            }
            xPos = topLeftPos.x + 0.5f * pixelSize;
            yPos -= pixelSize;
        }
    }

    void PixelMovement(Pixel px) {
         switch (px.content)
        {
            case PixelContent.Water:
                WaterMovement(px);
                break;
            case PixelContent.Empty:
                break;
        }
    }

    void WaterMovement(Pixel px) {
        // Force
        Pixel pxToSwap = GetPixelToSwap(px, px.dir);
        if (pxToSwap != null) {
            switch (pxToSwap.content)
            {
                case PixelContent.Water:
                    break;                
                default:
                    SwapContent(px, pxToSwap);
                    break;
            }
        }

        if (!px.GetMovedThisFrame()) {

            // Side movement
            // int rand = Random.Range(0, 5);
            // pxToSwap = GetPixelToSwap(px, (PixelDirection) rand);
            // if (pxToSwap != null) {
            //     switch (pxToSwap.content)
            //     {
            //         case PixelContent.Water:
            //             break;
            //         default:
            //             SwapContent(px, pxToSwap);
            //             break;
            //     } 
            // }
            List<Pixel> emptyList = CheckEmpty(px);
            int rand = Random.Range(0, emptyList.Count + 1);
            if (rand != emptyList.Count) {
                SwapContent(px, emptyList[rand]);
            }
        }
    }

    void SwapContent(Pixel px1, Pixel px2) {
        PixelContent tempContent = px1.content;
        px1.content = px2.content;
        px2.content = tempContent;
        RenderPixel(px1);
        RenderPixel(px2);
        px1.SetMovedThisFrame(true);
        px2.SetMovedThisFrame(true);
    }

    #region Display
    public void RenderGrid() {
        foreach (Pixel px in pixelArray)
        {
            RenderPixel(px);
        }
    }

    public void RenderPixel(Pixel px) {
        switch (px.content)
        {
            case PixelContent.Water:
                px.gameObject.GetComponent<SpriteRenderer>().color = waterColor;
                break;
            case PixelContent.Empty:
                px.gameObject.GetComponent<SpriteRenderer>().color = emptyColor;
                break;        
        }
    }
    #endregion


    IEnumerator PhysicsSim() {
        while (true) {
            foreach(Pixel px in pixelArray) {
                PixelMovement(px);
            }
            yield return new WaitForSeconds(simTimeStep);
        }
    }

    Pixel GetPixelToSwap(Pixel px, PixelDirection dir) {
        switch (dir)
        {
            case PixelDirection.Up:
                if (px.row > 0) {
                    return(pixelArray[px.row - 1, px.col]);
                }
                break;
            case PixelDirection.Down:
                if (px.row < totalRow - 1) {
                    return(pixelArray[px.row + 1, px.col]);
                }
                break;
            case PixelDirection.Left:
                if (px.col > 0) {
                    return(pixelArray[px.row, px.col - 1]);
                }
                break;
            case PixelDirection.Right:
                if (px.col < totalCol - 1) {
                    return(pixelArray[px.row, px.col + 1]);
                }
                break;
            default:
                break;
        }
        return null;
    }

    List<Pixel> CheckEmpty(Pixel px) {
        List<Pixel> returnList = new List<Pixel>();
        if (px.row > 0 && pixelArray[px.row - 1, px.col].content == PixelContent.Empty) {
            returnList.Add(pixelArray[px.row - 1, px.col]);
        }
        if (px.row < totalRow - 1 && pixelArray[px.row + 1, px.col].content == PixelContent.Empty) {
            returnList.Add(pixelArray[px.row + 1, px.col]);
        }
        if (px.col > 0 && pixelArray[px.row, px.col - 1].content == PixelContent.Empty) {
            returnList.Add(pixelArray[px.row, px.col - 1]);
        }
        if (px.col < totalCol - 1 && pixelArray[px.row, px.col + 1].content == PixelContent.Empty) {
            returnList.Add(pixelArray[px.row, px.col + 1]);
        }
        return returnList;
    }

}
