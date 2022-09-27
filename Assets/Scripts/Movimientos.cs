using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movimientos : MonoBehaviour
{
    public Puntaje movimientos;
    private TextMeshProUGUI TextMesh;
    public Contador tiempo;

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
