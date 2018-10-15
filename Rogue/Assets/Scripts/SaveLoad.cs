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
	public int jugadorMuerto = 0;
	public List<int> posicionMuerteJugador = new List<int> ();
	public List<List<int>> listadoEnemigos = new List<List<int>>(); //cada 4 num son vecesGolpeJugador, IA, Px y posY de cada enemigo en cada nivel (de lista interna)
	public List<List<List<int>>> ListadoGolpesEnemigosNiveles = new List<List<List<int>>> ();
	public List<List<List<int>>> ListadoitemsNiveles = new List<List<List<int>>> ();
	public List<int> score = new List<int> ();
	public List<int> killsEnemies = new List<int> ();
	public List<int> secretosEncontrados = new List<int> ();

	private string pasosDelJugador = "";
	private string vidaDelJugador = "";
	private string jugadorSeMurio = "";
	private string posicionMuerteDelJugador = "";
	private string listadoDeEnemigos = "";
	private string listadoDeGolpesDeEnemigosNiveles = "";
	private string listadoDeItems = "";
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

	public void MostrarJugadorMuerto(){
		//Debug.Log ("####################  Jugador Muerto ################");
		//Debug.Log (jugadorMuerto);
		jugadorSeMurio = "" + jugadorMuerto;
		//Debug.Log (jugadorSeMurio);

	}

	public void MostrarPosicionMuerteDelJugador(){
		//Debug.Log ("####################  PosicionMuerteJugador ################");
		//Debug.Log (posicionMuerteJugador[0] + " " + posicionMuerteJugador[1]);
		posicionMuerteDelJugador = "" + posicionMuerteJugador [0] + ";" + posicionMuerteJugador [1];
		//Debug.Log (posicionMuerteDelJugador);

	}

	public void MostrarListadoEnemigos(){
		//Debug.Log ("####################  Listado Enemigos ################");
		//Debug.Log (listadoEnemigos.Count);
		for(int i=0;i<listadoEnemigos.Count;i++){
			//Debug.Log ("nivel " + (i+1));

			if (listadoEnemigos[i].Count==0) {
				listadoDeEnemigos += "#";
			}

			for (int j = 0; j < listadoEnemigos [i].Count; j += 4) {
				//Debug.Log (listadoEnemigos [i] [j] + " " + listadoEnemigos [i] [j + 1] + " " + listadoEnemigos [i] [j + 2] + " " + listadoEnemigos [i] [j + 3]);
				listadoDeEnemigos += listadoEnemigos [i] [j] + "," + listadoEnemigos [i] [j + 1] + "," + listadoEnemigos [i] [j + 2] + "," + listadoEnemigos [i] [j + 3];
				if ((j + 4) < listadoEnemigos [i].Count)
					listadoDeEnemigos += ",";
			}
			if ((i + 1) < listadoEnemigos.Count) {
				listadoDeEnemigos += ";";
			}
		}

		//Debug.Log (listadoDeEnemigos);
	}

	public void MostrarListadoGolpesEnemigosNiveles(){
		//Debug.Log ("####################  Listado Golpes Enemigos Niveles ################");
		for(int i=0;i<ListadoGolpesEnemigosNiveles.Count;i++){
			//Debug.Log ("nivel " + (i + 1));

			if(ListadoGolpesEnemigosNiveles [i].Count == 0)
				listadoDeGolpesDeEnemigosNiveles += "#";
			
			for(int j=0;j<ListadoGolpesEnemigosNiveles[i].Count;j++){
				for(int k=0;k<ListadoGolpesEnemigosNiveles[i][j].Count;k++){
					//Debug.Log (ListadoGolpesEnemigosNiveles[i][j][k]);
					listadoDeGolpesDeEnemigosNiveles += ListadoGolpesEnemigosNiveles [i] [j] [k] + ",";
				}
				listadoDeGolpesDeEnemigosNiveles += "#";
				//Debug.Log ("Fin de un enemigo");
			}
			if ((i + 1) < ListadoGolpesEnemigosNiveles.Count) {
				listadoDeGolpesDeEnemigosNiveles += ";";
			}
		}
	}

	public void MostrarListadoitemsNiveles(){
		//Debug.Log ("####################  ListadoitemsNiveles ################");
		for(int i=0;i<ListadoitemsNiveles.Count;i++){
			for(int j=0;j<ListadoitemsNiveles[i].Count;j++){
				for(int k=0;k<ListadoitemsNiveles[i][j].Count;k++){
					//Debug.Log ("nivel " + (i+1)+" " + ListadoitemsNiveles[i][j][k]);
				}
			}
		}
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
		
	public void GuardarParaExportar(int numeroArchivo ){
		//Debug.Log ("Entro a crear el archivo de texto");
		StreamWriter texto = new StreamWriter ("Assets/datos"+numeroArchivo+ ".txt");

		resetDatosStringAGuardar (); //para que no se duplique la informacion en el archivo txt

		MostrarPasos ();
		MostrarVidaJugador ();
		//MostrarJugadorMuerto ();
		//MostrarPosicionMuerteDelJugador ();
		//MostrarListadoEnemigos ();
		//MostrarListadoGolpesEnemigosNiveles ();
		//MostrarListadoitemsNiveles ();
		MostrarPuntajes ();
		MostrarMuertes ();
		MostrarSecretosEncontrados ();

		texto.WriteLine (pasosDelJugador);
		texto.WriteLine (vidaDelJugador);
		//texto.WriteLine (jugadorSeMurio);
		//texto.WriteLine (posicionMuerteDelJugador);
		//texto.WriteLine (listadoDeEnemigos);
		//texto.WriteLine (listadoDeGolpesDeEnemigosNiveles);
		texto.WriteLine (puntaje);
		texto.WriteLine (muertes);
		texto.WriteLine (encontroSecretos);

		texto.Close ();

	}

	public void resetDatosStringAGuardar(){
		pasosDelJugador = "";
		vidaDelJugador = "";
		jugadorSeMurio = "";
		posicionMuerteDelJugador = "";
		listadoDeEnemigos = "";
		listadoDeGolpesDeEnemigosNiveles = "";
		puntaje = "";
		muertes = "";
		encontroSecretos = "";
	}

	/*Funcion que sirve para leer todos los archivo txt generados hasta el numero antes de numMaximo(porque aun se esta ejecutando la actual partida) que se da.*/
	public void leerArchivosTxt(int numMaximo){
		for (int i = 0; i < numMaximo; i++) {
			if (File.Exists ("Assets/datos" + i + ".txt")) {
				bool leyoPrimeraLinea = false; //Variable para controlar que leyo ya la primera linea y no repetir al tipo de jugada que corresponde
				pasosJugadorTxt.Add (new List<int>());
				vidaJugadorTxt.Add (new List<int>());

				posicionMuerteJugadorTxt.Add (new List<int>());
				listaEnemigosTxt.Add (new List<string>());
				listaGolpesEnemigosTxt.Add (new List<string>());
				listaPuntajeTxt.Add(new List<int>());
				listaMuertesTxt.Add(new List<int>());
				listaSecretosTxt.Add(new List<int>());

				StreamReader streamreader = new StreamReader ("Assets/datos" + i + ".txt");

				String linea = "";
				bool controlCadaArchivo = true; //sirve para que solo intente agrandar la lista de matrices por nivel solo una vez cada que lee un archivo txt

				for (int j = 0; j < 9; j++) {
					
					linea = streamreader.ReadLine ();
					string[] splitString = linea.Split (new string[] { ";" }, StringSplitOptions.None);
					int difDeNiveles = Math.Abs (nivelMasAltoEnTxt - splitString.Length);

					if (splitString.Length > nivelMasAltoEnTxt  & controlCadaArchivo) {
						nivelMasAltoEnTxt = splitString.Length;
						//Debug.Log ("Entro a cambiar el valor del nivel mas alto con: " + nivelMasAltoEnTxt);
						controlCadaArchivo = false;
						for (int k = 0; k < difDeNiveles; k++) {
							matrizPorNiveles.Add (new List<string> ()); //Se crea una nueva lista para cada nivel.
							matrizPorNivelesTipo.Add( new List<string>());
						}
					}

					//Debug.Log ("El experimento salio para tener tamaño: " + matrizPorNiveles.Count);


					IngresarDatosAMatrices (splitString, j, i, leyoPrimeraLinea);
					leyoPrimeraLinea = true;
				}


				streamreader.Close ();

			}
		}
		//revisarMatricestxt ();
	}

	public void IngresarDatosAMatrices(string[] splitString, int matrizElegida, int tipo, bool yaLeyoPrimeraLinea){


		for (int i = 0; i < splitString.Length; i++) {

			if (matrizElegida == 0) {
				//Debug.Log ("Pasos jugador" + splitString [i]);
				pasosJugadorTxt [pasosJugadorTxt.Count - 1].Add (int.Parse (splitString [i]));
				matrizPorNiveles [i].Add (splitString [i]);
			} else if (matrizElegida == 1) {
				//Debug.Log ("vida jugador" + splitString [i]);
				vidaJugadorTxt [vidaJugadorTxt.Count - 1].Add (int.Parse (splitString [i]));
				matrizPorNiveles [i].Add (splitString [i]);
			} else if (matrizElegida == 2) {
				//Debug.Log ("jugador muerto" + splitString [i]);
				jugadorMuertoTxt.Add (int.Parse (splitString [i]));
				//matrizPorNiveles [i].Add (splitString [i]);
			} else if (matrizElegida == 3) {
				//Debug.Log ("posicion muerte jugador" + splitString [i]);
				posicionMuerteJugadorTxt [vidaJugadorTxt.Count - 1].Add (int.Parse (splitString [i]));
				//matrizPorNiveles [i].Add (splitString [i]);
			} else if (matrizElegida == 4) {
				//Debug.Log ("Lista enemigos" + splitString [i]);
				listaEnemigosTxt [listaEnemigosTxt.Count - 1].Add (splitString [i]);
				//matrizPorNiveles [i].Add (splitString [i]);
			} else if (matrizElegida == 5) {
				//Debug.Log ("Lista golpes enemigo" + splitString [i]);
				listaGolpesEnemigosTxt [listaGolpesEnemigosTxt.Count - 1].Add (splitString [i]);
				//matrizPorNiveles [i].Add (splitString [i]);
			} else if (matrizElegida == 6) {
				//Debug.Log ("Lista puntaje" + splitString [i]);
				listaPuntajeTxt [listaPuntajeTxt.Count - 1].Add (int.Parse (splitString [i]));
				matrizPorNiveles [i].Add (splitString [i]);
			}else if (matrizElegida == 7 ) {
				//Debug.Log ("Lista muertes" + splitString [i]);
				listaMuertesTxt [listaMuertesTxt.Count - 1].Add (int.Parse (splitString [i]));
				matrizPorNiveles [i].Add (splitString [i]);
			}else if (matrizElegida == 8 ) {
				//Debug.Log ("Lista secretos" + splitString [i]);
				listaSecretosTxt [listaSecretosTxt.Count - 1].Add (int.Parse (splitString [i]));
				matrizPorNiveles [i].Add (splitString [i]);
			}
				
			if (yaLeyoPrimeraLinea == false) {
				//Debug.Log ("guarda solo 3 veces y no mas");
				matrizPorNivelesTipo[i].Add ("" + tipo);
			}

		}
	}

	public void revisarMatricestxt(){
		Debug.Log ("---------------------------------------------");
		//Debug.Log ("Pasos jugador");
		for (int i = 0; i < pasosJugadorTxt.Count; i++) {
			//Debug.Log ("Archivo n°:" + (i+1));
			for (int j = 0; j<pasosJugadorTxt [i].Count; j++) {
				//Debug.Log (pasosJugadorTxt [i][j]);
			}
		}
		//Debug.Log ("vida jugador");
		for (int i = 0; i < vidaJugadorTxt.Count; i++) {
			//Debug.Log ("Archivo n°:" + (i+1));
			for (int j = 0; j<vidaJugadorTxt [i].Count; j++) {
				//Debug.Log (vidaJugadorTxt [i][j]);
			}
		}
		//Debug.Log ("jugador muerto");
		for (int i = 0; i < jugadorMuertoTxt.Count; i++) {
			//Debug.Log ("Archivo n°:" + (i+1));
			//Debug.Log (jugadorMuertoTxt[i]);
		}
		//Debug.Log ("posicion muerte jugador");
		for (int i = 0; i < posicionMuerteJugadorTxt.Count; i++) {
			//Debug.Log ("Archivo n°:" + (i+1));
			for (int j = 0; j<posicionMuerteJugadorTxt [i].Count; j++) {
				//Debug.Log(posicionMuerteJugadorTxt [i] [j]);
			}
		}
		//Debug.Log ("listado enemigos");
		for (int i = 0; i < listaEnemigosTxt.Count; i++) {
			//Debug.Log ("Archivo n°:" + (i+1));
			for (int j = 0; j<listaEnemigosTxt [i].Count; j++) {
				//Debug.Log (listaEnemigosTxt [i][j]);
			}
		}
		//Debug.Log ("listado golpes enemigos");
		for (int i = 0; i < listaGolpesEnemigosTxt.Count; i++) {
			//Debug.Log ("Archivo n°:" + (i+1));
			for (int j = 0; j<listaGolpesEnemigosTxt [i].Count; j++) {
				//Debug.Log (listaGolpesEnemigosTxt [i][j]);
			}
		}
		Debug.Log ("listado por niveles");
		int tipo = 0;
		for (int i = 0; i < matrizPorNiveles.Count; i++) {
			Debug.Log ("nivel: " + (i+1));
			for (int j = 0; j<matrizPorNiveles [i].Count; j++) {
				Debug.Log ("J es: " + j);
				Debug.Log (matrizPorNiveles [i][j]);
				if (i == 1) {
					if (j == 7 || j == 15 || j == 23) {

						Debug.Log ("Es de tipo = " + matrizPorNivelesTipo [i] [tipo]);
						tipo++;
					}
				} else if (i > 1) {
					if (j == 6 || j == 13 || j == 20) {

						Debug.Log ("Es de tipo = " + matrizPorNivelesTipo [i] [tipo]);
						tipo++;
					}
				}else{
					if (j == 8 || j == 17 || j == 26) {

						Debug.Log ("Es de tipo = " + matrizPorNivelesTipo [i] [tipo]);
						tipo++;
					}

				}
				if (tipo == 3) {
					tipo = 0;
				}

			}
		}
		/*
		Debug.Log ("puntaje");
		for (int i = 0; i < listaPuntajeTxt.Count; i++) {
			Debug.Log ("Archivo n°:" + (i + 1));
			for (int j = 0; j < listaPuntajeTxt [i].Count; j++) {
				Debug.Log (listaPuntajeTxt [i] [j]);
			}
		}
		Debug.Log ("Muertes:");
		for (int i = 0; i < listaMuertesTxt.Count; i++) {
			Debug.Log ("Archivo n°:" + (i + 1));
			for (int j = 0; j < listaMuertesTxt [i].Count; j++) {
				Debug.Log (listaMuertesTxt [i] [j]);
			}
		}
		Debug.Log ("secretos:");
		for (int i = 0; i < listaPuntajeTxt.Count; i++) {
			Debug.Log ("Archivo n°:" + (i + 1));
			for (int j = 0; j < listaPuntajeTxt [i].Count; j++) {
				Debug.Log (listaPuntajeTxt [i] [j]);
			}
		}*/
	}

}
