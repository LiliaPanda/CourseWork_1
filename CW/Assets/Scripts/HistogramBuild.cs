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

	int MAX_QUBITS = 6;

	private Text message;

	private MeshFilter mFilter;

	List<Vector3> Vertices;
	List<int> Triangles;

	public struct MyOperator{
		public int size;
		public double[][] matrix;
		public int[] qubits;
	}

	List<MyOperator> operatorsList;

	private double[] statesAmplitudes;

	int nQubits;
	int curOpSize;

	string path = "";

	float width = Screen.width*0.6f;
	float heigth = Screen.height*0.6f;

	void Start(){
		mFilter = GetComponent<MeshFilter> ();
		message = GetComponent<Text> ();
		//mFilter.mesh = GenerateMesh ();
	}

	public void ProceedHisto(){
		if (AddAlgoFile () && ReadAlgoFromFile ())
			GenerateHisto ();
	}

	// permutate operation
	private bool CalculateOperation(MyOperator o){
		if (!IsValidOperator (o.matrix))
			return false;
		int qsize = o.qubits.Length;
		if (Math.Pow (2, qsize) != o.size)
			return false;
		curOpSize = o.size;
		for (int a = 0; a < statesAmplitudes.Length; a++) {	
			string res = "";
			for (int i = 0; i < statesAmplitudes.Length; i++)
				res = res + " " + statesAmplitudes [i];
			print (res+" first "+a);
			// choose only nessasary bits from state for calculation
			int[] bits = new Int32[nQubits];
			Dec2Bin (a, ref bits);

			int[] nbits = new Int32[qsize]; //nbits length - qsize
			for (int i = 0; i < qsize; i++) {
				nbits[i] = bits [o.qubits [i]];
			}

			// transform state to vector
			double[] vector = new Double[o.size];
			for (int i = 0; i < o.size; i++) {
				vector [i] = 0.0;
			}
			int k = 0;
			Bin2Dec (ref k, nbits);
			vector [k] = 1.0;

			// apply operator
			vector = MultMatrixVector(o.matrix, vector);

			// change them
			for (int i = 0; i < o.size; i++){
				// if vector [i] != 0, so binary decomposition of i need to be setup for some places in bits
				if (vector [i] > 0.00001 || vector[i] < -0.00001) {
					Dec2Bin (i, ref nbits);

					// change bits in decomposition
					for (int j = 0; j < qsize; j++) {
						bits[o.qubits [j]] = nbits[j];
					}

					// set up amplitudes
					Bin2Dec (ref k, bits);
					statesAmplitudes [k] += vector[i];
				}
			}
			res = "";
			for (int i = 0; i < statesAmplitudes.Length; i++)
				res = res + " " + statesAmplitudes [i];
			print (res+" second "+a);
		}
		return true; 
	}

	// validate operator matrix
	private bool IsValidOperator(double[][] m){
		int size1 = m.Length;
		int size2 = m[0].Length;
		double[][] mres = new Double[size1] [];
		for (int i = 0; i < size1; i++) {
			mres [i] = new Double[size2];
		}
		if (size1 != size2)
			return false;
		
		//multiply original matrix with transposed
		for (int i = 0; i < size1; i++) {
			for (int j = 0; j < size2; j++) {
				mres [i] [j] = MultVectorVector (mres [i], mres [j]);
			}
		}

		for (int i = 0; i < size1; i++) {
			for (int j = 0; j < size2; j++) {
				if ((i == j) && (mres [i] [j] - 1 > 0.00001))
					return false;
				if ((i != j) && (mres [i] [j] > 0.00001))
					return false;
			}
		}

		return true;
	}

	// permutate multiply matrix with vector
	private double[] MultMatrixVector(double[][] m, double[] v){
		double[] res = new Double[m[0].Length];
		if (m.Length != v.Length)
			return res;
		
		for (int i = 0; i < m.Length; i++) {
			res [i] = 0;
			for (int j = 0; j < v.Length; j++) {
				res [i] += m [i] [j] * v [j];
			} 
		}
		return res;
	}

	// permutate multiply vector with vector
	private double MultVectorVector(double[] v1, double[] v2){
		double res = 0;
		if (v1.Length != v2.Length)
			return res;
		for (int i = 0; i < v1.Length; i++) {
			res += v1 [i] * v2 [i];
		} 
		return res;
	}

	// permutate multiply matrix with matrix
	/*private bool MultMatrixMatrix(double[][] m1, double[][] m2){
		if (m1.Length != m2.Length)
			return false;
		for (int i = 0; i < m1.Length; 
		)
			return true;
	}*/

	private void Dec2Bin(int a, ref int[] bits){
		for (int i = 0; i < bits.Length; i++) {
			bits [i] = 0;
		}
		for (int i = bits.Length-1; a > 0; i--) {
			bits [i] = a % 2;
			a /= 2;
		}
	}

	private void Bin2Dec(ref int a, int[] bits){
		a = 0;
		int pow = 0;
		for (int i = bits.Length-1; i >= 0; i-- , pow++) {
			a += bits [i] * (int)Math.Pow (2, pow);
		}
	}

	private void GenerateHisto(){
		mFilter  = GetComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();

		int nStates = (int)Math.Pow(2,nQubits);

		statesAmplitudes = new double[nStates];

		foreach (MyOperator o in operatorsList) {
			CalculateOperation (o);
			GenerateHistoState ();

			mesh.SetVertices(Vertices);
			mesh.SetTriangles (Triangles,0);
			mesh.RecalculateNormals ();
			mFilter.mesh.Clear ();
			mFilter.mesh = mesh;

			System.Threading.Thread.Sleep(2000);
		}

		mesh.SetVertices(Vertices);
		mesh.SetTriangles (Triangles,0);
		mesh.RecalculateNormals ();

		mFilter.mesh.Clear ();
		mFilter.mesh = mesh;
	}

	private void GenerateHistoState(){
		float expand = (float)(width / 1920);
		float shift = 10;
		float colWidth = 200;
		float colHeight = 500;
		Vertices = new List<Vector3> ();
		Triangles = new List<int> ();

		for (int a = 0; a < statesAmplitudes.Length; a++) {
			//Real column
			Vertices.Add (new Vector3 ((float) a * (shift + colWidth) * expand + shift , 0.0f, 0.0f));//left bottom
			Vertices.Add (new Vector3 ((float) a * (shift + colWidth) * expand + shift, (float)statesAmplitudes[a]*colHeight, 0.0f));//right bottom
			Vertices.Add (new Vector3 ((float) (a+1) * (shift + colWidth) * expand, (float)statesAmplitudes[a]*colHeight, 0.0f));//right top
			Vertices.Add (new Vector3 ((float) (a+1) * (shift + colWidth) * expand, 0.0f, 0.0f));//right top

			Triangles.Add (0+4*a);
			Triangles.Add (1+4*a);
			Triangles.Add (2+4*a);

			Triangles.Add (0+4*a);
			Triangles.Add (2+4*a);
			Triangles.Add (3+4*a);
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
			if (nQubits > MAX_QUBITS){
				message.text = "Too much qubits! No more than " + MAX_QUBITS + " is allowed!";
				throw new UnityException("Too much qubits");
			}
			// let each line to be a permutation - string <name> : <arg1>, <arg2>, <arg3>, ...
			for (int op = 1; op < Algo.Length; ) {
				MyOperator mOperator = new MyOperator();

				// parse operator matrix size
				mOperator.size = int.Parse(Algo[op]);
				op++;
				// parse operator matrix
				mOperator.matrix = new double[mOperator.size][];
				for (int i = 0; i < mOperator.size; i++){
					mOperator.matrix[i] = new Double[mOperator.size];
					string[] Operator = Algo[op+i].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
					for (int j = 0; j < Operator.Length; j++){
						mOperator.matrix[i][j] = Single.Parse(Operator[j]);
					}
				}
				op += mOperator.size;

				string[] Arguments = Algo[op].Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
				mOperator.qubits = new int[Arguments.Length];
				for (int i = 0; i < Arguments.Length; i++){
					mOperator.qubits[i] = int.Parse(Arguments[i]);
				}

				operatorsList.Add(mOperator);
				op++;
			}
		}
		catch (Exception e) {
			print (e.Message);
			return false;
		}
		return true;
	}



}
