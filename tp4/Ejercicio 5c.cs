/** c) Modifique la solución (a) considerando que cada administrativo imprime 10 trabajos y
que todos los procesos deben terminar su ejecución.
Nota: ni los administrativos ni el director deben esperar a que se imprima el documento. **/

chan pedido(texto); //Administrativos envían documentos a esta cola de impresión
chan colaImpresora[3](texto); //Cola privada de cada Impresora

Process Administrativo[id:0..N-1] {
    texto doc; int limite = 10;
    for i in 1..limite { //Imprime 10 documentos
        doc = generarDoc();
        send(pedido(doc)); //Envía documento a imprimir
    }
}

Process Impresora[id:0..2] {
    texto doc; boolean continuar = true;
    while(continuar) {
        receive(pedido(doc)); //Espera a que haya un documento a imprimir
        if(doc <> "FIN") imprimirDocumento(doc); //Si es válido lo imprime, sino corta la ejecución del proceso
        else continuar = false;
    }
}

Process Coordinador {
    int i, id; texto doc; int contador = 0;

    while (contador < N*10) {
        receive(pedido(doc)); //Recibe el pedido

        //Envía pedido a alguna impresora (aleatoria entre las disponibles)
        if(empty(colaImpresora[0])) -> send(colaImpresora[0](doc));
        [] (empty(colaImpresora[1])) -> send(colaImpresora[1](doc));
        [] (empty(colaImpresora[2])) -> send(colaImpresora[2](doc));
        end if

        contador++;
    }

    for i in 0..2 {
        send(colaImpresora[i]("FIN"));
    }
}
