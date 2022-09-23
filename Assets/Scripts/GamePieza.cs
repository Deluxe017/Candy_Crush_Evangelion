using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePieza : MonoBehaviour
{
    public int coordenadaX; //xIndex
    public int coordenadaY; //yIndex

    public Board boardFac; //m_board

    public bool m_isMoving = false;

    public TipoDeInterpolacion tipoDeInterpolacion;
    public TipoFicha tipoFicha;

    Vector3 startPos;
    Vector3 endPos;

    [Range(0f, 1f)] public float t;

    public float tiempoMovi;
    public bool enMovimiento;

    public AnimationCurve curve;
    internal void Coordenadas(int x, int y)
    {
        coordenadaX = x;
        coordenadaY = y;
    }

    internal void Init(Board board)
    {
        boardFac = board;
    }

    internal void MoverPieza(int x, int y, float tiempoMovimiento)
    {
        StartCoroutine(MoverPiece(x,y,tiempoMovimiento));
    }

    IEnumerator MoverPiece(int desX, int destY, float timeToMover)
    {
        Vector2 startPosition = transform.position;
        bool reacedDestination = false;
        float elapsedTime = 0f;
        m_isMoving = true;

        while (!reacedDestination)
        {
            if (Vector2.Distance(transform.position, new Vector2(desX, destY)) < 0.01f)
            {
                reacedDestination = true;
                if (boardFac != null)
                {
                    boardFac.PiezaPosition(this, desX, destY);
                }
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeToMover, 0f, 1f);

            switch (tipoDeInterpolacion)
            {
                case TipoDeInterpolacion.Lineal:
                    break;

                case TipoDeInterpolacion.EaseOut:
                    t = Mathf.Cos(t = Mathf.PI * .5f);
                    break;

                case TipoDeInterpolacion.EseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * .5f);
                    break;

                case TipoDeInterpolacion.SmoothStep:
                    t = t * t * t * (3 - 2 * t);
                    break;

                case TipoDeInterpolacion.MasSuavizado:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }
            transform.position = Vector2.Lerp(startPos, new Vector2(desX, destY), t);
            yield return null;
        }
        m_isMoving = false;
    }
}

public enum TipoDeInterpolacion
{
    Lineal,
    EaseOut,
    SmootherStep,
    EseIn,
    SmoothStep,
    MasSuavizado,
}

public enum TipoFicha
{
    Mari,
    Shinji,
    Asuka,
    Eva00,
    Eva01,
    Eva02,
    Rei,
    Kaworu,
}