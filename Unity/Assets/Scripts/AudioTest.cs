using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioTest : MonoBehaviour {
    public bool autoPlay = false;
    public enum TipoOnda {  Sinusoidal, Cuadrada, Triangular, FM};
    [Range(0.0f, 1.0f)]
    public float volumen = 0.5f;
    [Range(100, 2000)]
    public double frecuencia = 440;
    [Range(0.0f, 1.0f)]
    public float paneo;
    public double[] frecuencias = new double[8];
    public TipoOnda tipoOnda;
    private double rate;
    double incremento;
    float mod;
    double fase=0;
    
    float leftPan;
    float rightPan;
    [Header("FM")]
    [Range(0.0f, 2.0f)]
    public float beta = 0;
    [Range(100, 2000)]
    public float fm;
    int keyPressed=-1;
    
    void Awake()
    {
        rate = AudioSettings.outputSampleRate;

    }
    void Update()
    {
        //Actualiza la frecuencia actual según la tecla que hayas pulsado
        keyPressed = HandleInput();
        if (keyPressed != -1)
        {
            frecuencia = frecuencias[keyPressed];
        }
        //Actualiza el porcentaje de paneo de cada lado
        rightPan = paneo;
        leftPan = 1.0f - rightPan;
    }

   
   
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (keyPressed != -1 || autoPlay)
        {
            incremento = frecuencia * 2.0 * Mathf.PI / rate;           
            
            for (int i = 0; i < data.Length; i += channels)
            {
                fase += incremento;
               
                switch (tipoOnda)
                {
                    case TipoOnda.Sinusoidal:
                        //sin( f * 2 * PI )
                        data[i] = volumen * (float)(Mathf.Sin((float)fase));
                        break;
                    case TipoOnda.Cuadrada:
                        //Para sin(f*2*PI) positivo a constante positiva                        
                        if (volumen * (float)(Mathf.Sin((float)fase)) >= 0)
                        {
                            data[i] = (float)volumen * 0.5f;
                        }
                        else
                        {//Para sin(f*2*PI) negativo a constante negativa
                            data[i] = (-(float)volumen) * 0.5f;
                        }
                        break;
                    case TipoOnda.Triangular:
                        //Onda con la función PingPong que oscila entre un rango de forma constante
                        data[i] = (float)(volumen * (double)Mathf.PingPong((float)fase, 1.0f));
                        break;
                    case TipoOnda.FM:
                        //Frecuencia modulada
                        mod = beta * Mathf.Sin(2.0f * Mathf.PI * fm / (float)rate);
                        //Suma a la fase la frecuencia moduladora
                        //sin(f*2*PI + beta*sin(fmod*2*PI))
                        fase += mod;
                        data[i] = volumen * (float)(Mathf.Sin((float)fase));
                        break;
                }

                if (channels == 2)
                {
                    //i impar canal para altavoz derecho, i par altavoz izquierdo
                    data[i + 1] = rightPan*data[i];
                    data[i] *= leftPan;
                }
                if (fase > (Mathf.PI * 2))
                {
                    fase -= Mathf.PI * 2;
                }
            }
        }
        
    }

    int HandleInput()
    {
        //Según la tecla que pulses devuelve un int que se usará después para el array de frecuencias
        if (Input.GetKey(KeyCode.Alpha1))
        {
            return 0;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            return 1;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            return 2;
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            return 3;
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            return 4;
        }
        else if (Input.GetKey(KeyCode.Alpha6))
        {
            return 5;
        }
        else if (Input.GetKey(KeyCode.Alpha7))
        {
            return 6;
        }
        else if (Input.GetKey(KeyCode.Alpha8))
        {
            return 7;
        }
        else return -1;
    }
}
