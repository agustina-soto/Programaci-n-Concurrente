/* 1) Resolver el siguiente problema. En una elección estudiantil, se utiliza una máquina para voto
electrónico. Existen N Personas que votan y una Autoridad de Mesa que les da acceso a la máquina
de acuerdo con el orden de llegada, aunque ancianos y embarazadas tienen prioridad sobre el resto.
La máquina de voto sólo puede ser usada por una persona a la vez. Nota: la función Votar() permite
usar la máquina. */

Process Persona[id:0..N-1] {
    boolean tiene_prioridad = getTienePrioridad();
    MaquinaElectronica.llegada(id, tiene_prioridad);
    usarMaquina();
    MaquinaElectronica.salida();
}

Process Autoridad de Mesa {
    for int i in 1 to N {
        MaquinaElectronica.darAcceso();
    }
}


Monitor MaquinaElectronica {
    int cant = 0;
    cond autoridad; //Si no hay nadie queriedo votar la autoridad se duerme
    cond espera[N];  //Los procesos se van a dormir cuando no puedan acceder a la maquina por orden de llegada
    Cola colaOrdenada;  //Los procesos se encolan de forma ordenada segun su prioridad

    /* La autoridad de mesa se duerme hasta que el proceso que esta usando la maquina termine de usarla.
    Cuando termina puede darle la maquina a la persona que sigue o dejarla libre para que otro la use */
    cond fin_uso;


    Procedure llegada(idP: IN int, tiene_prioridad: IN boolean) {
        insertar(colaOrdenada(idP, tiene_prioridad)); //Ordena por prioridad
        cant++;
        signal(autoridad);  //Avisa que hay alguien queriendo votar
        wait(espera[idP]);  //Duerme el proceso para encolarlo en la cola por orden de llegada
    }

    Procedure darAcceso() {
        int siguiente;
        
        if(cant == 0) wait(autoridad);  //Si no hay nadie queriendo usar la maquina, la autoridad de mesa se duerme
        
        //Hay alguien queriendo votar

        cant--;  //Decrementa cantidad de gente esperando pasar
        sacar(colaOrdenada, siguiente);  //Toma el id de la persona que sigue
        signal(espera[siguiente]);  //Despierta solamente la persona a la que le toca usar la maquina
        wait(fin_uso);  //Espera a que el proceso persona termine de usar la maquina
    }

    Procedure salida() {
        signal(fin_uso);  //Avisa que termino de usar la maquina
    }
}
