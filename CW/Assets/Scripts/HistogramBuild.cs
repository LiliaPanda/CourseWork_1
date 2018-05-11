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
		print (statesAmplitudes.Length);
		print (o.size);
		for (int a = 0; a < statesAmplitudes.Length; a++) {					
			// choose only nessasary bits from state for calculation
			int[] bits = Dec2Bin(a);
			int[] nbits;
			for (int i = 0; i < o.qubits.Length; i++) {
				nbits [i] = bits [o.qubits [i]];
			}
			// transform state to vector
			int k = Bin2Dec(nbits);
			double[] vector = new Double[o.size];
			Array.Clear (vector, 0.0, o.size);
			vector [k] = 1.0;

			// apply operator
			vector = MultMatrixVector(o.matrix, vector);
			// change them
			for (int i = 0; i < o.size; i++){
				if (vector [i] > 0.00001 || vector[i] < -0.00001) {
					
				}
			}
			// set up amplitude for state we got after permutation
		}
		return true; 
	}

	// validate operator matrix
	private bool IsValidOperator(double[][] m){
		double[][] mres;
		int size1 = m.GetUpperBound (0);
		int size2 = m.GetUpperBound (1);
		if (size1 != size2)
			return false;
		
		//multiply original matrix with transposed
		for (int i = 0; i < size1; i++) {
			for (int j = 0; j < size2; j++) {
				mres [i] [j] = MultVectorVector (mres [i], mres [j]);
			}
		}
		double[][] ematrix;
		Array.Clear (ematrix, 0.0, size1);
		for (int i = 0; i < size1; i++) {
			ematrix [i] [i] = 1.0;
		}

		for (int i = 0; i < size1; i++){
			for (int j = 0; j < size2; j++) {
				if (mres [i] [j] - ematrix [i] [j] < 0.00001)
					return false;
			}
		}
		return true;
	}

	// permutate multiply matrix with vector
	private bool MultMatrixVector(double[][] m, double[] v){
		if (m.Length != v.Length)
			return false;

		return true;
	}

	// permutate multiply matrix with matrix
	private bool MultMatrixMatrix(double[][] m1, double[][] m2){
		if (m1.Length != m2.Length)
			return false;
		for (int i = 0; i < m1.Length; 
		)
			return true;
	}

	// permutate multiply vector with vector
	private double MultVectorVector(double[] v1, double[] v2){
		double res = 0;
		for (int i = 0; i < v1.Length; i++) {
			res += v1 [i] * v2 [i];
		} 
		return res;
	}

	private int[] Dec2Bin(int a){
		int[] bits;
		for (int i = 0; a > 0; i++) {
			bits [i] = a % 2;
			a /= 2;
		}
		return bits;
	}

	private int Bin2Dec(int[] bits){
		int a = 0;
		int pow = 0;
		for (int i = 0; i < bits.Length; i++ & pow++) {
			a += bits [i] * pow (2, pow);
		}
		return a;
	}

	private void GenerateHisto(){
		mFilter  = GetComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();

		int nStates = (int)Math.Pow(2,nQubits);

		statesAmplitudes = new double[nStates];
		for (int i = 0; i < nStates; i++)
			statesAmplitudes [i] = new double();

		foreach (MyOperator o in operatorsList) {
			CalculateOperation (o);
			GenerateHistoState ();

			mesh.SetVertices(Vertices);
			mesh.SetTriangles (Triangles,0);
			mesh.RecalculateNormals ();
			mFilter.mesh = mesh;

			System.Threading.Thread.Sleep(10000);
		}

		mesh.SetVertices(Vertices);
		mesh.SetTriangles (Triangles,0);
		mesh.RecalculateNormals ();

		mFilter.mesh = mesh;
	}

	private void GenerateHistoState(){
		float expand = (float)(width / (Math.Pow (2, nQubits)));
		float shift = 10;
		size = 100;
		Vertices = new List<Vector3> ();
		Triangles = new List<int> ();

		for (int a = 0; a < statesAmplitudes.Length; a++) {
			//Real column
			Vertices.Add (new Vector3 ((float) a * expand * size + shift, 0.0f, 0.0f));//left bottom
			Vertices.Add (new Vector3 ((float)(a+1) * expand * size, 0.0f, 0.0f));//right bottom
			Vertices.Add (new Vector3 ((float)(a+1) * expand * size + shift, statesAmplitudes[a]*size, 0.0f));//right top
			Vertices.Add (new Vector3 ((float) a * expand * size, statesAmplitudes[a]*size, 0.0f));//right top

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
			for (int p = 1; p < Algo.Length; ) {
				MyOperator mOperator = new MyOperator();

				mOperator.size = int.Parse(Algo[p]);
				p++;

				double[,] a = new double[mOperator.size,mOperator.size];
				mOperator.matrix = new double[mOperator.size,mOperator.size];
				for (int i = 0; i < mOperator.size; i++){
					string[] COperator = Algo[p+i].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

					for (int j = 0; j < mOperator.size; j++){
						string[] complexOp = COperator[j].Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
						//print (Single.Parse(complexOp[0])+1+" "+Single.Parse(complexOp[1]));
						mOperator.matrix[i,j] = new double(Single.Parse(complexOp[0]),Single.Parse(complexOp[1]));
						//mOperator.matrix[i,j] = new double(0.0f,1.0f);
					}
				}
				p += mOperator.size;

				string[] Arguments = Algo[p].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
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
