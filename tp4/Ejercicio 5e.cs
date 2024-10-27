/** e) Si la solución al ítem d) implica realizar Busy Waiting, modifíquela para evitarlo.
Nota: ni los administrativos ni el director deben esperar a que se imprima el documento. **/

chan colaImpresion(texto); //Administrativos envían documentos a esta cola de impresión
chan colaImpresionPrioritaria(texto); //Director envía sus documentos a esta cola prioritaria

Process Administrativo[id:0..N-1] {
    texto doc; int cant_docs = 0;

    while(cant_docs < 10) {
        doc = generarDoc();
        send(colaImpresion(doc)); //Envía documento a imprimir
        send Signal(); //Envía aviso de que hay un documento a imprimir
        cant_docs++; //Incrementa su cantidad de documentos enviados
    }
}

Process Director {
    texto doc; int cant_docs = 0;
    while(cant_docs < 10) {
        doc = generarDoc();
        send(colaImpresionPrioritaria(doc));
        send Signal(); //Envía aviso de que hay un documento a imprimir
        cant_docs++; //Incrementa su cantidad de documentos enviados
    }
}

Process Impresora[id:0..2] {
    texto doc;

    //Se despierta cuando un alguien le avisa que hay un documento a imprimir (va a imprimir si o si, por la cola normal o por la prioritaria)
    receive Signal(); // Recibe N*10 + 10 avisos para despertarse

    if(empty(colaImpresionPrioritaria)) ->
        // colaImpresion tiene un documento si o si porque no lo despertó el director
        receive(colaImpresion(doc));
    [] (not empty(colaImpresionPrioritaria)) ->
        receive(colaImpresionPrioritaria(doc));
    imprimirDocumento(doc); //Imprime
}
