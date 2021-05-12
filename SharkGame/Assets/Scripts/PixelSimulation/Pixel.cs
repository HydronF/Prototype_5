using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PixelContent {
    Empty,
    Water,
    Electricity,
}

public enum PixelDirection {
    None,
    Up,
    Down, 
    Left,
    Right
}

public class Pixel
{
    public PixelManager grid;
    public PixelContent content;
    public PixelDirection dir;
    public int row;
    public int col;
    public bool movedThisFrame;
}