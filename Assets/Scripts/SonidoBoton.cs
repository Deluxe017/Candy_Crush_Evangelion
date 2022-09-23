using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoBoton : MonoBehaviour
{

    public AudioSource source;
    public AudioClip clip;


    void Start()
    {
        source.clip = clip;
    }

    public void Reproducir()
    {
        source.Play();
    }
}
