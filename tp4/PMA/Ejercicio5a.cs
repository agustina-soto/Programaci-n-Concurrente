/** Resolver la administración de 3 impresoras de una oficina. Las impresoras son usadas por N
administrativos, los cuales están continuamente trabajando y cada tanto envían documentos
a imprimir. Cada impresora, cuando está libre, toma un documento y lo imprime, de
acuerdo con el orden de llegada.
a) Implemente una solución para el problema descrito.
Nota: ni los administrativos ni el director deben esperar a que se imprima el documento. **/


chan colaImpresion(texto); //Administrativos envían documentos a esta cola de impresión

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
        receive(colaImpresion(doc)); //Espera a que haya un documento a imprimir
        imprimirDocumento(doc); //Imprime
    }
}
