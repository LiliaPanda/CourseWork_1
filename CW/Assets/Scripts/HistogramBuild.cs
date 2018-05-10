using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class HistogramBuild : MonoBehaviour {

	public int size = 100;

	private Text message;

	private MeshFilter mFilter;

	List<Vector3> Vertices;
	List<int> Triangles;

	public struct MyOperator{
		public int size;
		public Complex[,] matrix;
		public int[] qubits;
	}

	List<MyOperator> operatorsList;

	private Complex[] statesAmplitudes;

	int nQubits;

	string path = "";

	void Start(){
		mFilter = GetComponent<MeshFilter> ();
		message = GetComponent<Text> ();
		//mFilter.mesh = GenerateMesh ();
	}

	public void ProceedHisto(){
		if (AddAlgoFile () && ReadAlgoFromFile ())
			GenerateHisto ();
	}


	float width = Screen.width*0.6f;
	float heigth = Screen.height*0.6f;

	private void GenerateHisto(){
		print ("GH1");
		mFilter  = GetComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();
		print ("GH2");

		int nStates = (int)Math.Pow(2,nQubits);
		print ("GH3");

		statesAmplitudes = new Complex[nStates];
		for (int i = 0; i < nStates; i++)
			statesAmplitudes [i] = new Complex();
		print ("GH4");

		foreach (MyOperator o in operatorsList) {
			print ("GH5");

			CalculateOperation (o);
			print ("GH6");

			GenerateHistoState ();
			print ("GH7");


			mesh.SetVertices(Vertices);
			mesh.SetTriangles (Triangles,0);
			mesh.RecalculateNormals ();
			mFilter.mesh = mesh;
			print ("GH8");

			System.Threading.Thread.Sleep(5000);
		}

		mesh.SetVertices(Vertices);
		mesh.SetTriangles (Triangles,0);
		mesh.RecalculateNormals ();
		print ("GH9");

		mFilter.mesh = mesh;
		print ("GH10");

	}

	private void CalculateOperation(MyOperator o){
		//Complex[] newAmplitudes = new Complex[statesAmplitudes.Length];
		print (statesAmplitudes.Length);
		print (o.size);
		for (int a = 0; a < statesAmplitudes.Length; a++) {
			for (int i = 0; i < o.size; i++) {
				print (o.matrix [i, a]);
				print ("CO1 ");
				statesAmplitudes [i] += o.matrix [i, a];
				print ("CO "+i + " "+a);
			}
		}

		//	statesAmplitudes = newAmplitudes;
	}

	private void GenerateHistoState(){
		print ("GHS1");
		float shift = (float)(width / (Math.Pow (2, nQubits)) / 2);
		size = 100;
		Vertices = new List<Vector3> ();
		Triangles = new List<int> ();
		print ("GHS2");

		for (int a = 0; a < statesAmplitudes.Length; a++) {
			print ("GHS3");

			//Real column
			Vertices.Add (new Vector3 ((float)2*a*shift*size, 0.0f, 0.0f));//left bottom
			print ("GHS31");

			Vertices.Add (new Vector3 ((float)(2*a+1) * shift*size, 0.0f, 0.0f));//right bottom
			print ("GHS32");

			print (statesAmplitudes [a]);
			print ("GHS321");

			Vertices.Add (new Vector3 ((float)(2*a+1) * shift*size, statesAmplitudes[a].getRe()*size, 0.0f));//right top
			print ("GHS33");

			Vertices.Add (new Vector3 ((float)2*a * shift*size, statesAmplitudes[a].getRe()*size, 0.0f));//right top
			//Complex column
			print ("GHS4");

			Vertices.Add (new Vector3 ((float)(2*a+1) * shift*size, 0.0f, 0.0f));//left bottom
			Vertices.Add (new Vector3 ((float)2*(a+1) * shift*size, 0.0f, 0.0f));//right bottom
			Vertices.Add (new Vector3 ((float)2*(a+1) * shift*size, statesAmplitudes[a].getIm()*size, 0.0f));//right top
			Vertices.Add (new Vector3 ((float)(2*a+1) * shift*size, statesAmplitudes[a].getIm()*size, 0.0f));//right top

			print ("GHS5");


			Triangles.Add (0+4*a);
			Triangles.Add (1+4*a);
			Triangles.Add (2+4*a);

			print ("GHS5");


			Triangles.Add (0+4*a);
			Triangles.Add (2+4*a);
			Triangles.Add (3+4*a);

			Triangles.Add (4+4*a);
			Triangles.Add (5+4*a);
			Triangles.Add (6+4*a);

			Triangles.Add (4+4*a);
			Triangles.Add (6+4*a);
			Triangles.Add (7+4*a);
		}
	}

	private bool AddAlgoFile(){
		path = EditorUtility.OpenFilePanel("Choose algorithm description", "", "txt");
		if (path != "")
			return true;
		return false;
	}

	private bool ReadAlgoFromFile(){
		try { 
			//Read operator's matrix size, read matrix
			operatorsList = new List<MyOperator> ();
			string[] Algo = System.IO.File.ReadAllLines(path);

			// file need to contain qubit quantity as first line!
			nQubits = int.Parse(Algo[0]);
			if (nQubits > 10){
				message.text = "Too much qubits! No more than 10 is allowed!";
				throw new UnityException("Too much qubits");
			}
			// let each line to be a permutation - string <name> : <arg1>, <arg2>, <arg3>, ...
			for (int p = 1; p < Algo.Length; ) {
				MyOperator mOperator = new MyOperator();

				mOperator.size = int.Parse(Algo[p]);
				p++;

				Complex[,] a = new Complex[mOperator.size,mOperator.size];
				mOperator.matrix = new Complex[mOperator.size,mOperator.size];
				for (int i = 0; i < mOperator.size; i++){
					string[] COperator = Algo[p+i].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

					for (int j = 0; j < mOperator.size; j++){
						string[] complexOp = COperator[j].Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
						//print (Single.Parse(complexOp[0])+1+" "+Single.Parse(complexOp[1]));
						mOperator.matrix[i,j] = new Complex(Single.Parse(complexOp[0]),Single.Parse(complexOp[1]));
						//mOperator.matrix[i,j] = new Complex(0.0f,1.0f);
					}
				}
				p+=mOperator.size;

				string[] Arguments = Algo[p].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				print ("OK5");

				mOperator.qubits = new int[Arguments.Length];
				for (int i = 0; i < Arguments.Length; i++){
					mOperator.qubits[i] = int.Parse(Arguments[i]);
				}

				operatorsList.Add(mOperator);
				p++;
			}
		}
		catch (Exception e) {
			print (e.Message);
			return false;
		}
		return true;
	}



}
