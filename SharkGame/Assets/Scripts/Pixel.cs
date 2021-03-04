using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PixelContent {
    Empty,
    Water,
}

public enum PixelDirection {
    None,
    Up,
    Down, 
    Left,
    Right
}

public class Pixel : MonoBehaviour
{
    public PixelManager grid;
    public PixelContent content;
    public PixelDirection dir = PixelDirection.Down;
    public uint row;
    public uint col;
    public bool movedThisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate() {
        movedThisFrame = false;
    }

    #region Getters and Setters
    public bool GetMovedThisFrame() {return movedThisFrame;}
    public void SetMovedThisFrame(bool val) {movedThisFrame = val;}
    #endregion}
}