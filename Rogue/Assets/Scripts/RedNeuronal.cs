using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedNeuronal : MonoBehaviour {

	private double[,] entrada;
	private double[,] entradaNetaCapaOculta;
	private double[,] salidaCapaOculta;
	private double[,] entradaNetaCapaOculta2;
	private double[,] salidaCapaOculta2;
	private double[,] entradaNetaCapaSalida;

	public double[,] Transpuesta(double [,] matriz){

		double[,] matrizTrans = new double[matriz.GetLength (1), matriz.GetLength(0)];
		Debug.Log("matrizTrans Con Rank " + matrizTrans.GetLength(0));
		Debug.Log ("matrizTrans Con GetLength " + matrizTrans.GetLength(1));

		for (int i = 0; i < matriz.GetLength(0); i++) { //GetLength(0) da las filas
			for (int j = 0; j < matriz.GetLength(1); j++) { // GetLength(1) da las columnas
				matrizTrans [j, i] = matriz [i, j];
			}
		}

		return matrizTrans;
	}


	public double[,] MultiMatrices(double [,] matriz_1, double [,] matriz_2){

		int filas1 = matriz_1.GetLength (0);
		int columnas1 = matriz_1.GetLength (1);
		int filas2 = matriz_2.GetLength (0);
		int columnas2 = matriz_2.GetLength (1);

		double[,] matrizResultante = new double[matriz_1.GetLength (0), matriz_2.GetLength(1)];

		if (columnas1 == filas2) {
			Debug.Log ("El tamaño de las matrices sera: " + matriz_1.GetLength (0) + " " + matriz_2.GetLength(1));
			for (int i = 0; i < matriz_1.GetLength (0); i++) { //GetLength(0) da las filas
				for (int j = 0; j < matriz_2.GetLength (1); j++) { // GetLength(1) da las columnas
					for (int k = 0; k < matriz_1.GetLength (1); k++) { // GetLength(1) da las columnas
						matrizResultante[i,j] += matriz_1[i,k] * matriz_2[k,j];
					}
				}
			}


		} else {
			Debug.Log ("El tamaño de las matrices no es el adecuado");
		}

		return matrizResultante;
	}

	public double[,] matrizRadom(int filas, int columnas){

		double[,] matrizResultante = new double[filas, columnas];

		for (int i = 0; i < filas; i++) {
			for (int j = 0; j < columnas; j++) {
				matrizResultante [i, j] = (double)Random.value;
			}
		}

		return matrizResultante;
	}

	public double[,] Suma_Escalar_a_Matriz (double numero, double[,] matriz){

		double[,] matriz_resultante = new double[matriz.GetLength (0), matriz.GetLength (1)];

		for (int i = 0; i < matriz.GetLength(0); i++) {
			for (int j = 0; j < matriz.GetLength(1); j++) {
				matriz_resultante [i, j] += numero;
			}
		}

		return matriz_resultante;
	}

	public double sigmoidea(double numero){
		///////////////  FALTA CODIGO //////////////////
	}

	public double derivadaSigmoidea(double numero){
		///////////////  FALTA CODIGO //////////////////
	}

	public double funcionActivacion(int numero){
		///////////////  FALTA CODIGO //////////////////
	}

	public void Red_neuronal(double[,] entrada, double[,] pesosCapa1, double[,] pesosCapa2,double[,] pesosCapaSalida){

		//Capa de entrada
		//No se realiza ningun procesamiento
		//*************************************

		//Capa oculta
		double tendenciaCapa1 = 1;
		double[,] entradaNetaCapaOculta = MultiMatrices(pesosCapa1, Transpuesta(entrada));
		entradaNetaCapaOculta = Transpuesta (Suma_Escalar_a_Matriz (tendenciaCapa1, entradaNetaCapaOculta));
		salidaCapaOculta = 1;
	}
		

}
