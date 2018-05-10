using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Complex
{
	private float Re;
	private float Im;

	public Complex(){
		Re = 0;
		Im = 0;
	}

	public Complex (float a, float b)
	{
		Re = a;
		Im = b;
	}

	public static bool operator !=(Complex a, Complex b){
		if ((a.Im != b.Im) || (a.Re != b.Re))
			return true;
		return false;
	}

	public static bool operator ==(Complex a, Complex b){
		if ((a.Im == b.Im) && (a.Re == b.Re))
			return true;
		return false;
	}

	public static Complex operator* (Complex a, Complex b){
		Complex res = new Complex();
		res.Re = a.Re * b.Re - a.Im * b.Im;
		res.Im = a.Im * b.Re + a.Re * b.Im;
		return res;
	}

	public static Complex operator+ (Complex a, Complex b){
		Complex res = new Complex();
		res.Re = a.Re + b.Re;
		res.Im = a.Im + b.Im;
		return res;
	}

	public float getRe(){return Re;}

	public float getIm(){return Im;}
}


