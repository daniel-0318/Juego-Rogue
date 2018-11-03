using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class SaveLoad {

	//variables
	public List<int> numeroPasosJugador= new List<int>();
	public List<int> vidajugador = new List<int>();
	public int nivel = 0;

	public List<int> score = new List<int> ();
	public List<int> killsEnemies = new List<int> ();
	public List<int> secretosEncontrados = new List<int> ();

	private string pasosDelJugador = "";
	private string vidaDelJugador = "";

	private string puntaje = "";
	private string muertes =  "";
	private string encontroSecretos = "";

	//Matriz por cada tipo Guardado, cada fila es un archivo distinto y cada columna son los datos de cada nivel de un mismo txt
	private List<List<int>> pasosJugadorTxt = new List<List<int>> ();
	private List<List<int>> vidaJugadorTxt = new List<List<int>> ();
	private List<int> jugadorMuertoTxt = new List<int> ();
	private List<List<int>> posicionMuerteJugadorTxt = new List<List<int>> ();
	private List<List<String>> listaEnemigosTxt = new List<List<String>> ();
	private List<List<String>> listaGolpesEnemigosTxt = new List<List<String>> ();
	private List<List<int>> listaPuntajeTxt = new List<List<int>> ();
	private List<List<int>> listaMuertesTxt = new List<List<int>> ();
	private List<List<int>> listaSecretosTxt = new List<List<int>> ();

	private List<List<string>> matrizPorNiveles = new List<List<string>>();
	private List<List<string>> matrizPorNivelesTipo = new List<List<string>>();
	private int nivelMasAltoEnTxt = 0;


	public string getPasosDelJugador(){
		return pasosDelJugador;
	}

	public string getvidaDelJugador(){
		return vidaDelJugador;
	}

	public List<List<string>> getMatrizPorNiveles(){
		return matrizPorNiveles;
	}

	public List<List<string>> getMatrizPorNivelesTipo(){
		return matrizPorNivelesTipo;
	}

	public void MostrarPasos(){
		//Debug.Log ("####################  Pasos jugador ################");
		for(int i=0;i<numeroPasosJugador.Count;i++){
			//Debug.Log ("nivel " + (i+1) + " " + numeroPasosJugador[i]);
			pasosDelJugador = pasosDelJugador + numeroPasosJugador[i];
			if ((i + 1) < numeroPasosJugador.Count) {
				pasosDelJugador += ",";
			}
		}
		//Debug.Log (pasosDelJugador);
	}

	public void MostrarVidaJugador(){
		//Debug.Log ("####################  Vida jugador ################");
		for(int i=0;i<vidajugador.Count;i++){
			//Debug.Log ("nivel " + (i+1) + " " + vidajugador[i]);
			int vida;
			if (i == 0) {
				vida = (vidajugador [i] - 100);

			} else {
				vida = (vidajugador [i] - vidajugador [i - 1]);
			}
			if (vida > 0) {
				vidaDelJugador += vida;
			} else {
				vidaDelJugador += 0;
			}
			if ((i + 1) < vidajugador.Count) {
				vidaDelJugador += ",";
			}
		}
		//Debug.Log (vidaDelJugador);

	}


	public void MostrarPuntajes(){
		for (int i = 0; i < score.Count; i++) {
			if (i == 0) {
				puntaje += score [i];
			} else {
				puntaje += (score [i] - score[i-1]);
			}

			if ((i + 1) < score.Count) {
				puntaje += ",";
			}
		}
	}

	public void MostrarMuertes(){
		for (int i = 0; i < killsEnemies.Count; i++) {
			if (i == 0) {
				muertes += killsEnemies [i];
			} else {
				muertes += (killsEnemies [i] - killsEnemies[i-1]);
			}

			if ((i + 1) < killsEnemies.Count) {
				muertes += ",";
			}
		}
	}

	public void MostrarSecretosEncontrados(){
		for (int i = 0; i < secretosEncontrados.Count; i++) {
			encontroSecretos += secretosEncontrados [i]; // no hay necesidad de restar con el nivel anterior ya que este es como un booleano (encontro o no)

			if ((i + 1) < secretosEncontrados.Count) {
				encontroSecretos += ",";
			}
		}
	}
		
	public void GuardarParaExportar(){
		//Debug.Log ("Entro a crear el archivo de texto");
		StreamWriter texto = new StreamWriter ("Assets/datos.csv");

		resetDatosStringAGuardar (); //para que no se duplique la informacion en el archivo txt

		MostrarPasos ();
		MostrarVidaJugador ();
		MostrarPuntajes ();
		MostrarMuertes ();
		MostrarSecretosEncontrados ();

		texto.WriteLine (nivel);
		texto.WriteLine (pasosDelJugador);
		texto.WriteLine (vidaDelJugador);

		texto.WriteLine (puntaje);
		texto.WriteLine (muertes);
		texto.WriteLine (encontroSecretos);

		texto.Close ();

	}

	public void resetDatosStringAGuardar(){
		pasosDelJugador = "";
		vidaDelJugador = "";
		puntaje = "";
		muertes = "";
		encontroSecretos = "";
	}

	/*Funcion que sirve para leer todos los archivo txt generados hasta el numero antes de numMaximo(porque aun se esta ejecutando la actual partida) que se da.*/
	public void leerArchivosTxt(int numMaximo){
		for (int i = 0; i < numMaximo; i++) {
			if (File.Exists ("Assets/datos" + i + ".csv")) {
				bool leyoPrimeraLinea = false; //Variable para controlar que leyo ya la primera linea y no repetir al tipo de jugada que corresponde
//				pasosJugadorTxt.Add (new List<int>());
//				vidaJugadorTxt.Add (new List<int>());
//
//				posicionMuerteJugadorTxt.Add (new List<int>());
//				listaEnemigosTxt.Add (new List<string>());
//				listaGolpesEnemigosTxt.Add (new List<string>());
//				listaPuntajeTxt.Add(new List<int>());
//				listaMuertesTxt.Add(new List<int>());
//				listaSecretosTxt.Add(new List<int>());

				StreamReader streamreader = new StreamReader ("Assets/datos" + i + ".csv");

				String linea = "";
				bool controlCadaArchivo = true; //sirve para que solo intente agrandar la lista de matrices por nivel solo una vez cada que lee un archivo txt

				for (int j = 0; j < 5; j++) {
					
					linea = streamreader.ReadLine ();
					string[] splitString = linea.Split (new string[] { ";" }, StringSplitOptions.None);
//					int difDeNiveles = Math.Abs (nivelMasAltoEnTxt - splitString.Length);
//
//					if (splitString.Length > nivelMasAltoEnTxt  & controlCadaArchivo) {
//						nivelMasAltoEnTxt = splitString.Length;
//						//Debug.Log ("Entro a cambiar el valor del nivel mas alto con: " + nivelMasAltoEnTxt);
//						controlCadaArchivo = false;
//						for (int k = 0; k < difDeNiveles; k++) {
//							matrizPorNiveles.Add (new List<string> ()); //Se crea una nueva lista para cada nivel.
//							matrizPorNivelesTipo.Add( new List<string>());
//						}
//					}
//
//					//Debug.Log ("El experimento salio para tener tamaño: " + matrizPorNiveles.Count);
//
//
					//IngresarDatosAMatrices (splitString, j, i, leyoPrimeraLinea);
					leyoPrimeraLinea = true;
				}


				streamreader.Close ();

			}
		}
		//revisarMatricestxt ();
	}



}
