using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

[RequireComponent(typeof(MeshFilter))]
public class Main : MonoBehaviour {

	public int size = 100;
	public float rotSpeed = 20;
	private MeshFilter mFilter;

	string path = "";

	List<Vector3> Vertices = new List<Vector3>();
	List<int> Triangles = new List<int>();
	int VerticesCount = 0;

	public InputField VerticesInput;
	public InputField FacetsInput;

    public void Generate(){
		mFilter = GetComponent<MeshFilter> ();
		ReadMeshFromFile ();
		mFilter.mesh = GenerateMesh ();
	}

	void Update(){

		if (Input.GetMouseButton (0)) {
			float rotX = Input.GetAxis ("Mouse X") * rotSpeed * Mathf.Deg2Rad;
			float rotY = Input.GetAxis ("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

			transform.RotateAround (Vector3.up, -rotX);
			transform.RotateAround (Vector3.right, rotY);
		}
	}

	public void AddPolyhedraFile(){
		path = EditorUtility.OpenFilePanel("Choose polyhedra description", "", "txt");
	}

	void ReadMeshFromFile(){
		try { 
			string[] Facets = System.IO.File.ReadAllLines(path);//each line is facet, vertices in order to go,counterclockwise
			//Well!
			//needed time - facets*point_in_facet
			// for each facet-line
			for (int f = 0; f < Facets.Length; f++) {
				//let each point separated by comma
				string[] FacetStrPoints;
				FacetStrPoints = Facets[f].Split (new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
				//for each point get coordinates
				int vc = FacetStrPoints.Length;
				for (int p = 0; p < vc; p++){
					string[] PointStr = FacetStrPoints [p].Split (new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
					Vertices.Add(new Vector3(Single.Parse(PointStr[0])*size,Single.Parse(PointStr[1])*size,Single.Parse(PointStr[2])*size));
				}
				//three vertices for each triangle
				for (int t = 1; t < vc-1; t ++){
					Triangles.Add(VerticesCount);
					Triangles.Add(VerticesCount+t);
					Triangles.Add(VerticesCount+t+1);
					print(Triangles.Count);
				}
				VerticesCount+= vc;
			}
		}
		catch (Exception) {
			print ("troubles in file!");
		}
	}

	Mesh GenerateMesh(){
		Mesh mesh = new Mesh ();
		mesh.SetVertices(Vertices);

		mesh.SetTriangles (Triangles,0);
		mesh.RecalculateNormals ();

		return mesh;
	}

}
