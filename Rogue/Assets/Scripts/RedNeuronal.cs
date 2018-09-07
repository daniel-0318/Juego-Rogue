using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedNeuronal {


	private double factorEntrenamiento = 0.9;
	private double errorPermitido = 0.1;

	private double[,] entrada;
	private double[,] entradaNetaCapaOculta;
	private double[,] salidaCapaOculta;
	private double[,] entradaNetaCapaOculta2;
	private double[,] salidaCapaOculta2;
	private double[,] entradaNetaCapaSalida;
	private double[,] salida;

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

	public double Sigmoidea(double numero){
		if (numero > 45) {
			return 1;
		} else {
			if (numero < -45)
				return 0;
			else {
				return  1.0 / (1 + Mathf.Exp((float)-numero));
			}
		}
	}

	public double DerivadaSigmoidea(double numero){
		return Sigmoidea(numero)*(1-Sigmoidea(numero));
	}

	public double FuncionActivacion(double numero){
		double resultado;
		resultado = Sigmoidea (numero);
		return resultado;
	}

	public double[,] Red_neuronal(double[,] entrada, double[,] pesosCapa1, double[,] pesosCapa2,double[,] pesosCapaSalida){

		//Capa de entrada
		//No se realiza ningun procesamiento
		//*************************************

		//Capa oculta
		double tendenciaCapa1 = 1;
		entradaNetaCapaOculta = MultiMatrices(pesosCapa1, Transpuesta(entrada));
		entradaNetaCapaOculta = Transpuesta (Suma_Escalar_a_Matriz (tendenciaCapa1, entradaNetaCapaOculta));
		salidaCapaOculta = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaOculta);

		//Capa oculta2
		double tendenciaCapa2 = 1;
		entradaNetaCapaOculta2 = MultiMatrices(pesosCapa2, Transpuesta(salidaCapaOculta));
		entradaNetaCapaOculta2 = Transpuesta (Suma_Escalar_a_Matriz (tendenciaCapa2, entradaNetaCapaOculta2));
		salidaCapaOculta2 = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaOculta2);

		//Capa de salida
		double tendenciaCapa3 = 1;
		entradaNetaCapaSalida = MultiMatrices(pesosCapaSalida, Transpuesta(salidaCapaOculta2));
		entradaNetaCapaSalida = Transpuesta (Suma_Escalar_a_Matriz (tendenciaCapa3, entradaNetaCapaSalida));
		salida = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaSalida);


		return salida;  /////////// En si no es necesario devolverlo porque es una variable de la clase mirar como seria mejor /////////////////////////
	}


	public void Backpropagation(double[,] entradaDeseada, double[,] salidaDeseada){

		//Pesos Aleatorios
		double[,] pesosCapaOculta = matrizRadom (3, 3);
		double[,] pesosCapaOculta2 = matrizRadom (2, 3);
		double[,] pesosCapaSalida = matrizRadom (1, 2);

		////// FALTARIA QUE SE COGAN PATRONES ALEATORIOS

		int iteraciones = 0;

		for (int i = 0; i < entradaDeseada.GetLength (0); i++) {
			iteraciones++;
			double[,] pesosCapaOcultaActual = Copiar_Matriz(pesosCapaOculta);
			double[,] pesosCapaOcultaActual2 = Copiar_Matriz(pesosCapaOculta2);
			double[,] pesosCapaSalidaActual = Copiar_Matriz(pesosCapaSalida);


			double[,] salida = Red_neuronal (Sacar_fila_de_matriz (i, entradaDeseada), pesosCapaOcultaActual, pesosCapaOculta2, pesosCapaSalidaActual);

			Debug.Log ("-----------------------------------------------");
			Debug.Log ("la salida fue: " + salida[0,0]);
			Debug.Log ("Tamaño de la matriz: filas: " + salida.GetLength(0) + " columnas: " + salida.GetLength(1) );

			Debug.Log ("Valor antes de modificiar: " + pesosCapaOculta[0,0]);
			pesosCapaOculta [0, 0] = 18;
			Debug.Log ("Valor despues de modificar: " + pesosCapaOculta[0,0]);
			Debug.Log ("El valor que no devio de modificarse: " + pesosCapaOcultaActual[0,0]);


			//Miramos la neurona de la capa de salida para ir corrigiendo pesos. Como es una una sola la miramos a ella.

			double errorCapaDeSalida = (salidaDeseada[0,i] - salida[0,0])*DerivadaSigmoidea (entradaNetaCapaSalida [0, 0]);
			Debug.Log ("salidaDeseada: " + salidaDeseada [0, i]); 
			Debug.Log ("salida: " + salida [0, 0]);
			Debug.Log ("resta: " +(salidaDeseada[0,i] - salida[0,0]));
			Debug.Log ("entradaNetaCapaSalida: " + entradaNetaCapaSalida [0, 0]);
			Debug.Log ("Error en la salida: " + errorCapaDeSalida);

			//Actualizamos pesos de la capa de salida
			Debug.Log("salidaCapaOculta2 tamaño: " + salidaCapaOculta2.GetLength(0) + " y " + salidaCapaOculta2.GetLength(1));
			pesosCapaSalida[0,0] += factorEntrenamiento*errorCapaDeSalida*salidaCapaOculta2[0,0];
			pesosCapaSalida[0,0] += factorEntrenamiento*errorCapaDeSalida*salidaCapaOculta2[0,1];

			//Dos memorias en la segunda capa oculta, entonces actualizamos pesos
			//El error total de la capa oculta, es el error de la cada de salida
			//propagamos hacia atras el error de saliida utilizando el peso
			double errorNeurona1CapaOculta2 = DerivadaSigmoidea(entradaNetaCapaOculta2[0,0])*pesosCapaSalidaActual[0,0]*errorCapaDeSalida;
			pesosCapaOculta2 [0, 0] += factorEntrenamiento * errorNeurona1CapaOculta2 * entradaDeseada [i, 0];
			pesosCapaOculta2 [0, 1] += factorEntrenamiento * errorNeurona1CapaOculta2 * entradaDeseada [i, 1];
			pesosCapaOculta2 [0, 2] += factorEntrenamiento * errorNeurona1CapaOculta2 * entradaDeseada [i, 2];

			double errorNeurona2CapaOculta2 = DerivadaSigmoidea(entradaNetaCapaOculta2[0,1])*pesosCapaSalidaActual[0,1]*errorCapaDeSalida;
			pesosCapaOculta2 [1, 0] += factorEntrenamiento * errorNeurona2CapaOculta2 * entradaDeseada [i, 0];
			pesosCapaOculta2 [1, 1] += factorEntrenamiento * errorNeurona2CapaOculta2 * entradaDeseada [i, 1];
			pesosCapaOculta2 [1, 2] += factorEntrenamiento * errorNeurona2CapaOculta2 * entradaDeseada [i, 2];


			//Tres neuronas en la primera capa oculta, entonces actualizamos pesos
			//El error total de la capa oculta, es el error de la capa oculta2
			//propagamos hacia atras el error de saliida utilizando el peso
			Debug.Log("Tamaño de entradaNetaCapaOculta: " + entradaNetaCapaOculta.GetLength(0) + " " + entradaNetaCapaOculta.GetLength(1) );

			double errorNeurona1CapaOculta = DerivadaSigmoidea (entradaNetaCapaOculta [0, 0]) * pesosCapaOcultaActual2 [0, 0] * errorNeurona1CapaOculta2 +
			                                 DerivadaSigmoidea (entradaNetaCapaOculta [0, 0]) * pesosCapaOcultaActual2 [1, 0] * errorNeurona2CapaOculta2;
			pesosCapaOculta [0, 0] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, 0];
			pesosCapaOculta [0, 1] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, 1];
			pesosCapaOculta [0, 2] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, 2];

			double errorNeurona2CapaOculta = DerivadaSigmoidea (entradaNetaCapaOculta [0, 1]) * pesosCapaOcultaActual2 [0, 1] * errorNeurona1CapaOculta2 +
			                                 DerivadaSigmoidea (entradaNetaCapaOculta [0, 1]) * pesosCapaOcultaActual2 [1, 1] * errorNeurona2CapaOculta2;
			pesosCapaOculta [1, 0] += factorEntrenamiento * errorNeurona2CapaOculta * entradaDeseada [i, 0];
			pesosCapaOculta [1, 1] += factorEntrenamiento * errorNeurona2CapaOculta * entradaDeseada [i, 1];
			pesosCapaOculta [1, 2] += factorEntrenamiento * errorNeurona2CapaOculta * entradaDeseada [i, 2];

			double errorNeurona3CapaOculta = DerivadaSigmoidea (entradaNetaCapaOculta [0, 2]) * pesosCapaOcultaActual2 [0, 2] * errorNeurona1CapaOculta2 +
			                                 DerivadaSigmoidea (entradaNetaCapaOculta [0, 2]) * pesosCapaOcultaActual2 [1, 2] * errorNeurona2CapaOculta2;
			pesosCapaOculta [2, 0] += factorEntrenamiento * errorNeurona3CapaOculta * entradaDeseada [i, 0];
			pesosCapaOculta [2, 1] += factorEntrenamiento * errorNeurona3CapaOculta * entradaDeseada [i, 1];
			pesosCapaOculta [2, 2] += factorEntrenamiento * errorNeurona3CapaOculta * entradaDeseada [i, 2];

			double error = 0;
		}
	}

}
