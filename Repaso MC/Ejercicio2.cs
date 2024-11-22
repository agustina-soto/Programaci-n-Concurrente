/* Implemente una solución para el siguiente problema. Un sistema debe validar un conjunto de 10000
transacciones que se encuentran disponibles en una estructura de datos. Para ello, el sistema dispone
de 7 workers, los cuales trabajan colaborativamente validando de a 1 transacción por vez cada uno.
Cada validación puede tomar un tiempo diferente y para realizarla los workers disponen de la
función Validar(t), la cual retorna como resultado un número entero entre 0 al 9. Al finalizar el
procesamiento, el último worker en terminar debe informar la cantidad de transacciones por cada
resultado de la función de validación. Nota: maximizar la concurrencia. */


Cola transacciones[10000] = cargarTransacciones();
int contador[10] = ([0..9] 0);
int cantT = 0;  //Contador de transacciones validadas
sem mutex = 1, mutex_cont[10] = ([10] 1);

Process Worker [id:0..6] {
    P(mutex);
    while(cantT < 10000) {
        Transaccion t = transacciones.pop();  //Toma la primer transaccion
        cantT++;
        V(mutex);
        int nro = Validar(t); //Devuelve un numero entre el 0 y el 9
        P(mutex_cont[nro]);
        contador[nro]++;
        V(mutex_cont[nro]);
        P(mutex);
        if(cantT == 10000) ->
            for int i in 1 to 10 print("Cantidad de transacciones por resultado de la funcion = + i + ": " + contador[i]);
    }
    V(mutex);
}
