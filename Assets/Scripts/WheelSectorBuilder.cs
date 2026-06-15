using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SectorData
{
    public Color color;
    public int count;        // 섹터 개수 (적을수록 희귀)
    public float multiplier; // 배율
}

public class WheelSectorBuilder : MonoBehaviour
{
    [SerializeField]
    public SectorData[] sectors = new SectorData[]
    {
        new SectorData { color = Color.red,    count = 1, multiplier = 10f },
        new SectorData { color = Color.yellow, count = 2, multiplier = 5f  },
        new SectorData { color = Color.cyan,   count = 4, multiplier = 3f  },
        new SectorData { color = Color.green,  count = 8, multiplier = 2f  },
    };

    [SerializeField] private float radius = 1f;
    [SerializeField] private float offset = 0.04f; // 추가

    void Start()
    {
        BuildWheel();
    }

    public List<SectorData> builtSectors = new List<SectorData>(); // 판정용 저장

    void BuildWheel()
    {
        // 섹터 리스트 펼치기
        List<SectorData> flatList = new List<SectorData>();
        foreach (var s in sectors)
            for (int i = 0; i < s.count; i++)
                flatList.Add(s);

        // 셔플
        for (int i = flatList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (flatList[i], flatList[j]) = (flatList[j], flatList[i]);
        }

        builtSectors = flatList; // 판정용 저장

        float sectorAngle = 360f / flatList.Count;
        float currentAngle = 0f;

        foreach (var s in flatList)
        {
            CreateSector(s, currentAngle, currentAngle + sectorAngle);
            currentAngle += sectorAngle;
        }
    }

    public List<Vector3> sectorCenterDirections = new List<Vector3>();
    void CreateSector(SectorData data, float angleStart, float angleEnd)
    {
        float centerAngle = (angleStart + angleEnd) / 2f * Mathf.Deg2Rad;
        Vector3 centerDir = new Vector3(Mathf.Cos(centerAngle), 0, Mathf.Sin(centerAngle));
        sectorCenterDirections.Add(centerDir);

        GameObject sector = new GameObject($"Sector_{data.multiplier}x");
        sector.transform.SetParent(transform, false);

        sector.transform.localPosition = Vector3.zero;
        sector.transform.localRotation = Quaternion.identity;
        sector.transform.localScale = Vector3.one;

        MeshFilter mf = sector.AddComponent<MeshFilter>();
        MeshRenderer mr = sector.AddComponent<MeshRenderer>();

        mf.sharedMesh = CreateSectorMesh(angleStart, angleEnd);

        mr.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mr.material.color = data.color;
    }

    Mesh CreateSectorMesh(float angleStart, float angleEnd)
    {
        int resolution = 20;
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        // XZ 평면으로 수정
        verts.Add(new Vector3(0, offset, 0));

        for (int i = 0; i <= resolution; i++)
        {
            float angle = Mathf.Lerp(angleStart, angleEnd, i / (float)resolution);
            float rad = angle * Mathf.Deg2Rad;

            verts.Add(new Vector3(
                Mathf.Cos(rad) * radius,
                offset,
                Mathf.Sin(rad) * radius  // Y → Z
            ));
        }

        for (int i = 1; i <= resolution; i++)
        {
            tris.Add(0);
            tris.Add(i + 1);
            tris.Add(i);
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}