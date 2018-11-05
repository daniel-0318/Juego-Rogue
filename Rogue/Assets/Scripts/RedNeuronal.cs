using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class RedNeuronal {



	public void ExecuteCommand (){
		
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
