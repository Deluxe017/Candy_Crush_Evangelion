using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int indiceX;
    public int indiceY;

    public Board board;
    public AudioSource Source;
    public AudioClip audioFX;
    public GameObject[] pref;
    
    public void Iniciador(int x, int y)
    {
        indiceX = x;
        indiceY = y;

    }

    public void OnMouseDown()
    {
        board.InicialTile(this);
    }

    public void OnMouseEnter()
    {
        board.FinalTile(this);
    }

    public void OnMouseUp()
    {
        board.RelaseTile();
        AudioSource.PlayClipAtPoint(audioFX, gameObject.transform.position);
    }
}
