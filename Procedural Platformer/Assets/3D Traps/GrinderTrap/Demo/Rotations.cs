using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotations : MonoBehaviour
{
    public float rotateSpeed = 10f; // скорость вращения

    public Transform leftColumn; // ссылка на левую колонну
    public Transform rightColumn; // ссылка на правую колонну

    void Update()
    {
        // Вычисляем вектор вращения по оси Y
        Vector3 rotation = Vector3.left * rotateSpeed * Time.deltaTime;

        // Вращаем левую колонну влево
        leftColumn.Rotate(-rotation);

        // Вращаем правую колонну вправо
        rightColumn.Rotate(rotation);
    }
}
