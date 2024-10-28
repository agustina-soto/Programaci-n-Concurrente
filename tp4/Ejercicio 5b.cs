/** b) Modifique la solución implementada para que considere la presencia de un director de
oficina que también usa las impresoras, el cual tiene prioridad sobre los administrativos.

Nota: ni los administrativos ni el director deben esperar a que se imprima el documento. **/

chan colaImpresion(texto); //Administrativos envían documentos a esta cola de impresión
chan colaImpresionPrioritaria(texto); //Director envía sus documentos a esta cola prioritaria

Process Administrativo[id:0..N-1] {
    texto doc;
    while(true) {
        doc = generarDoc();
        send(colaImpresion(doc)); //Envía documento a imprimir
    }
}

Process Impresora[id:0..2] {
    texto doc;
    while(true) {
        if(empty(colaImpresionPrioritaria) and (not empty(colaImpresion))) ->
                receive(colaImpresion(doc)); //Si hay un doc en la cola no prioritaria, lo recibe
                imprimirDocumento(doc); //Imprime
            }
        [] (not empty(colaImpresionPrioritaria)) ->
            receive(colaImpresionPrioritaria(doc)); //Si hay un doc en la cola prioritaria, lo recibe
            imprimirDocumento(doc); //Imprime
    }
}

Process Director {
    texto doc;
    while(true) {
        doc = generarDoc();
        send(colaImpresionPrioritaria(doc));
    }
}
