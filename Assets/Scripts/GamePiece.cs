using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    //Estas son las variables que se usaron en este script

    public int xIndex;
    public int yIndex;

    Board m_board;

    bool m_isMoving = false;

    public TipoInterpolacion tipoDeInterpolo;
    public TipoFicha tipoFicha;
    

    //Aquí el jugador va a seleccionar la cantidad de fichas que quiere en X y Y 
    internal void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    //El board
    internal void Init(Board board)
    {
        m_board = board;
    }

    //Los tipos de movimiento elegibles que las fichas van a tener
    internal void Move(int x, int y, float moveTime)
    {
        if (!m_isMoving)
        {
            StartCoroutine(MoveRoutine(x, y, moveTime));
        }
    }

    //Revisará si las fichas están en movimiento
    IEnumerator MoveRoutine(int destX, int destY, float timeToMove)
    {
        Vector2 startPosition = transform.position;
        bool reacedDestination = false;
        float elapsedTime = 0f;
        m_isMoving = true;

        while (!reacedDestination)
        {
            if (Vector2.Distance(transform.position, new Vector2(destX, destY)) < 0.01f)
            {
                reacedDestination = true;
                if (m_board != null)
                {
                    m_board.placeGamePiece(this, destX, destY);
                }
                break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            switch (tipoDeInterpolo)
            {
                case TipoInterpolacion.Lineal:

                    break;

                case TipoInterpolacion.Entrada:

                    t = 1 - Mathf.Cos(t * Mathf.PI * .5f);

                    break;

                case TipoInterpolacion.Salida:

                    t = Mathf.Sin(t + Mathf.PI * .5f);

                    break;

                case TipoInterpolacion.Suavizado:

                    t = t * t * (3 - 2 * t);

                    break;

                case TipoInterpolacion.MasSuavizado:

                    t = t * t * t * (t * (t * 6 - 15) + 10);

                    break;
            }

            transform.position = Vector2.Lerp(startPosition, new Vector2(destX, destY), t);

            yield return null;
        }

        m_isMoving = false;
    }

    //Tipos de movimiento elegibles para el movimiento de las fichas
    public enum TipoInterpolacion
    {
        Lineal,
        Entrada,
        Salida,
        Suavizado,
        MasSuavizado,
    }

    //Los nombres de los diferentes tipos de fichas que hay en el juego
    public enum TipoFicha
    {
        Asuka,
        Rei,
        Shinji,
        Kaworu,
        Mari,
        Eva00,
        Eva01,
        Eva02,
    }
}