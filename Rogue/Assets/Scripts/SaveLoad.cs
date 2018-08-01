﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

[Serializable]
public class SaveLoad {

	//variables
	public List<int> numeroPasosJugador= new List<int>();
	public List<int> vidajugador = new List<int>();
	public int jugadorMuerto = 0;
	public List<int> posicionMuerteJugador = new List<int> ();
	public List<List<int>> listadoEnemigos = new List<List<int>>(); //cada 4 num son vecesGolpeJugador, IA, Px y posY de cada enemigo en cada nivel (de lista interna)
	public List<List<List<int>>> ListadoGolpesEnemigosNiveles = new List<List<List<int>>> ();
	public List<List<List<int>>> ListadoitemsNiveles = new List<List<List<int>>> ();

	private string pasosDelJugador = "";
	private string vidaDelJugador = "";
	private string jugadorSeMurio = "";
	private string posicionMuerteDelJugador = "";
	private string listadoDeEnemigos = "";
	private string listadoDeGolpesDeEnemigosNiveles = "";
	private string listadoDeItems = "";

	//Matriz por cada tipo Guardado, cada fila es un archivo distinto y cada columna son los datos de cada nivel de un mismo txt
	private List<List<int>> pasosJugadorTxt = new List<List<int>> ();
	private List<List<int>> vidaJugadorTxt = new List<List<int>> ();
	private List<int> jugadorMuertoTxt = new List<int> ();
	private List<List<int>> posicionMuerteJugadorTxt = new List<List<int>> ();
	private List<List<String>> listaEnemigosTxt = new List<List<String>> ();
	private List<List<String>> ListaGolpesEnemigosTxt = new List<List<String>> ();


	public string getPasosDelJugador(){
		return pasosDelJugador;
	}

	public string getvidaDelJugador(){
		return vidaDelJugador;
	}

	public string getJugadorSeMurio(){
		return jugadorSeMurio;
	}

	public string getPosicionMuerteDelJugador(){
		return posicionMuerteDelJugador;
	}

	public string getListadoDeEnemigos(){
		return listadoDeItems;
	}

	public string getListadoDeGolpesDeEnemigosNiveles(){
		return listadoDeGolpesDeEnemigosNiveles;
	}

	public string getListadoDeItems(){
		return listadoDeItems;
	}

	public void MostrarPasos(){
		Debug.Log ("####################  Pasos jugador ################");
		for(int i=0;i<numeroPasosJugador.Count;i++){
			Debug.Log ("nivel " + (i+1) + " " + numeroPasosJugador[i]);
			pasosDelJugador = pasosDelJugador + numeroPasosJugador[i];
			if ((i + 1) < numeroPasosJugador.Count) {
				pasosDelJugador += ";";
			}
		}
		Debug.Log (pasosDelJugador);
	}

	public void MostrarVidaJugador(){
		Debug.Log ("####################  Vida jugador ################");
		for(int i=0;i<vidajugador.Count;i++){
			Debug.Log ("nivel " + (i+1) + " " + vidajugador[i]);
			vidaDelJugador = vidaDelJugador + vidajugador [i];
			if ((i + 1) < vidajugador.Count) {
				vidaDelJugador += ";";
			}
		}
		Debug.Log (vidaDelJugador);

	}

	public void MostrarJugadorMuerto(){
		Debug.Log ("####################  Jugador Muerto ################");
		Debug.Log (jugadorMuerto);
		jugadorSeMurio = "" + jugadorMuerto;
		Debug.Log (jugadorSeMurio);

	}

	public void MostrarPosicionMuerteDelJugador(){
		Debug.Log ("####################  PosicionMuerteJugador ################");
		Debug.Log (posicionMuerteJugador[0] + " " + posicionMuerteJugador[1]);
		posicionMuerteDelJugador = "" + posicionMuerteJugador [0] + ";" + posicionMuerteJugador [1];
		Debug.Log (posicionMuerteDelJugador);

	}

	public void MostrarListadoEnemigos(){
		Debug.Log ("####################  Listado Enemigos ################");
		Debug.Log (listadoEnemigos.Count);
		for(int i=0;i<listadoEnemigos.Count;i++){
			Debug.Log ("nivel " + (i+1));

			if (listadoEnemigos[i].Count==0) {
				listadoDeEnemigos += "#";
			}

			for (int j = 0; j < listadoEnemigos [i].Count; j += 4) {
				Debug.Log (listadoEnemigos [i] [j] + " " + listadoEnemigos [i] [j + 1] + " " + listadoEnemigos [i] [j + 2] + " " + listadoEnemigos [i] [j + 3]);
				listadoDeEnemigos += listadoEnemigos [i] [j] + "," + listadoEnemigos [i] [j + 1] + "," + listadoEnemigos [i] [j + 2] + "," + listadoEnemigos [i] [j + 3];
				if ((j + 4) < listadoEnemigos [i].Count)
					listadoDeEnemigos += ",";
			}
			if ((i + 1) < listadoEnemigos.Count) {
				listadoDeEnemigos += ";";
			}
		}

		Debug.Log (listadoDeEnemigos);
	}

	public void MostrarListadoGolpesEnemigosNiveles(){
		Debug.Log ("####################  Listado Golpes Enemigos Niveles ################");
		for(int i=0;i<ListadoGolpesEnemigosNiveles.Count;i++){
			Debug.Log ("nivel " + (i + 1));

			if(ListadoGolpesEnemigosNiveles [i].Count == 0)
				listadoDeGolpesDeEnemigosNiveles += "#";
			
			for(int j=0;j<ListadoGolpesEnemigosNiveles[i].Count;j++){
				for(int k=0;k<ListadoGolpesEnemigosNiveles[i][j].Count;k++){
					Debug.Log (ListadoGolpesEnemigosNiveles[i][j][k]);
					listadoDeGolpesDeEnemigosNiveles += ListadoGolpesEnemigosNiveles [i] [j] [k] + ",";
				}
				listadoDeGolpesDeEnemigosNiveles += "#";
				Debug.Log ("Fin de un enemigo");
			}
			if ((i + 1) < ListadoGolpesEnemigosNiveles.Count) {
				listadoDeGolpesDeEnemigosNiveles += ";";
			}
		}
	}

	public void MostrarListadoitemsNiveles(){
		Debug.Log ("####################  ListadoitemsNiveles ################");
		for(int i=0;i<ListadoitemsNiveles.Count;i++){
			for(int j=0;j<ListadoitemsNiveles[i].Count;j++){
				for(int k=0;k<ListadoitemsNiveles[i][j].Count;k++){
					Debug.Log ("nivel " + (i+1)+" " + ListadoitemsNiveles[i][j][k]);
				}
			}
		}
	}


	public void GuardarParaExportar(int numeroArchivo ){
		Debug.Log ("Entro a crear el archivo de texto");
		StreamWriter texto = new StreamWriter ("Assets/datos"+numeroArchivo+ ".txt");

		resetDatosStringAGuardar (); //para que no se duplique la informacion en el archivo txt

		MostrarPasos ();
		MostrarVidaJugador ();
		MostrarJugadorMuerto ();
		MostrarPosicionMuerteDelJugador ();
		MostrarListadoEnemigos ();
		MostrarListadoGolpesEnemigosNiveles ();
		MostrarListadoitemsNiveles ();

		texto.WriteLine (pasosDelJugador);
		texto.WriteLine (vidaDelJugador);
		texto.WriteLine (jugadorSeMurio);
		texto.WriteLine (posicionMuerteDelJugador);
		texto.WriteLine (listadoDeEnemigos);
		texto.WriteLine (listadoDeGolpesDeEnemigosNiveles);

		texto.Close ();

	}

	public void resetDatosStringAGuardar(){
		pasosDelJugador = "";
		vidaDelJugador = "";
		jugadorSeMurio = "";
		posicionMuerteDelJugador = "";
		listadoDeEnemigos = "";
		listadoDeGolpesDeEnemigosNiveles = "";
	}

	/*Funcion que sirve para leer todos los archivo txt generados hasta el numero antes de numMaximo(porque aun se esta ejecutando la actual partida) que se da.*/
	public void leerArchivosTxt(int numMaximo){
		for (int i = 0; i < numMaximo; i++) {
			if (File.Exists ("Assets/datos" + i + ".txt")) {
				pasosJugadorTxt.Add (new List<int>());
				vidaJugadorTxt.Add (new List<int>());

				posicionMuerteJugadorTxt.Add (new List<int>());
				listaEnemigosTxt.Add (new List<string>());
				ListaGolpesEnemigosTxt.Add (new List<string>());

				StreamReader streamreader = new StreamReader ("Assets/datos" + i + ".txt");

				String linea = "";

				for (int j = 0; j < 6; j++) {
					
					linea = streamreader.ReadLine ();
					IngresarDatosAMatrices (linea.Split (new string[] { ";" }, StringSplitOptions.None), j );
				}

				streamreader.Close ();

			}
		}
	}

	public void IngresarDatosAMatrices(string[] splitString, int matrizElegida){
		for (int i = 0; i < splitString.Length; i++) {

			if (matrizElegida == 0) {
				Debug.Log ("Pasos jugador" + splitString[i]);
				pasosJugadorTxt [pasosJugadorTxt.Count - 1].Add (int.Parse (splitString [i]));
			} else if (matrizElegida == 1) {
				Debug.Log ("vida jugador" + splitString[i]);
				vidaJugadorTxt [vidaJugadorTxt.Count - 1].Add (int.Parse (splitString [i]));
			}else if (matrizElegida == 2) {
				Debug.Log ("jugador muerto" + splitString[i]);
				jugadorMuertoTxt.Add (int.Parse (splitString [i]));
			}else if (matrizElegida == 3) {
				Debug.Log ("posicion muerte jugador" + splitString[i]);
				posicionMuerteJugadorTxt [vidaJugadorTxt.Count - 1].Add (int.Parse (splitString [i]));
			}else if (matrizElegida == 4) {
				Debug.Log ("Lista enemigos" + splitString[i]);
				listaEnemigosTxt [listaEnemigosTxt.Count - 1].Add (splitString [i]);
			}else if (matrizElegida == 5) {
				Debug.Log ("Lista golpes enemigo" + splitString[i]);
				ListaGolpesEnemigosTxt [ListaGolpesEnemigosTxt.Count - 1].Add (splitString [i]);
			}

		}
	}

}
