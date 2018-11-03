# -*- coding: utf-8 -*-
"""
Created on Tue Oct  9 11:32:30 2018

@author: Daniel Ramirez
"""

import numpy as np
import pandas as pd
import threading, time
from matplotlib import pyplot as plt
from sklearn.neural_network import MLPClassifier
from sklearn.model_selection import train_test_split
#from sklearn.metrics import classification_report, confusion_matrix


ejecuciones = open("ejecuciones.csv","w")
dataIn = pd.read_csv('datos0.csv',header=None) #LA BD


dato = dataIn.transpose()
tamaño = dato.shape
df_x = dato.iloc[:,:(tamaño[1]-1)]
df_y = dato.iloc[:,(tamaño[1]-1)]


x_train, x_test, y_train, y_test = train_test_split(df_x, df_y, test_size=0.2) 
nn=MLPClassifier(activation='logistic',solver='lbfgs',hidden_layer_sizes=(10,8,8))
nn.fit(x_train,y_train)


pred=nn.predict(x_test)
a=y_test.values
b=len(a)
count=0
for i in range(len(pred)):
    if pred[i] == a[i]:
        count=count+1
porcentaje = count/b

linea = str(porcentaje) + "\n"
ejecuciones.write(linea)
ejecuciones.close()

#print(confusion_matrix(y_test,pred))  
#print(classification_report(y_test,pred))  
def tipo_jugador():
    nivel_a_ejecutar = 6;
    while(True):
        nivel = open('datos.csv')
        nivel = nivel.readline()
        print(nivel)
        niveles = pd.read_csv('datos.csv',header=None, skiprows=1)
        niveles = niveles.transpose()
        if int(nivel) == nivel_a_ejecutar:
            print("encontrado")
            prediciones = nn.predict(niveles)
            print(prediciones)
            nivel_a_ejecutar+=3
        time.sleep(10)

        
hilo = threading.Thread(target=tipo_jugador)
hilo.start()






