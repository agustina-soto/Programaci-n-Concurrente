/* En un estadio de fútbol hay una máquina expendedora de gaseosas que debe ser usada por E Espectadores
de acuerdo con el orden de llegada. Cuando el espectador accede a la máquina en su turno usa la máquina
y luego se retira para dejar al siguiente.

Nota: cada Espectador una sólo una vez la máquina. */

Process Espectadores [id:0..E-1] {
    Admin!llegada(id); //Avisa que llegó
    Admin?miTurno(); //Espera a que le avisen que es su turno de usar la máquina
    usarMaquina();
    Admin!fin(); //Avisa que terminó de usar la máquina
}

Process Buffer {
    int i, idE, cantP = 0; cola buffer; boolean maquinaLibre = true;

    do (cantP < E); Espectador[*]?llegada(idE) -> //Espera para recibir el id de un nuevo espectador
        if(!maquinaLibre) -> //Si la máquina no está disponible para usar
            push(buffer(idE)); //Agrega el id del espectador a la fila
        [] (maquinaLibre) -> //Si la máquina está disponible para usar
            maquinaLibre = false; //Actualiza el estado de la máquina a "ocupada"
            Espectador[idE]!miTurno(); //Avisa al espectador que es su turno
            cantP++;
        end if
    [] Espectador[*]?fin() -> //Si alguien avisa que terminó de usar la máquina
        if(empty (buffer)) -> //Si no hay más espectadores en la fila
            maquinaLibre = true; //Actualiza el estado de la máquina a "disponible"
        [] (not empty(buffer)) -> //Si quedan espectadores en la fila, tiene que pasar el que está primero
            Espectador[pop(buffer)]!miTurno(); //Avisa al espectador que se recuperó de la cola que es su turno
        end if
    end if
}
