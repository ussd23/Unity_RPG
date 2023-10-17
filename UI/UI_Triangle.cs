using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Triangle : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>(); // �Ž� ���͸� ��������, mf��� �̸��� ���� ����
        Mesh msh = mf.mesh; // �Ž� ������ �Ž��� ������.

        Vector3[] verts = msh.vertices; // vertice�� �迭�� ���ؼ� �ٽ� ������.

        for (int i = 0; i < msh.vertexCount; i++) // msh.vertexCount(������ ����)��ŭ �ݺ�����. 
        {
            if (msh.vertices[i].y < 0.0f) // ����, ���� [i]�� y ���� 0.0f���� Ŭ ���
            {
                verts[i] = new Vector3(0.0f, -0.5f, 0.0f); // i�� ��ǥ ���� (0 ,0.5 ,0)���� �����Ѵ�.
            }
        }
        // ������ ���ؽ��� �ٽ� ����
        msh.SetVertices(verts);
        msh.RecalculateNormals(); // ���� ���� ���͸� ����
        msh.RecalculateTangents(); // ���� ������ ������ ���� ���, ź��Ʈ�� ������ ����Ƽ ���ο��� ź��Ʈ�� ���� ����.
        msh.RecalculateBounds(); // �浹ü�� ���� ����
    }
}
