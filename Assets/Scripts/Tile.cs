using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Variables que se usan en este script

    public int xIndex;
    public int yIndex;

    Board m_board;

    private AudioSource musica;
    public AudioClip audioFX;


    public void Init(int cambioX, int cambioY, Board board)
    {
        xIndex = cambioX;
        yIndex = cambioY;

        m_board = board;
    }

    //Se selecciona la ficha a la cual le daremos click
    public void OnMouseDown()
    {
        m_board.ClickedTile(this);
    }

    //La ficha seleccionada se cambiará por la ficha a donde arrastremos el mouse si está al lado de la seleccionada

    public void OnMouseEnter()
    {
        m_board.DragToTile(this);
    }

    //Hace el sonido de las fichas
    public void OnMouseUp()
    {
        m_board.ReleaseTile();
        AudioSource.PlayClipAtPoint(audioFX, gameObject.transform.position);
    }

}