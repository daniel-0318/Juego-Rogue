using System.Collections;
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
	public bool jugadorMuerto = false;
	public List<int> posicionMuerteJugador = new List<int> ();
	public List<List<int>> listadoEnemigos = new List<List<int>>(); //cada 4 num son vecesGolpeJugador, IA, Px y posY de cada enemigo en cada nivel (de lista interna)
	public List<List<List<int>>> ListadoGolpesEnemigosNiveles = new List<List<List<int>>> ();
	public List<List<List<int>>> ListadoitemsNiveles = new List<List<List<int>>> ();

	private string pasosDelJugador;
	private string vidaDelJugador;
	private string jugadorSeMurio;
	private string posicionMuerteDelJugador;
	private string listadoDeEnemigos;
	private string listadoDeGolpesDeEnemigosNiveles;
	private string listadoDeItems;


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

	public int MostrarPasos(){
		Debug.Log ("####################  Pasos jugador ################");
		for(int i=0;i<numeroPasosJugador.Count;i++){
			Debug.Log ("nivel " + (i+1) + " " + numeroPasosJugador[i]);
			pasosDelJugador = pasosDelJugador + numeroPasosJugador[i] + ";";
		}
		Debug.Log (pasosDelJugador);
		return 1;
	}

	public int MostrarVidaJugador(){
		Debug.Log ("####################  Vida jugador ################");
		for(int i=0;i<vidajugador.Count;i++){
			Debug.Log ("nivel " + (i+1) + " " + vidajugador[i]);
			vidaDelJugador = vidaDelJugador + vidajugador [i] + ";";
		}
		Debug.Log (vidaDelJugador);
		return 1;
	}

	public int MostrarJugadorMuerto(){
		Debug.Log ("####################  Jugador Muerto ################");
		Debug.Log (jugadorMuerto);
		jugadorSeMurio = "" + jugadorMuerto;
		Debug.Log (jugadorSeMurio);
		return 1;

	}

	public int MostrarPosicionMuerteDelJugador(){
		Debug.Log ("####################  PosicionMuerteJugador ################");
		Debug.Log (posicionMuerteJugador[0] + " " + posicionMuerteJugador[1]);
		posicionMuerteDelJugador = "" + posicionMuerteJugador [0] + ";" + posicionMuerteJugador [1];
		Debug.Log (posicionMuerteDelJugador);

		return 1;
	}

	public int MostrarListadoEnemigos(){
		Debug.Log ("####################  Listado Enemigos ################");
		Debug.Log (listadoEnemigos.Count);
		for(int i=0;i<listadoEnemigos.Count;i++){
			Debug.Log ("nivel " + i + " "+ listadoEnemigos.Count );
			for(int j=0;j<listadoEnemigos[i].Count;j++){
				Debug.Log (listadoEnemigos[i][j]);
			}
		}
		return 1;
	}

	public int MostrarListadoGolpesEnemigosNiveles(){
		Debug.Log ("####################  Listado Golpes Enemigos Niveles ################");
		for(int i=0;i<ListadoGolpesEnemigosNiveles.Count;i++){
			for(int j=0;j<ListadoGolpesEnemigosNiveles[i].Count;j++){
				for(int k=0;k<ListadoGolpesEnemigosNiveles[i][j].Count;k++){
					Debug.Log ("nivel " + (i+1) +" "+ ListadoGolpesEnemigosNiveles[i][j][k]);
				}
			}
		}
		return 1;
	}

	public int MostrarListadoitemsNiveles(){
		Debug.Log ("####################  ListadoitemsNiveles ################");
		for(int i=0;i<ListadoitemsNiveles.Count;i++){
			for(int j=0;j<ListadoitemsNiveles[i].Count;j++){
				for(int k=0;k<ListadoitemsNiveles[i][j].Count;k++){
					Debug.Log ("nivel " + (i+1)+" " + ListadoitemsNiveles[i][j][k]);
				}
			}
		}
		return 1;
	}


	public void GuardarParaExportar(){

		StreamWriter texto = new StreamWriter ("Assets/Prueba.txt");

		texto.WriteLine ("Hola mundo");
		texto.WriteLine ("hello world");

		texto.Close ();

	}

}
