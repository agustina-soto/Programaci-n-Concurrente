/** d) Modifique la solución (b) considerando que tanto el director como cada administrativo
imprimen 10 trabajos y que todos los procesos deben terminar su ejecución.
Nota: ni los administrativos ni el director deben esperar a que se imprima el documento. **/

chan colaImpresion(texto); //Administrativos envían documentos a esta cola de impresión
chan colaImpresionPrioritaria(texto); //Director envía sus documentos a esta cola prioritaria

Process Administrativo[id:0..N-1] {
    texto doc; int cant_docs = 0;
    while(cant_docs < 10) {
        doc = generarDoc();
        send(colaImpresion(doc)); //Envía documento a imprimir
        cant_docs++;
    }
}

Process Impresora[id:0..2] {
    texto doc; int cant_impresiones = 0, limite = N*10;
    while(cant_impresiones < limite) {
        //BW
        if(empty(colaImpresionPrioritaria)) ->
            if(not empty(colaImpresion)) {
                receive(colaImpresion(doc)); //Si hay un doc en la cola no prioritaria, lo recibe
                imprimirDocumento(doc); //Imprime
                cant_impresiones++; //Incrementa cantidad de documentos impresos
            }
        [] (not empty(colaImpresionPrioritaria)) ->
            receive(colaImpresionPrioritaria(doc)); //Si hay un doc en la cola prioritaria, lo recibe
            imprimirDocumento(doc); //Imprime
            cant_impresiones++; //Incrementa cantidad de documentos impresos
    }
}

Process Director {
    texto doc; int cant_docs = 0;
    while(cant_docs < 10) {
        doc = generarDoc();
        send(colaImpresionPrioritaria(doc));
        cant_docs++;
    }
}
