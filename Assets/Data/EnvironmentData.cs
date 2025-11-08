using UnityEngine;

[CreateAssetMenu(menuName = "Environment Data")]
public class EnvironmentData : ScriptableObject
{
    [Header("Shake Parameters")]
    public float shakeDuration = 0.1f; //Длительность тряски при ударе
    public float shakeMagnitude = 0.25f; //Амплитуда тряски 
    public float dampingSpeed = 1f; // Время затухания

    [Header("Soul Crystal")]
    public int crystalLife = 2;

    [Header("Destructible Wall")]
    public int wallLife = 3;


    //[Header("Claw Destructible Wall")]
    //public int clawWallLife = 1;
}

