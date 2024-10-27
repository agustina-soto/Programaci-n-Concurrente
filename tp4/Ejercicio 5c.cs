/** c) Modifique la solución (a) considerando que cada administrativo imprime 10 trabajos y
que todos los procesos deben terminar su ejecución.
Nota: ni los administrativos ni el director deben esperar a que se imprima el documento. **/

chan colaImpresion(texto); //Administrativos envían documentos a esta cola de impresión

Process Administrativo[id:0..N-1] {
    texto doc; int cant_docs = 0;
    while(cant_docs < 10) { //Imprime 10 documentos
        doc = generarDoc();
        send(colaImpresion(doc)); //Envía documento a imprimir
    }
}

Process Impresora[id:0..2] {
    texto doc; int cant_impresiones = 0, limite = N*10;
    while(cant_impresiones < limite) {
        receive(colaImpresion(doc)); //Espera a que haya un documento a imprimir
        imprimirDocumento(doc); //Imprime
        cant_impresiones++; //Incrementa cantidad de documentos impresos
    }
}
