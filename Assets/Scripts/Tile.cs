using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int indiceX;
    public int indiceY;

    public Board m_board;
    public AudioSource Source;
    public AudioClip audioFX;
    public GameObject[] pref;
    
    public void Iniciador(int x, int y, Board board)
    {
        indiceX = x;
        indiceY = y;
        m_board = board;
    }

    public void OnMouseDown()
    {
        m_board.InicialTile(this);
    }

    public void OnMouseEnter()
    {
        m_board.FinalTile(this);
    }

    public void OnMouseUp()
    {
        m_board.RelaseTile();
        AudioSource.PlayClipAtPoint(audioFX, gameObject.transform.position);
    }
}
