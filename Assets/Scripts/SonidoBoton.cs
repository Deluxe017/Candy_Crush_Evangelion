using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoBoton : MonoBehaviour
{
    //Variables usadas en este script

    public AudioSource source;
    public AudioClip clip;


    void Start()
    {
        source.clip = clip;
    }

    //Reproduce el sonido de las fichas
    public void Reproducir()
    {
        source.Play();
    }
}
