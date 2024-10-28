/** d) Modifique la solución (b) considerando que tanto el director como cada administrativo
imprimen 10 trabajos y que todos los procesos deben terminar su ejecución.
Nota: ni los administrativos ni el director deben esperar a que se imprima el documento. **/

chan pedidos(texto); //Administrativos envían documentos a esta cola de impresión
chan pedidosConPrioridad(texto); //Director envía sus documentos a esta cola prioritaria
chan colaImpresora[3](texto); //Cola privada de cada Impresora
chan Signal; //Administrativos y director avisan que necesitan usar la impresora

Process Administrativo[id:0..N-1] {
    int i; texto doc;
    for i in 1..10 { //Imprime 10 documentos
        doc = generarDoc();
        send(pedido(doc)); //Envía documento a imprimir
        send Signal(); //Avisa que necesita imprimir
    }
}

Process Director {
    int i; texto doc;
    for i in 1..10 { //Imprime 10 documentos
        doc = generarDoc();
        send(pedido(doc)); //Envía documento a imprimir
        send Signal(); //Avisa que necesita imprimir
    }
}

Process Impresora[id:0..2] {
    texto doc; boolean continuar = true;
    while(continuar) {
        receive(colaImpresora[id](doc)); //Espera a que el Coordinador le envíe un documento a imprimir
        if(doc <> "FIN") -> imprimirDocumento(doc); //Si es válido lo imprime, sino corta la ejecución del proceso
        [] (doc.equals("FIN")) continuar = false;
    }
}

Process Coordinador {
    int i, id; texto doc; int contador = 0;

    while (contador < N*10+10) {
        receive Signal(); //Espera a que alguien necesite una impresora. Va a entrar si o si al if-[]
        
        if(not empty(pedidosConPrioridad)) -> //Si hay pedidos con prioridad
            receive(pedidoConPrioridad(doc)); //Recibe el pedido prioritario
        [] (empty(pedidosConPrioridad) and (not empty(pedidos)))-> //Si no tengo pedidos con prioridad pero tengo pedidos normales
            receive(pedido(doc)); //Recibe el pedido normal
            //Envía pedido a alguna impresora (aleatoria entre las disponibles)
            if(empty(colaImpresora[0])) -> send(colaImpresora[0](doc));
            [] if(empty(colaImpresora[1])) -> send(colaImpresora[1](doc));
            [] if(empty(colaImpresora[2])) -> send(colaImpresora[2](doc));
            end if
        endif

        contador++;
    }

    for i in 0..2 {
        send(colaImpresora[i]("FIN"));
    }
}
