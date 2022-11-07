using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour
{
    private Material material;
    [SerializeField] private float colorSpeed = 5;
    private bool isAntmatedColor = false;
    Color targetColor;
    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        material.color=Color.white;
    }
    private void Update()
    {
        if (isAntmatedColor)
        {
            material.color = Color.Lerp(material.color, targetColor, colorSpeed * Time.deltaTime); 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        isAntmatedColor = true;
    }
    public void LerpColor(Color _targetColor)
    {
        targetColor = _targetColor;
      
    } 
}
