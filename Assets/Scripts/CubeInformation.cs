using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CubeInformation : MonoBehaviour
{
    [System.NonSerialized] public List<GameObject> vertexList = new List<GameObject>(); // 全ての頂点のリスト
    [System.NonSerialized] public List<GameObject> edgeList = new List<GameObject>(); // 全ての辺のリスト
    [System.NonSerialized] public List<GameObject> faceList = new List<GameObject>(); // 全ての面のリスト

    [System.NonSerialized] public List<List<GameObject>> vertexList_FromEdge = new List<List<GameObject>>(); // 辺に接する頂点のリスト
    [System.NonSerialized] public List<List<GameObject>> faceList_FromEdge = new List<List<GameObject>>();   // 辺に接する面のリスト
    [System.NonSerialized] public List<List<GameObject>> edgeList_FromFace = new List<List<GameObject>>();   // 面に接する辺のリスト

    private Collider[] colList;
    private bool openingSequence = true;
    private EdgeInformation edge;
    private FaceInformation face;

    private float searchDis_vertex = 2.0f; // 頂点の原点位置からの探索範囲、ステージの大きさに依存する！Debug_Countの値を確認して都度調整
    private float searchDis_edge = 2.0f; // 辺の原点位置からの探索範囲、ステージの大きさに依存する！Debug_Countの値を確認して都度調整

    // Update is called once per frame
    void Update()
    {
        if (openingSequence) // 開幕処理
        {
            ObjectClassification(); // 全ての頂点・辺・面をリスト化
            NumberingAndMakeLists(); // 辺と面に識別番号を付与、新規リストの作成
            MakeLists1_AroundVertex(); // 頂点探索の実行し、辺に接する頂点をリスト化
            MakeLists2_AroundEdge(); // 辺探索（各辺の原点位置から円形に）の実行、辺に接する面のリスト化

            //Debug_Count(); // デバッグ用（範囲探索距離の調整に用いる）

            openingSequence = false;
        }
    }

    void ObjectClassification()
    {
        // HitBoxの子オブジェクトの全ての頂点・辺・面のリスト化
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.CompareTag("Vertex"))
            {
                vertexList.Add(child.gameObject);
            }
            else if (child.gameObject.CompareTag("Edge"))
            {
                edgeList.Add(child.gameObject);
            }
            else if (child.gameObject.CompareTag("Face"))
            {
                faceList.Add(child.gameObject); // 現時点では不要
            }
        }
    }

    void NumberingAndMakeLists()
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            edge = edgeList[i].GetComponent<EdgeInformation>();
            edge.EdgeNum = i; // 辺に識別番号を付与

            vertexList_FromEdge.Add(new List<GameObject>()); // 辺の数だけ新規リストを作成
            faceList_FromEdge.Add(new List<GameObject>()); // 辺の数だけ新規リストを作成
        }

        for (int j = 0; j < faceList.Count; j++)
        {
            face = faceList[j].GetComponent<FaceInformation>();
            face.FaceNum = j; // 面に識別番号を付与

            edgeList_FromFace.Add(new List<GameObject>()); // 面の数だけ新規リストを作成
        }
    }

    void MakeLists1_AroundVertex()
    {
        for (int i = 0; i < vertexList.Count; i++)
        {
            colList = Physics.OverlapSphere(vertexList[i].transform.position, searchDis_vertex); // 各頂点の原点位置を中心にオブジェクト探索

            for (int j = 0; j < colList.Length; j++)
            {
                if (colList[j].gameObject.CompareTag("Edge"))
                {
                    edge = colList[j].gameObject.GetComponent<EdgeInformation>();
                    // EdgeInformationに、辺に接する２頂点のtransform情報を与える
                    if (edge.vertex[0] == Vector3.zero)
                    {
                        edge.vertex[0] = vertexList[i].transform.position;
                    }
                    else
                    {
                        edge.vertex[1] = vertexList[i].transform.position;
                    }

                    int num = edge.EdgeNum; // 辺の識別番号 edgeNum を取得する
                    // 辺に接する頂点のリスト化
                    vertexList_FromEdge[num].Add(vertexList[i].gameObject); // ◆今のところデバッグ用としてのみの使用◆頂点のgameObject = vertexList[i].gameObject
                }
            }
        }
    }

    void MakeLists2_AroundEdge()
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            colList = Physics.OverlapSphere(edgeList[i].transform.position, searchDis_edge); // 各辺の原点位置を中心にオブジェクト探索
            edge = edgeList[i].GetComponent<EdgeInformation>();

            for (int j = 0; j < colList.Length; j++)
            {
                if (colList[j].gameObject.CompareTag("Face"))
                {
                    ////////////////////////////////
                    //// 辺に接する面のリスト化 ////
                    ////////////////////////////////

                    // EdgeInformationに、辺に接する２面のtransform情報を与える
                    if (edge.face[0] == Vector3.zero)
                    {
                        edge.face[0] = colList[j].gameObject.transform.position;
                    }
                    else
                    {
                        edge.face[1] = colList[j].gameObject.transform.position;
                    }

                    // 辺に接する面のリスト化
                    faceList_FromEdge[i].Add(colList[j].gameObject); // ◆今のところデバッグ用としてのみの使用◆辺の識別番号 = i、面のgameObject = colList[j].gameObject

                    ////////////////////////////////
                    //// 面に接する辺のリスト化 ////
                    ////////////////////////////////

                    face = colList[j].gameObject.GetComponent<FaceInformation>();
                    // FaceInformationに、面に接する４辺のtransform情報を与える
                    if (face.edge[0] == Vector3.zero)
                    {
                        face.edge[0] = edgeList[i].transform.position;
                    }
                    else if (face.edge[1] == Vector3.zero)
                    {
                        face.edge[1] = edgeList[i].transform.position;
                    }
                    else if (face.edge[2] == Vector3.zero)
                    {
                        face.edge[2] = edgeList[i].transform.position;
                    }
                    else
                    {
                        face.edge[3] = edgeList[i].transform.position;
                    }

                    // 面に接する辺のリスト化
                    edgeList_FromFace[face.FaceNum].Add(edgeList[i]); // ◆今のところデバッグ用としてのみの使用◆面の識別番号 = face.FaceNum、辺のgameObject = edgeList[i]
                }
            }
        }
    }

    void Debug_Count()
    {
        for (int i = 0; i < vertexList_FromEdge.Count; i++)
        {
            // Debug.Log("edgeに接するvertexの数 = " + vertexList_FromEdge[i].Count);
            if (vertexList_FromEdge[i].Count != 2)
            {
                Debug.Log("エラー！辺に接する頂点の数が２ではありません！");
            }
        }

        for (int i = 0; i < faceList_FromEdge.Count; i++)
        {
            // Debug.Log("edgeに接するfaceの数 = " + faceList_FromEdge[i].Count);
            if (faceList_FromEdge[i].Count != 2)
            {
                Debug.Log("エラー！辺に接する面の数が２ではありません！");
            }
        }

        for (int i = 0; i < edgeList_FromFace.Count; i++)
        {
            Debug.Log("faceに接するedgeの数 = " + edgeList_FromFace[i].Count);
            if (edgeList_FromFace[i].Count != 4)
            {
                Debug.Log("エラー！辺に接する面の数が４ではありません！");
            }
        }
    }
}