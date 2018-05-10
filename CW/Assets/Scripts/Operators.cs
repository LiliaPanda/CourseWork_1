using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operators{

	public struct MyOperator{
		public string name;
		public int[][] matrix;
		public int[] qubits;
	}

	public static List<MyOperator> operatorsList = new List<MyOperator>();

	//Allowed gates: CNOT(2), CNOT'(2), T(ofolli)(3), F(redkin)(2), I(1), H(1), X(1), Y(1), Z(1)

}
