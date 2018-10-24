# -*- coding: utf-8 -*-
"""
Created on Tue Oct  9 11:32:30 2018

@author: USER
"""

import numpy as np
import pandas as pd 
from matplotlib import pyplot as plt
from sklearn.neural_network import MLPClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report, confusion_matrix

ejecuciones = open("ejecuciones.csv","w")
mejoresPesos = open("pesos.csv","w")
dataIn = pd.read_csv('datos0.csv',header=None)

dato = dataIn.transpose()
tamaño = dato.shape
df_x = dato.iloc[:,:(tamaño[1]-1)]
df_y = dato.iloc[:,(tamaño[1]-1)]

mejorPorcentaje = 0
listapesos = 0
for i in range(10):
    x_train, x_test, y_train, y_test = train_test_split(df_x, df_y, test_size=0.2)
    nn=MLPClassifier(activation='logistic',solver='lbfgs',hidden_layer_sizes=(10,8,8))
    nn.fit(x_train,y_train)

    apesos = nn.coefs_
    aclases = nn.classes_
    aiteraciones = nn.n_iter_
    acapas = nn.n_layers_
    asalidas = nn.n_outputs_

    pred=nn.predict(x_test)
    a=y_test.values
    b=len(a)
    count=0
    for i in range(len(pred)):
        if pred[i] == a[i]:
            count=count+1
    porcentaje = count/b
    if(porcentaje > mejorPorcentaje): 
        listapesos = nn.coefs_
    
    linea = str(porcentaje) + "\n"
    ejecuciones.write(linea)
lineaPesos = ""
for i in range(len(listapesos)): #es una lista de listas
    for j in range(listapesos[i].shape[1]): #aqui ya tengo una lista
        for k in range(listapesos[i].shape[0]):#aqui obtengo la primera lista de cada capa
            lineaPesos += str(listapesos[i][k,j])
            if((k+1) != listapesos[i].shape[0]):
                lineaPesos += "; "
        lineaPesos += "\n"
        mejoresPesos.write(lineaPesos)
        lineaPesos = ""
            
ejecuciones.close()
mejoresPesos.close()
#print(confusion_matrix(y_test,pred))  
#print(classification_report(y_test,pred))  