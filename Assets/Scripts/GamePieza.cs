using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePieza : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    public int coordenadaX;
    public int coordenadaY;
    [Range(0f, 1f)] public float t;

    public float tiempoMovi;
    public bool enMovimiento;

    public TipoDeInterpolacion tipoDeInterpolacion;

    public AnimationCurve curve;

    public Board boar_D;

    public TipoFicha tipoFicha;


    private void Update()
    {
       /* if( Mathf.Abs(coordenadaX -transform.position.x) > .1f)
        {
           temPosition = new Vector3(coordenadaX, transform.position.y);
            transform.position = Vector3.Lerp(transform.position, temPosition, .4f);
        }
        else
        {
            temPosition = new Vector3(coordenadaX, transform.position.y);
            transform.position = temPosition;
            boar_D. [coordenadaX, coordenadaY] = this.gameObject;*/

            
       /* if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovePieza(new Vector3((int)transform.position.x, (int)transform.position.y + 1, 0), tiempoMovi);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            MovePieza(new Vector3((int)transform.position.x, (int)transform.position.y - 1, 0), tiempoMovi);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            MovePieza(new Vector3((int)transform.position.x -1, (int)transform.position.y, 0), tiempoMovi);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            MovePieza(new Vector3((int)transform.position.x +1, (int)transform.position.y, 0), tiempoMovi);
        }*/



    }
    public void PiezaIniciada(int x, int y)
    {
        coordenadaX = x;
        coordenadaY = y;
    }

    public void MovePieza(int x, int y, float duracion)
    {
        if(enMovimiento)
        {
            StartCoroutine(MovePlece(new Vector3(x, y, 0), duracion));
        }   
    }

    IEnumerator MovePlece(Vector3 posicionFinal, float tiempoMovimiento)
    {
        enMovimiento = false;

        bool llegoAlPunto = false;
        float tiempoTranscurrido = 0f;
        Vector3 startPos = transform.position;
  
        while(!llegoAlPunto)
        {
            if(Vector3.Distance(transform.position, posicionFinal) < 0.01f)
            {
                llegoAlPunto = true;
                enMovimiento = true;

                boar_D.PiezaPosition(this, (int)posicionFinal.x, (int)posicionFinal.y);

                transform.position = new Vector3((int)posicionFinal.x, (int)posicionFinal.y, 0);
                
                break;
            }

            float t = tiempoTranscurrido / tiempoMovimiento;

            switch (tipoDeInterpolacion)
            {
                case TipoDeInterpolacion.Lineal:
                    //Movimiento Lineal
                    t = curve.Evaluate(t);
                    break;

                case TipoDeInterpolacion.Salida:
                    //Movimiento Salida
                    t = Mathf.Sin(t * Mathf.PI * .5f);

                    break;

                case TipoDeInterpolacion.Entrada:
                    //Movimiento Entrada
                    t = 1 - Mathf.Cos(t * Mathf.PI * .5f);

                    break;

                case TipoDeInterpolacion.Suavizado:
                    //Movimiento Suavizado
                    t = t * t * (3 - 2 * t);

                    break;

                case TipoDeInterpolacion.MasSuavizado:
                    //Movimiento MasSuavizado
                    t = t * t * t*(t * (6 - 15) + 10);

                    break;
               
            }
          
            transform.position = Vector3.Lerp(startPos, posicionFinal, t);
            tiempoTranscurrido += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public enum TipoDeInterpolacion
    {
        Lineal,
        Entrada,
        Salida,
        Suavizado,
        MasSuavizado
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









}
