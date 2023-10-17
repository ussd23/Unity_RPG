using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Triangle : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>(); // 매쉬 필터를 가져오고, mf라는 이름의 변수 지정
        Mesh msh = mf.mesh; // 매쉬 필터의 매쉬를 가져옴.

        Vector3[] verts = msh.vertices; // vertice를 배열을 통해서 다시 세팅함.

        for (int i = 0; i < msh.vertexCount; i++) // msh.vertexCount(정점의 개수)만큼 반복하자. 
        {
            if (msh.vertices[i].y < 0.0f) // 만약, 정점 [i]의 y 값이 0.0f보다 클 경우
            {
                verts[i] = new Vector3(0.0f, -0.5f, 0.0f); // i의 좌표 값을 (0 ,0.5 ,0)으로 조정한다.
            }
        }
        // 조정된 버텍스를 다시 세팅
        msh.SetVertices(verts);
        msh.RecalculateNormals(); // 면의 방향 벡터를 재계산
        msh.RecalculateTangents(); // 면의 오른쪽 방향의 벡터 계산, 탄젠트인 이유는 유니티 내부에서 탄젠트로 쓰고 있음.
        msh.RecalculateBounds(); // 충돌체에 대한 재계산
    }
}
