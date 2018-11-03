using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class RedNeuronal {


	public double[,] Transpuesta(double [,] matriz){

		double[,] matrizTrans = new double[matriz.GetLength (1), matriz.GetLength(0)];

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
				matrizResultante [i, j] = (double) UnityEngine.Random.value;
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

	public double[,] Suma_Resta_Matrices (double[,] matriz1, double[,] matriz2, int tipo_operacion){

		double[,] matriz_resultante = new double[matriz1.GetLength (0), matriz1.GetLength (1)];

		if (matriz1.GetLength (0) == matriz1.GetLength (1) && matriz2.GetLength (0) == matriz2.GetLength (1)) {
			
			for (int i = 0; i < matriz1.GetLength(0); i++) {
				for (int j = 0; j < matriz1.GetLength(1); j++) {
					if (tipo_operacion == 0) {
						matriz_resultante [i, j] = matriz1[i,j] + matriz2[i,j];
					}else if (tipo_operacion == 1) {
						matriz_resultante [i, j] = matriz1[i,j] - matriz2[i,j];
					}

				}
			}
		}



		return matriz_resultante;
	}

	public double[,] Sacar_fila_de_matriz(int fila, double[,] matriz){
		
		double[,] matriz_resultante = new double[1, matriz.GetLength (1)];

		for (int i = 0; i < matriz.GetLength(1); i++) {
			matriz_resultante [0, i] += matriz[fila,i];
		}

		return matriz_resultante;
	}

	public double[,] Copiar_Matriz(double[,] matriz1){

		double[,] matriz_resultante = new double[matriz1.GetLength (0), matriz1.GetLength (1)];

		for (int i = 0; i < matriz1.GetLength(0); i++) {
			for (int j = 0; j < matriz1.GetLength(1); j++) {
				matriz_resultante [i, j] += matriz1[i,j];
			}
		}

		return matriz_resultante;
		
	}
		

	public double[,] Aplicar_funcion_activacion_a_matriz (double[,] matriz){

		double[,] matriz_resultante = new double[matriz.GetLength (0), matriz.GetLength (1)];

		for (int i = 0; i < matriz.GetLength(0); i++) {
			for (int j = 0; j < matriz.GetLength(1); j++) {
				matriz_resultante [i, j] += FuncionActivacion(matriz[i,j]);
			}
		}

		return matriz_resultante;
	}

	public void ImprimirMatriz(double[,] matriz){
		Debug.Log ("Imprimiendo matriz");
		for (int i = 0; i < matriz.GetLength (0); i++) {
			string linea= "";
			for (int j = 0; j < matriz.GetLength (1); j++) {
				linea += matriz [i, j] + " ";
			}
			Debug.Log (linea + "fin de la fila");
		}
		Debug.Log ("Fin");
	}

	public double Sigmoidea(double numero){
//		if (numero > 45) {
//			return 1;
//		} else {
//			if (numero < -45)
//				return 0;
//			else {
//				return  1.0 / (1 + Mathf.Exp((float)-numero));
//			}
//		}
		return  1.0 / (1 + Mathf.Exp((float)-numero));
	}

	public double DerivadaSigmoidea(double numero){
		return Sigmoidea(numero)*(1-Sigmoidea(numero));
	}

	public double FuncionActivacion(double numero){
		double resultado;
		resultado = Sigmoidea (numero);
		return resultado;
	}

	public string Decimal_a_binario(int numero, int tamañoRequerido, bool usarCeros){

		string cadena = "";
		int tamaño = 0;
		if (numero > 0) {

			while (numero > 0) {
				if (numero % 2 == 0) {
					if (usarCeros == false) {
						cadena = "-1 " + cadena;
					} else {
						cadena = "0 " + cadena;
					}
					tamaño++;
				} else {
					cadena = "1 " + cadena;
					tamaño++;
				}
				numero = (int)(numero / 2);
			}

		} else if (numero == 0) {
			if (usarCeros == false) {
				cadena = "-1";
			} else {
				cadena = "0";
			}
			tamaño++;
		}

		if (tamaño >= tamañoRequerido) {
			return cadena;
		} else {
			int diferencia = Mathf.Abs (tamaño - tamañoRequerido);
			for (int i = 0; i < diferencia; i++) {
				if (usarCeros == false) {
					cadena = "-1 " + cadena;
				} else {
					cadena = "0 " + cadena;
				}
			}
			return cadena;
		}
	}

	public void ExecuteCommand ()
	{
		string comandos = "cd Assets & python rrnn.py";
		//Crear proceso
		System.Diagnostics.Process process = new System.Diagnostics.Process();

		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();



		//Aqui se puede usar Hidden para no mostrar la ventana del CMD

		// Esto es útil para procesos en los que no quieras mostrar la ventana

		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal; //Hidden, Maximized, Minimized, Normal

		startInfo.FileName = "cmd.exe";

		startInfo.Arguments = "/c" + comandos;

		//process.EnableRaisingEvents = true;

		//process.Exited += (sender, e) => { Finalizado(); }; //Método al cual se llamará al finalizar

		process.StartInfo = startInfo;
		process.WaitForExit();
		process.Start();

	}

	public void Finalizado() {	

		Debug.Log("Proceso Finalizado!");	

	}


}
