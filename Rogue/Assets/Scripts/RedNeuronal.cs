using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedNeuronal : MonoBehaviour {



	public float[,] Transpuesta(float [,] matriz){

		float[,] matrizTrans = new float[matriz.GetLength (1), matriz.GetLength(0)];
		Debug.Log("matrizTrans Con Rank " + matrizTrans.GetLength(0));
		Debug.Log ("matrizTrans Con GetLength " + matrizTrans.GetLength(1));

		for (int i = 0; i < matriz.GetLength(0); i++) { //Rank da las filas
			for (int j = 0; j < matriz.GetLength(1); j++) { // GetLength da las columnas
				matrizTrans [j, i] = matriz [i, j];
			}
		}

		return matrizTrans;
	}


	public void MultiMatrices(){
	}
		

}
