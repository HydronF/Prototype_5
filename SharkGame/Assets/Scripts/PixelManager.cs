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

    void FixedUpdate()
    {
        foreach(Pixel px in pixelArray) {
            PixelMovement(px);
        }

    }

    public void InitializeGrid() {
        pixelArray = new Pixel[totalRow, totalCol];
        // Initialize pixel position
        float xPos = topLeftPos.x + 0.5f * pixelSize;
        float yPos = topLeftPos.y - 0.5f * pixelSize;
        for(uint i = 0; i < totalRow; ++i) {
            for (uint j = 0; j < totalCol; ++j) {
                GameObject newPixel = Instantiate(pixelPrefab, new Vector3(xPos, yPos, 8), Quaternion.identity, gameObject.transform);
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
        Pixel pxToSwap = null;
        switch (px.force)
        {
            case ForceDirection.Up:
                if (px.row > 0) {
                    pxToSwap = pixelArray[px.row - 1, px.col];
                }
                break;
            case ForceDirection.Down:
                if (px.row < totalRow - 1) {
                    pxToSwap = pixelArray[px.row + 1, px.col];
                }
                break;
            case ForceDirection.Left:
                if (px.col > 0) {
                    pxToSwap = pixelArray[px.row, px.col - 1];
                }
                break;
            case ForceDirection.Right:
                if (px.col < totalCol - 1) {
                    pxToSwap = pixelArray[px.row, px.col + 1];
                }
                break;
            default:
                break;
        }
        
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
            int rand = Random.Range(0, 3);
            if (rand == 1 && px.col < totalCol - 1) {
                Pixel pxRight = pixelArray[px.row, px.col + 1];
                switch (pxRight.content)
                {
                    case PixelContent.Water:
                        break;
                    default:
                        SwapContent(px, pxRight);
                        break;
                } 
            }
            else if (rand == 2 && px.col > 0) {
                Pixel pxLeft = pixelArray[px.row, px.col - 1];
                switch (pxLeft.content)
                {
                    case PixelContent.Water:
                        break;
                    default:
                        SwapContent(px, pxLeft);
                        break;
                } 
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
}
