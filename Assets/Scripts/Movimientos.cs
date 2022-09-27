using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movimientos : MonoBehaviour
{
    //Variables que se usan en este script

    public Puntaje movimientos;
    private TextMeshProUGUI TextMesh;
    public Contador tiempo;

    //Aquí mostrará la cantidad de movimientos restantes que tendrá el jugador para mover las fichas

    public void Start()
    {
        TextMesh = GetComponent<TextMeshProUGUI>();
    }

   /* private void Update()
    {
        TextMesh.text = movimientos.movimientosNecesarios.ToString("0");

        if (movimientos.movimientosNecesarios < 1 || tiempo.restantes < 1)
        {
            movimientos.SetCondition();
        }
    }*/
}
