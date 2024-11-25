/* 3) Implemente una solución para el siguiente problema. Se debe simular el uso de una máquina
expendedora de gaseosas con capacidad para 100 latas por parte de U usuarios. Además, existe un
repositor encargado de reponer las latas de la máquina. Los usuarios usan la máquina según el orden
de llegada. Cuando les toca usarla, sacan una lata y luego se retiran. En el caso de que la máquina se
quede sin latas, entonces le debe avisar al repositor para que cargue nuevamente la máquina en forma
completa. Luego de la recarga, saca una botella y se retira. Nota: maximizar la concurrencia; mientras
se reponen las latas se debe permitir que otros usuarios puedan agregarse a la fila. */

boolean libre = true;
Cola colaUsuarios[U];
Cola latas_disponibles = inicializarLatas(100);
sem mutex = 1;
sem mutex_latas = 1;
sem espera[U] = ([U] 0)
sem avisar_repositor = 0;
sem esperar_reposicion = 0;

Process Usuario[id:0..U-1] {
    Lata lata;

    P(mutex);
    if(!libre) {
        colaUsuarios.push(id);
        V(mutex);
        P(espera[id]);
    }
    else libre = false; V(mutex);

    P(mutex_latas);
    if(latas_disponibles.isEmpty()) {
        V(avisar_repositor);  //Despierta al repositor
        V(mutex_latas);
        P(esperar_reposicion);  //Espera a que se recargue la maquina
    }
    
    lata = latas_disponibles.pop(); //Saca una lata de la naquina

    // Antes de irse tiene que hacer passing the button

    P(mutex);
    if(colaUsuarios.isEmpty()){  //Si no hay nadie esperando
        libre = true;
    }
    else {
        int siguiente = colaUsuarios.pop();
        V(espera[siguiente]);
    }
    V(mutex);
}

Process Repositor {
    while(true) {
        P(avisar_repositor);
        P(mutex_latas);
        latas_disponibles.push(100);
        V(esperar_reposicion);
        V(mutex_latas)
    }
}
