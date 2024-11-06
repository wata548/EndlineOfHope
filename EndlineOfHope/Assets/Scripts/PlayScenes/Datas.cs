using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Datas : MonoBehaviour
{
    public static Datas Instance { get; private set; } = null;

    [SerializeField]    GameObject  playField;
    public              Vector2     PlayFieldSize    { get; private set; } 
    public              Vector2     PlayFieldPos     { get; private set; } 

    private void Awake() {
        
        if(Instance == null) {

            Instance = this;
        }

        PlayFieldSize = playField.transform.localScale / 2;
        PlayFieldPos = playField.transform.position;

    }

    private void Update() {

        PlayFieldSize = playField.transform.localScale / 2;
        PlayFieldPos = playField.transform.position;

    }
}
