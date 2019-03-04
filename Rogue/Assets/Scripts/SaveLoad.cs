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

	private int nivelABorrar=6;


	public string getPasosDelJugador(){
		return pasosDelJugador;
	}

	public string getvidaDelJugador(){
		return vidaDelJugador;
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
		
	public void GuardarParaExportar(int level){
		//Debug.Log ("Entro a crear el archivo de texto");
		StreamWriter texto = new StreamWriter ("Assets/datos.csv");

		resetDatosStringAGuardar (level); //para que no se duplique la informacion en el archivo txt

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

	public void resetDatosStringAGuardar(int level){
		pasosDelJugador = "";
		vidaDelJugador = "";
		puntaje = "";
		muertes = "";
		encontroSecretos = "";
	}

	public void resetListas(int level){
		if (level == nivelABorrar) {
			numeroPasosJugador = new List<int>();
			vidajugador = new List<int> ();
			score = new List<int> ();
			killsEnemies = new List<int> ();
			secretosEncontrados = new List<int> ();
			nivelABorrar += 2;
		}
	}

	/*Funcion que sirve para leer todos los archivo txt generados hasta el numero antes de numMaximo(porque aun se esta ejecutando la actual partida) que se da.*/
	public int leerArchivosCsv(){
		int t0 = 0;
		int t1 = 0;
		int t2 = 0;

		if (File.Exists ("Assets/tipos.csv")) {

			StreamReader streamreader = new StreamReader ("Assets/tipos.csv");

			String linea = streamreader.ReadLine ();
			while(linea != null){
//				Debug.Log ("Encontro" + linea + "Fin");
//				Debug.Log ("Pruebas " + (linea== " ") + (linea==null));
				int tp = int.Parse (linea);
				if (tp == 0) {
					t0++;
				} else if (tp == 1) {
					t1++;
				} else {
					t2++;
				}
				linea = streamreader.ReadLine ();
			}

			streamreader.Close ();
		}
		File.Delete ("Assets/tipos.csv");
		if ((t0 > t1)&& (t0 > t2)) {
			return 0;
		} else if ((t1 > t0) && (t1 > t2)) {
			return 1;
		} else {
			return 2;
		}
	}




}
