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

//	private double[,] entradaNetaCapaOculta;
//	private double[,] salidaCapaOculta;
//	private double[,] entradaNetaCapaOculta2;
//	private double[,] salidaCapaOculta2;
//	private double[,] entradaNetaCapaOculta3;
//	private double[,] salidaCapaOculta3;
//	private double[,] entradaNetaCapaSalida;

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

	public double[,] Red_neuronal(double[,] entrada){

		//Capa de entrada
		//No se realiza ningun procesamiento
		//*************************************
		//Capa oculta
		double tendenciaCapa1 = 0;
		double[,] entradaNetaCapaOculta = MultiMatrices(pesosCapaOculta, Transpuesta(entrada));
		entradaNetaCapaOculta = Transpuesta (entradaNetaCapaOculta);
		//Debug.Log ("Entrada neta 1: " + entradaNetaCapaOculta.GetLength(0) + " " + entradaNetaCapaOculta.GetLength(1));
		//Debug.Log ("Entrada neta 1: " + entradaNetaCapaOculta [0, 0] + " " + entradaNetaCapaOculta [0, 1] + " " + entradaNetaCapaOculta [0, 2]);
		double[,] salidaCapaOculta = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaOculta);

		//Capa oculta2
		double tendenciaCapa2 = 0;
		double[,] entradaNetaCapaOculta2 = MultiMatrices(pesosCapaOculta2, Transpuesta(salidaCapaOculta));
		entradaNetaCapaOculta2 = Transpuesta (entradaNetaCapaOculta2);
		//Debug.Log ("Entrada neta 2: " + entradaNetaCapaOculta2.GetLength(0) + " " + entradaNetaCapaOculta2.GetLength(1));
		//Debug.Log ("Entrada neta 2: " + entradaNetaCapaOculta2 [0, 0] + " " + entradaNetaCapaOculta2 [0, 1] );
		double[,] salidaCapaOculta2 = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaOculta2);

		//Capa oculta3
		double tendenciaCapa3 = 0;
		double[,] entradaNetaCapaOculta3 = MultiMatrices(pesosCapaOculta3, Transpuesta(salidaCapaOculta2));
		entradaNetaCapaOculta3 = Transpuesta (entradaNetaCapaOculta3);

		double[,] salidaCapaOculta3 = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaOculta3);


		//Capa de salida
		double tendenciaCapa4 = 0;
		double[,] entradaNetaCapaSalida = MultiMatrices(pesosCapaSalida, Transpuesta(salidaCapaOculta3));
		entradaNetaCapaSalida = Transpuesta (entradaNetaCapaSalida);
		ImprimirMatriz (entradaNetaCapaSalida);
		double[,] salida = Aplicar_funcion_activacion_a_matriz (entradaNetaCapaSalida);


		return salida;
	}


	public void invocar_algoritmo_entrenamiento(){
		//Backpropagation (entradas, salidas);
		Debug.Log ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Cargo pesos ~~~~~~~~~~~~~~~~+");
		Cargar_pesos();
		Debug.Log ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Cargo datos ~~~~~~~~~~~~~~~~+");
		Cargar_datos ();

		for (int i = 0; i < 100; i++) {
			Debug.Log ("=========================================");
			Debug.Log ("Se desea: " + salidas [0,i]);
			double[,] salida = Red_neuronal (Sacar_fila_de_matriz (i, entradas));
			Debug.Log ("La salida tiene" + salida.Length);
			Debug.Log ("Se obtuvo: " + salida [0, 0] + " " + salida[0,1] + " " + salida[0,2]);
		//	Debug.Log ("Un error de : " + (salida [0, 0] - salidas [i, 0]) + " y " + (salida [0, 1] - salidas [i, 1]));
		}
		
	}

	public void Cargar_pesos(){
		if (File.Exists ("Assets/pesos.csv")) {
			pesosCapaOculta = new double[10,5];
			pesosCapaOculta2 = new double[8,10];
			pesosCapaOculta3 = new double[8,8];
			pesosCapaSalida = new double[3,8];

			StreamReader streamreader = new StreamReader ("Assets/pesos.csv");


			String linea = "";

			for (int i = 0; i < 29; i++) {

				linea = streamreader.ReadLine();
				string[] splitString = linea.Split (new string[] { ";" }, StringSplitOptions.None);
				for (int j = 0; j < splitString.Length; j++) {
					if (i < 10) {
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


	public void Cargar_datos(){
		if (File.Exists ("Assets/datos0.csv")) {
			StreamReader streamreader = new StreamReader ("Assets/datos0.csv");


			String linea = "";

			for (int i = 0; i < 6; i++) {

				linea = streamreader.ReadLine ();
				//Debug.Log ("Se intentara cortar: " + linea);
				string[] splitString = linea.Split (new string[] { ";" }, StringSplitOptions.None);
				//Debug.Log ("primer dato: " + splitString [0]);
				entradas = new double[splitString.Length, 5];
				salidas = new double[1,splitString.Length];
				for (int j = 0; j < splitString.Length; j++) {
					if (i < 5) {
						entradas [j, i] =  double.Parse (splitString [j]);
					} else {
						salidas [0, j] =  double.Parse (splitString [j]);
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
