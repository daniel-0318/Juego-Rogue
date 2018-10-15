using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class RedNeuronal {


	private double factorEntrenamiento = 0.3;
	private double errorPermitido = 0.4;
	private double p = 1.1;
	private double a = 0.5;

	private List<List<string>> matrizPorNiveles = new List<List<string>>();// El que le pasan antes de convertir los valores a binarios
	private List<List<string>> matrizPorNivelesTipo = new List<List<string>>();// El que le pasan antes de convertir los valores a binarios

	private double[,] entradas;
	private double[,] salidas;

	private double[,] entradaNetaCapaOculta;
	private double[,] salidaCapaOculta;
	private double[,] entradaNetaCapaOculta2;
	private double[,] salidaCapaOculta2;
	private double[,] entradaNetaCapaOculta3;
	private double[,] salidaCapaOculta3;
	private double[,] entradaNetaCapaSalida;

	double[,] pesosCapaOculta;
	double[,] pesosCapaOculta2;
	double[,] pesosCapaOculta3;
	double[,] pesosCapaSalida;

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

	public void Copiar_datos_salidas(List<List<string>> salida){
		//Debug.Log ("Prueba: " + salida.Count + " y " + salida [0].Count);
		for (int i = 0; i < salida.Count; i++) {
			matrizPorNivelesTipo.Add(new List<string>());
			for (int j = 0; j < salida[i].Count; j++) {
				matrizPorNivelesTipo [i].Add (salida [i] [j]);
			}
		}
		//Debug.Log ("matrizPorNivelesTipo: " + matrizPorNivelesTipo.Count + " y " + matrizPorNivelesTipo [0].Count);
	}

	public void Copiar_datos_entrada(List<List<string>> entrada){
		//Debug.Log ("Prueba: " + entrada.Count + " y " + entrada [0].Count);

		for (int i = 0; i < entrada.Count; i++) { // por prueba solo  copiare los que voy a usar.
				matrizPorNiveles.Add (new List<string> ());
				for (int j = 0; j < entrada [i].Count; j++) {
					matrizPorNiveles [matrizPorNiveles.Count-1].Add (entrada [i] [j]);
				}
		}
		//Debug.Log ("matrizPorNiveles: " + matrizPorNiveles.Count + " y " + matrizPorNiveles [0].Count);
			

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
		//Debug.Log ("Entrada neta 1: " + entradaNetaCapaOculta.GetLength(0) + " " + entradaNetaCapaOculta.GetLength(1));
		//Debug.Log ("Entrada neta 1: " + entradaNetaCapaOculta [0, 0] + " " + entradaNetaCapaOculta [0, 1] + " " + entradaNetaCapaOculta [0, 2]);
		salidaCapaOculta = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaOculta);

		//Capa oculta2
		double tendenciaCapa2 = 1;
		entradaNetaCapaOculta2 = MultiMatrices(pesosCapa2, Transpuesta(salidaCapaOculta));
		entradaNetaCapaOculta2 = Transpuesta (Suma_Escalar_a_Matriz (tendenciaCapa2, entradaNetaCapaOculta2));
		//Debug.Log ("Entrada neta 2: " + entradaNetaCapaOculta2.GetLength(0) + " " + entradaNetaCapaOculta2.GetLength(1));
		//Debug.Log ("Entrada neta 2: " + entradaNetaCapaOculta2 [0, 0] + " " + entradaNetaCapaOculta2 [0, 1] );
		salidaCapaOculta2 = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaOculta2);

		//Capa de salida
		double tendenciaCapa3 = 1;
		entradaNetaCapaSalida = MultiMatrices(pesosCapaSalida, Transpuesta(salidaCapaOculta2));
		entradaNetaCapaSalida = Transpuesta (Suma_Escalar_a_Matriz (tendenciaCapa3, entradaNetaCapaSalida));
		double[,] salida = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaSalida);


		return salida;  /////////// En si no es necesario devolverlo porque es una variable de la clase mirar como seria mejor /////////////////////////
	}


	public void Backpropagation(double[,] entradaDeseada, double[,] salidaDeseada){

		//Pesos Aleatorios
		pesosCapaOculta = matrizRadom (10, 5);
		pesosCapaOculta2 = matrizRadom (8, 10);
		pesosCapaOculta3 = matrizRadom (8, 8);
		pesosCapaSalida = matrizRadom (3, 8);
		ImprimirMatriz (pesosCapaOculta);
		ImprimirMatriz (pesosCapaOculta2);
		ImprimirMatriz (pesosCapaSalida);
		double error_anterior = 0;

		////// FALTARIA QUE SE COGAN PATRONES ALEATORIOS PARA ENTRENAMIENTO Y PARA PRUEBAS

		int iteraciones = 0;
		double error = 0;

		for (int j = 0; j < entradaDeseada.GetLength (0); j++) {
			double [,] salida_prueba = Red_neuronal (Sacar_fila_de_matriz (j, entradaDeseada), pesosCapaOculta, pesosCapaOculta2, pesosCapaSalida);
			error_anterior += 0.5 * (Mathf.Pow ((float)(salidaDeseada [j, 0] - salida_prueba [0, 0]), 2f) +
			Mathf.Pow ((float)(salidaDeseada [j, 1] - salida_prueba [0, 1]), 2f));
		}
		for (int d = 0; d < 100; d++) {

			for (int i = 0; i < entradaDeseada.GetLength (0); i++) {
				iteraciones++;
				double[,] pesosCapaOcultaActual = Copiar_Matriz (pesosCapaOculta);
				double[,] pesosCapaOcultaActual2 = Copiar_Matriz (pesosCapaOculta2);
				double[,] pesosCapaSalidaActual = Copiar_Matriz (pesosCapaSalida);


				double[,] salida = Red_neuronal (Sacar_fila_de_matriz (i, entradaDeseada), pesosCapaOcultaActual, pesosCapaOcultaActual2, pesosCapaSalidaActual);
				/*
				Debug.Log ("-----------------------------------------------");

				Debug.Log ("la salida 1 fue: " + salida [0, 0] + " y " + salida [0, 1]);
				Debug.Log ("la salida esperada era: " + salidaDeseada [i, 0] + " y " + salidaDeseada [i, 1]);*/


				//Miramos la neurona de la capa de salida para ir corrigiendo pesos.

				double errorCapaDeSalida = (salidaDeseada [i, 0] - salida [0, 0]) * DerivadaSigmoidea (entradaNetaCapaSalida [0, 0]);

				//Actualizamos pesos de la capa de salida
				pesosCapaSalida [0, 0] += factorEntrenamiento * errorCapaDeSalida * salidaCapaOculta2 [0, 0];
				pesosCapaSalida [0, 1] += factorEntrenamiento * errorCapaDeSalida * salidaCapaOculta2 [0, 1];


				//Neurona 2 capa de salida.

				double errorCapaDeSalida2 = (salidaDeseada [i, 1] - salida [0, 1]) * DerivadaSigmoidea (entradaNetaCapaSalida [0, 1]);
				pesosCapaSalida [1, 0] += factorEntrenamiento * errorCapaDeSalida * salidaCapaOculta2 [0, 0];
				pesosCapaSalida [1, 1] += factorEntrenamiento * errorCapaDeSalida * salidaCapaOculta2 [0, 1];



				//Dos neuronas en la segunda capa oculta, entonces actualizamos pesos
				//El error total de la capa oculta, es el error de la cada de salida
				//propagamos hacia atras el error de saliida utilizando el peso
				double errorNeurona1CapaOculta2 = DerivadaSigmoidea (entradaNetaCapaOculta2 [0, 0]) * pesosCapaSalidaActual [0, 0] * errorCapaDeSalida +
				                                 DerivadaSigmoidea (entradaNetaCapaOculta2 [0, 0]) * pesosCapaSalidaActual [1, 0] * errorCapaDeSalida2;
				pesosCapaOculta2 [0, 0] += factorEntrenamiento * errorNeurona1CapaOculta2 * salidaCapaOculta [0, 0];
				pesosCapaOculta2 [0, 1] += factorEntrenamiento * errorNeurona1CapaOculta2 * salidaCapaOculta [0, 1];
				pesosCapaOculta2 [0, 2] += factorEntrenamiento * errorNeurona1CapaOculta2 * salidaCapaOculta [0, 2];

				double errorNeurona2CapaOculta2 = DerivadaSigmoidea (entradaNetaCapaOculta2 [0, 1]) * pesosCapaSalidaActual [0, 1] * errorCapaDeSalida +
				                                 DerivadaSigmoidea (entradaNetaCapaOculta2 [0, 1]) * pesosCapaSalidaActual [1, 1] * errorCapaDeSalida2;
				pesosCapaOculta2 [1, 0] += factorEntrenamiento * errorNeurona2CapaOculta2 * salidaCapaOculta [0, 0];
				pesosCapaOculta2 [1, 1] += factorEntrenamiento * errorNeurona2CapaOculta2 * salidaCapaOculta [0, 1];
				pesosCapaOculta2 [1, 2] += factorEntrenamiento * errorNeurona2CapaOculta2 * salidaCapaOculta [0, 2];


				//Tres neuronas en la primera capa oculta, entonces actualizamos pesos
				//El error total de la capa oculta, es el error de la capa oculta2
				//propagamos hacia atras el error de saliida utilizando el peso

				double errorNeurona1CapaOculta = DerivadaSigmoidea (entradaNetaCapaOculta [0, 0]) * pesosCapaOcultaActual2 [0, 0] * errorNeurona1CapaOculta2 +
				                                DerivadaSigmoidea (entradaNetaCapaOculta [0, 0]) * pesosCapaOcultaActual2 [1, 0] * errorNeurona2CapaOculta2;
				/*pesosCapaOculta [0, 0] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, 0];
			pesosCapaOculta [0, 1] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, 1];
			pesosCapaOculta [0, 2] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, 2];*/

				for (int j = 0; j < pesosCapaOculta.GetLength (1); j++) {
					pesosCapaOculta [0, j] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, j];
				}

				double errorNeurona2CapaOculta = DerivadaSigmoidea (entradaNetaCapaOculta [0, 1]) * pesosCapaOcultaActual2 [0, 1] * errorNeurona1CapaOculta2 +
				                                DerivadaSigmoidea (entradaNetaCapaOculta [0, 1]) * pesosCapaOcultaActual2 [1, 1] * errorNeurona2CapaOculta2;
				/*pesosCapaOculta [1, 0] += factorEntrenamiento * errorNeurona2CapaOculta * entradaDeseada [i, 0];
			pesosCapaOculta [1, 1] += factorEntrenamiento * errorNeurona2CapaOculta * entradaDeseada [i, 1];
			pesosCapaOculta [1, 2] += factorEntrenamiento * errorNeurona2CapaOculta * entradaDeseada [i, 2];*/

				for (int j = 0; j < pesosCapaOculta.GetLength (1); j++) {
					pesosCapaOculta [1, j] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, j];
				}

				double errorNeurona3CapaOculta = DerivadaSigmoidea (entradaNetaCapaOculta [0, 2]) * pesosCapaOcultaActual2 [0, 2] * errorNeurona1CapaOculta2 +
				                                DerivadaSigmoidea (entradaNetaCapaOculta [0, 2]) * pesosCapaOcultaActual2 [1, 2] * errorNeurona2CapaOculta2;
				/*pesosCapaOculta [2, 0] += factorEntrenamiento * errorNeurona3CapaOculta * entradaDeseada [i, 0];
			pesosCapaOculta [2, 1] += factorEntrenamiento * errorNeurona3CapaOculta * entradaDeseada [i, 1];
			pesosCapaOculta [2, 2] += factorEntrenamiento * errorNeurona3CapaOculta * entradaDeseada [i, 2];*/

				for (int j = 0; j < pesosCapaOculta.GetLength (1); j++) {
					pesosCapaOculta [2, j] += factorEntrenamiento * errorNeurona1CapaOculta * entradaDeseada [i, j];
				}

				error = 0;
				for (int j = 0; j < entradaDeseada.GetLength (0); j++) {
					double[,] salida_prueba = Red_neuronal (Sacar_fila_de_matriz (j, entradaDeseada), pesosCapaOculta, pesosCapaOculta2, pesosCapaSalida);
					error += 0.5 * (Mathf.Pow ((float)(salidaDeseada [j, 0] - salida_prueba [0, 0]), 2f) + Mathf.Pow ((float)(salidaDeseada [j, 1] - salida_prueba [0, 1]), 2f));
				}

				Debug.Log ("Error global acumulado: " + error);

				//Ajuste de entrenamiento
				if (error < error_anterior)
					factorEntrenamiento = p * factorEntrenamiento;
				else {
					factorEntrenamiento = a * factorEntrenamiento;
				}
				//Guardamos el error actual como el anterior para la sgte iteracion
				error_anterior = error;

				//Condicion de salida del ajuste de pesos
				if (error < errorPermitido) {
					break;
					Debug.Log ("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
				}
			}
		}

		Debug.Log ("Se necesito iterar el for: " + iteraciones + " veces");
		Debug.Log ("Error global es: " + error);

		ImprimirMatriz (pesosCapaOculta);
		ImprimirMatriz (pesosCapaOculta2);
		ImprimirMatriz (pesosCapaSalida);
	}


	public void invocar_algoritmo_entrenamiento(){
		//Backpropagation (entradas, salidas);
		Cargar_pesos();
		Debug.Log ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Fin de entrenamiento ~~~~~~~~~~~~~~~~+");


		for (int i = 0; i < 24; i++) {
			Debug.Log ("=========================================");
			Debug.Log ("Se desea: " + salidas [i, 0] + " " + salidas[i,1]);
			double[,] salida = Red_neuronal (Sacar_fila_de_matriz (i, entradas), pesosCapaOculta, pesosCapaOculta2, pesosCapaSalida);
			Debug.Log ("Se obtuvo: " + salida [0, 0] + " " + salida[0,1]);
			Debug.Log ("Un error de : " + (salida [0, 0] - salidas [i, 0]) + " y " + (salida [0, 1] - salidas [i, 1]));
		}
		
	}

	public void Cargar_pesos(){
		if (File.Exists ("Assets/pesos.csv")) {
			pesosCapaOculta = new double[10,5];
			pesosCapaOculta2 = new double[8,10];
			pesosCapaOculta3 = new double[8,8];
			pesosCapaSalida = new double[3,8];

			StreamReader streamreader = new StreamReader ("Assets/pesos.csv");

			////
			string prueba = "-3.68";
			double p2 = double.Parse (prueba);
			Debug.Log ("prueba: " + p2);
			/// 

			String linea = "";

			for (int i = 0; i < 29; i++) {

				linea = streamreader.ReadLine();
				Debug.Log ("Se intentara cortar: " + linea);
				string[] splitString = linea.Split (new string[] { ";" }, StringSplitOptions.None);
				Debug.Log ("primer dato: " + splitString[0]);
				for (int j = 0; j < splitString.Length; j++) {
					if (i < 10) {
						Debug.Log ("numero: " + double.Parse (splitString [j]));
						pesosCapaOculta [i, j] = double.Parse (splitString [j]);
					} else if (i < 18) {
						pesosCapaOculta2 [i - 10, j] = double.Parse (splitString [j]);
					} else if (i < 26) {
						pesosCapaOculta3 [i - 18, j] = double.Parse (splitString [j]);
					} else {
						pesosCapaSalida [i - 26, j] = double.Parse (splitString [j]);
					}
				}
			}
		}
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

	public void Matriz_decimal_a_binaria(){
		int tamaño = matrizPorNiveles.Count * (matrizPorNiveles [0].Count / 5);
		entradas = new double[tamaño, 25];
		int posicion = 0; // es la posicion donde debe ir cada nivel en la lista binarizada.
		for (int i = 0; i < matrizPorNiveles.Count; i++) {
			for (int j = 0; j < matrizPorNiveles[i].Count; j+=5) {
				/*
				Debug.Log ("primer dato: " + matrizPorNiveles [i] [j]);
				Debug.Log ("second dato: " + matrizPorNiveles [i] [j+1]);
				Debug.Log ("tercero dato: " + matrizPorNiveles [i] [j+2]);
				Debug.Log ("cuatro dato: " + matrizPorNiveles [i] [j+3]);
				Debug.Log ("quinto dato: " + matrizPorNiveles [i] [j+4]);*/
				string dato1 = Decimal_a_binario (int.Parse(matrizPorNiveles [i] [j]), 9, false);
				string dato2 = Decimal_a_binario (int.Parse(matrizPorNiveles [i] [j+1]), 9, false);
				string dato3 = Decimal_a_binario (int.Parse(matrizPorNiveles [i] [j+2]), 3, false);
				string dato4 = Decimal_a_binario (int.Parse(matrizPorNiveles [i] [j+3]), 3, false);
				string dato5 = Decimal_a_binario (int.Parse(matrizPorNiveles [i] [j+4]), 1, false);

				string[] splitDato1 = dato1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				string[] splitDato2 = dato2.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				string[] splitDato3 = dato3.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				string[] splitDato4 = dato4.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				string[] splitDato5 = dato5.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

				Ingresar_matriz_entrada (splitDato1, 0, posicion);
				Ingresar_matriz_entrada (splitDato2, 1, posicion);
				Ingresar_matriz_entrada (splitDato3, 2, posicion);
				Ingresar_matriz_entrada (splitDato4, 3, posicion);
				Ingresar_matriz_entrada (splitDato5, 4, posicion);
				posicion++;

			}
		}

		int posicionEnSalida = 0;
		int tamañoSalidas = matrizPorNivelesTipo.Count * matrizPorNivelesTipo [0].Count;
		salidas = new double[tamañoSalidas,2];
		for (int i = 0; i < matrizPorNivelesTipo.Count; i++) {
			for (int j = 0; j < matrizPorNivelesTipo [i].Count; j++) {
				string dato = Decimal_a_binario(int.Parse(matrizPorNivelesTipo [i] [j]),2, true);
				string[] splitdato = dato.Split (new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				salidas [posicionEnSalida, 0] = int.Parse (splitdato [0]);
				salidas [posicionEnSalida, 1] = int.Parse(splitdato [1]);
				posicionEnSalida++;
			}
		}
		Debug.Log ("Tamaño de entradas: " + entradas.GetLength (0) + " " + entradas.GetLength (1));
		Debug.Log ("Tamaño de salidas: " + salidas.GetLength(0) + " " + salidas.GetLength(1));
	}

	public void Ingresar_matriz_entrada(String[] matriz, int tipo, int posicion){

		if(tipo ==0){
			for (int i = 0; i < 9; i++) {
				entradas [posicion,i] = int.Parse(matriz [i]);
			}
		}else if(tipo ==1){
			int j = 0;
			for (int i = 9; i < 18; i++) {
				entradas [posicion,i] = int.Parse(matriz [j]);
				j++;
			}
		}else if(tipo ==2){
			int j = 0;
			for (int i = 18; i < 21; i++) {
				entradas [posicion,i] = int.Parse(matriz [j]);
				j++;
			}
		}else if(tipo ==3){
			int j = 0;
			for (int i = 21; i < 24; i++) {
				entradas [posicion,i] = int.Parse(matriz [j]);
				j++;
			}
		}else if(tipo==4)
			entradas [posicion,24] = int.Parse(matriz [0]);

	}

}
