using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotations : MonoBehaviour
{
    public float rotateSpeed = 10f; // �������� ��������

    public Transform leftColumn; // ������ �� ����� �������
    public Transform rightColumn; // ������ �� ������ �������

    void Update()
    {
        // ��������� ������ �������� �� ��� Y
        Vector3 rotation = Vector3.left * rotateSpeed * Time.deltaTime;

        // ������� ����� ������� �����
        leftColumn.Rotate(-rotation);

        // ������� ������ ������� ������
        rightColumn.Rotate(rotation);
    }
}
