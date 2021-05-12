using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Collections;
using Unity.Jobs;

public class PixelManager : MonoBehaviour
{
    [Header("Dimensions")]
    public uint totalRow;
    public uint totalCol;
    public float pixelSize;
    public Pixel[,] pixelArray;
    
    [Header("Physics")]
    public float simTimeStep;
    public float electricityDuration;
    float electricityTimer;
    bool electricityPresent = false;
    public int maxPixelsRenderedPerFrame;
    List<Transform> sharknadoTransforms;
    bool[,] elecTravelled;
    Queue<Pixel> renderQueue;

    [Header("Tilemap")]
    public Tilemap waterMap;
    public Tilemap electricityMap;
    public Vector3Int topLeft;
    public TileBase emptyTile;
    public TileBase waterTile;
    public Tilemap foreground;
    public TileBase waterForeground;
    public TileBase electricityTile;

    [Header("References")]
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        sharknadoTransforms = new List<Transform>();
        renderQueue = new Queue<Pixel>();
        InitializeGrid();
        StartCoroutine(PhysicsSim());
        RenderGrid();
        elecTravelled = new bool[totalRow, totalCol];
        for (int i = 0; i < totalRow; ++i) {
            for (int j = 0; j < totalCol; ++j) {
                elecTravelled[i, j] = false;
            }
        }
    }


    void Update() {
        if (electricityPresent) {
            electricityTimer -= Time.deltaTime;
        }
    }


    public void InitializeGrid() {
        pixelArray = new Pixel[totalRow, totalCol];
        // Initialize pixel position
        for(int i = 0; i < totalRow; ++i) {
            for (int j = 0; j < totalCol; ++j) {
                pixelArray[i, j] = new Pixel();
                pixelArray[i, j].grid = this;
                pixelArray[i, j].row = i;
                pixelArray[i, j].col = j;
                pixelArray[i, j].movedThisFrame = false;
                if (i > totalRow * 11 / 20) {
                    pixelArray[i, j].content = PixelContent.Water;
                }
                else if (i == totalRow / 2 && j % 2 == 0) {
                    pixelArray[i, j].content = PixelContent.Water;
                }
                else {
                    pixelArray[i, j].content = PixelContent.Empty;
                }
                
                pixelArray[i, j].dir = PixelDirection.Down;
            }
        }
    }

    void PixelMovement(Pixel px) {
         switch (px.content)
        {
            case PixelContent.Water:
                WaterMovement(px);
                break;
            case PixelContent.Electricity:
                ElectricityMovement(px);
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
                case PixelContent.Electricity:
                    break;         
                default:
                    SwapContent(px, pxToSwap);
                    break;
            }
        }

        if (!px.movedThisFrame) {
            List<Pixel> emptyList = CheckEmpty(px);
            int rand = Random.Range(0, emptyList.Count + 1);
            if (rand != emptyList.Count) {
                SwapContent(px, emptyList[rand]);
            }
        }
    }

    void ElectricityMovement(Pixel px)
    {
        // We advance electricity 10 times each frame
        for (int i = 0; i < 10; ++i) {
            ElectricityPass(px);
        }
    }

    void ElectricityPass(Pixel px) {
        // We advance all electricity by 1 tile
        if (px.row <= 0 || px.col <= 0 || px.row >= totalRow - 1 || px.col >= totalCol - 1
            || elecTravelled[px.row, px.col]) {
            return;
        }
        List<Pixel> waterList = new List<Pixel>();
        List<Pixel> airList = new List<Pixel>();        
        switch (px.dir)
        {
            case PixelDirection.Up:
                CheckElectricityNext(pixelArray[px.row - 1, px.col], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row, px.col - 1], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row, px.col + 1], ref waterList, ref airList); 
                break;
            case PixelDirection.Down:
                CheckElectricityNext(pixelArray[px.row + 1, px.col], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row, px.col - 1], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row, px.col + 1], ref waterList, ref airList); 
                break;
            case PixelDirection.Left:
                CheckElectricityNext(pixelArray[px.row - 1, px.col], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row + 1, px.col], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row, px.col - 1], ref waterList, ref airList); 
                break;
            case PixelDirection.Right:
                CheckElectricityNext(pixelArray[px.row - 1, px.col], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row + 1, px.col], ref waterList, ref airList); 
                CheckElectricityNext(pixelArray[px.row, px.col + 1], ref waterList, ref airList); 
                break;
            default:
                break;
        }

        if (waterList.Count > 0) {
            Pixel nextPx = waterList[Random.Range(0, waterList.Count)];
            SetElectricity(nextPx, px.dir);
            elecTravelled[px.row, px.col] = true;
        }
        else if (airList.Count > 0 && Random.Range(0.0f, 1.0f) < 0.06f) {
            Pixel nextPx = airList[Random.Range(0, airList.Count)];
            SetElectricity(nextPx, px.dir);
            elecTravelled[px.row, px.col] = true;

        }
    }

    void CheckElectricityNext(Pixel pixel, ref List<Pixel> waterList, ref List<Pixel> airList) {
        if (pixel.content == PixelContent.Water) {
                waterList.Add(pixel);
        }
        else if (pixel.content == PixelContent.Empty) {
                airList.Add(pixel);
        }
    }

    void SwapContent(Pixel px1, Pixel px2) {
        PixelContent tempContent = px1.content;
        px1.content = px2.content;
        px2.content = tempContent;
        renderQueue.Enqueue(px1);
        renderQueue.Enqueue(px2);
        px1.movedThisFrame = true;
        px2.movedThisFrame = true;
    }

    #region Display
    public void RenderGrid() {
        foreach (Pixel px in pixelArray)
        {
            renderQueue.Enqueue(px);
        }
    }

    public void RenderPixel(Pixel px) {
        Vector3Int pos = new Vector3Int(topLeft.x + px.col, topLeft.y - px.row, topLeft.z);
        switch (px.content)
        {
            case PixelContent.Empty:
                waterMap.SetTile(pos, emptyTile);
                foreground.SetTile(pos, emptyTile);
                electricityMap.SetTile(pos, null);
                break;      
            case PixelContent.Water:
                waterMap.SetTile(pos, waterTile);
                foreground.SetTile(pos, waterForeground);
                electricityMap.SetTile(pos, null);
                break;
            case PixelContent.Electricity:
                electricityMap.SetTile(pos, electricityTile);
                break;
        }
    }
    #endregion


    IEnumerator PhysicsSim() {
        while (true) {
            if (electricityPresent && electricityTimer < 0.0f) {
                    ClearElectricity();
            }
            HandleTornados();
            UpdatePixelPhysics();
            for (int i = 0; i < maxPixelsRenderedPerFrame; ++i) {
                if (renderQueue.Count > 0) {
                    RenderPixel(renderQueue.Dequeue());
                }
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

    void UpdatePixelPhysics() {
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
        foreach (Pixel px in pixelArray) {
            px.movedThisFrame = false;
        }
    }
    public Vector2Int GetPixelAtPos(Vector3 worldPos) {
        int x = topLeft.y - waterMap.WorldToCell(worldPos).y;
        int y = waterMap.WorldToCell(worldPos).x - topLeft.x;
        return new Vector2Int(x, y);
    }

    public PixelContent GetContentWorld(Vector3 worldPos) {
        if (electricityMap.GetTile(waterMap.WorldToCell(worldPos)) == electricityTile) {
            return PixelContent.Electricity;
        }
        else if (waterMap.GetTile(waterMap.WorldToCell(worldPos)) == waterTile) {
            return PixelContent.Water;
        }
        else {
            return PixelContent.Empty;
        }
    }

    public void StartTornado(Transform sharknado) {
        if (sharknadoTransforms.Count == 0) { audioManager.StartStorm(); }
        sharknadoTransforms.Add(sharknado);
    }

    public void StopTornado(Transform sharknado) {
        sharknadoTransforms.Remove(sharknado);
        if (sharknadoTransforms.Count == 0 && audioManager != null) { audioManager.EndStorm(); }
    }


    void HandleTornados() {
        foreach (Pixel px in pixelArray) {
            if (px.content == PixelContent.Water) {
                px.dir = PixelDirection.Down;
            }
        }
        foreach (Transform sharknado in sharknadoTransforms) {
            TornadoForceField(sharknado);
        }
    }

    void TornadoForceField(Transform sharknado) {
        // Figure out which column is the shark in
        int sharkCol = GetPixelAtPos(sharknado.position).y;
        int innerRange = 6;
        int outerRange = 15;
        int inOutCutoff = 60;
        int bottomRow = (int) totalRow ;

        // Middle: Force = Up
        for(int j = sharkCol - innerRange; j <= sharkCol + innerRange; ++j) {
            if (j >= 0 && j < totalCol) {
                for (int i = 0; i < bottomRow; ++i) {
                    if (pixelArray[i, j].content != PixelContent.Electricity) {
                        pixelArray[i, j].dir = PixelDirection.Up;
                    }
                }
            }
        }

        // Left Low: Force = Right
        for(int j = sharkCol - outerRange; j > sharkCol - innerRange; ++j) {
            if (j >= 0 && j < totalCol) {
                for (int i = inOutCutoff; i < bottomRow; ++i) {
                    if (pixelArray[i, j].content != PixelContent.Electricity) {
                        pixelArray[i, j].dir = PixelDirection.Right;
                    }
                }
            }
        }

        // Right Low: Force = Left
        for(int j = sharkCol + innerRange; j < sharkCol + outerRange; ++j) {
            if (j >= 0 && j < totalCol) {
                for (int i = inOutCutoff; i < bottomRow; ++i) {
                    if (pixelArray[i, j].content != PixelContent.Electricity) {
                        pixelArray[i, j].dir = PixelDirection.Left;
                    }
                }
            }
        }

        // Left High: Force = Left
        for(int j = sharkCol - outerRange; j > sharkCol - innerRange; ++j) {
            if (j >= 0 && j < totalCol) {
                for (int i = 0; i <= inOutCutoff; ++i) {
                    if (pixelArray[i, j].content != PixelContent.Electricity) {
                        pixelArray[i, j].dir = PixelDirection.Left;
                    }
                }
            }
        }

        // Right High: Force = Right
        for(int j = sharkCol + innerRange + 1; j < sharkCol + outerRange; ++j) {
            if (j >= 0 && j < totalCol) {
                for (int i = 0; i <= inOutCutoff; ++i) {
                    if (pixelArray[i, j].content != PixelContent.Electricity) {
                        pixelArray[i, j].dir = PixelDirection.Right;
                    }
                }
            }
        }
    }

    public void ActivateElectricity(Vector3 worldPos) {
        audioManager.StartElectricity();
        electricityTimer = electricityDuration;
        electricityPresent = true;
        Vector2Int center = GetPixelAtPos(worldPos);
        SetElectricity(center.x - 3, center.y, PixelDirection.Up);
        SetElectricity(center.x + 3, center.y, PixelDirection.Down);
        SetElectricity(center.x, center.y - 3, PixelDirection.Left);
        SetElectricity(center.x, center.y + 3, PixelDirection.Right);
        SetElectricity(center.x - 2, center.y + 2, PixelDirection.Up);
        SetElectricity(center.x + 2, center.y - 2, PixelDirection.Down);
        SetElectricity(center.x - 2, center.y - 2, PixelDirection.Left);
        SetElectricity(center.x + 2, center.y + 2, PixelDirection.Right);

    }

    void SetElectricity(int i, int j, PixelDirection dir) {
        if (i > 0 && i < totalRow && j > 0 && j < totalCol) {
            pixelArray[i, j].content = PixelContent.Electricity;
            pixelArray[i, j].dir = dir;
            renderQueue.Enqueue(pixelArray[i, j]);
        }
    }

    void SetElectricity(Pixel px, PixelDirection dir) {
            px.content = PixelContent.Electricity;
            px.dir = dir;
            renderQueue.Enqueue(px);
    }

    Vector3Int PixelToTile(Pixel px) {
        return new Vector3Int(
            topLeft.x + px.col,
            topLeft.y - px.row,
            0
        );
    }

    public void ClearElectricity() {
        audioManager.EndElectricity();
        foreach (Pixel px in pixelArray) {
            if (px.content == PixelContent.Electricity) {
                if (foreground.GetTile(PixelToTile(px)) == waterForeground) {
                    px.content = PixelContent.Water;
                    px.dir = PixelDirection.Down;
                    renderQueue.Enqueue(px);
                }
                else {
                    px.content = PixelContent.Empty;
                    px.dir = PixelDirection.Down;
                    renderQueue.Enqueue(px);
                }
            }
        }
        electricityPresent = false;
        for (int i = 0; i < totalRow; ++i) {
            for (int j = 0; j < totalCol; ++j) {
                elecTravelled[i, j] = false;
            }
        }
    }
}

