/* En un estadio de fútbol hay una máquina expendedora de gaseosas que debe ser usada por E Espectadores
de acuerdo con el orden de llegada. Cuando el espectador accede a la máquina en su turno usa la máquina
y luego se retira para dejar al siguiente.

Nota: cada Espectador una sólo una vez la máquina. */

Process Espectador [id:0..E-1] {
    Admin!llegada(id); //Avisa que llegó
    Admin?usarMaquina(); //Espera a que le avisen que es su turno de usar la máquina
    usandoMaquinaExpendedora();
    Admin!fin(); //Avisa que terminó de usar la máquina
}

Process Admin {
    int i, idE, cantP = 0; cola buffer; boolean maquinaLibre = true;

    do (cantP < E); Espectador[*]?llegada(idE) -> //Espera para recibir el id de un nuevo espectador
        if(!maquinaLibre) -> //Si la máquina no está disponible para usar
            push(buffer, idE); //Agrega el id del espectador a la fila
        [] (maquinaLibre) -> //Si la máquina está disponible para usar
            maquinaLibre = false; //Actualiza el estado de la máquina a "ocupada"
            Espectador[idE]!usarMaquina(); //Avisa al espectador que es su turno
            cantP++;
        end if
    [] Espectador[*]?fin() -> //Si alguien avisa que terminó de usar la máquina
        if(empty (buffer)) -> //Si no hay más espectadores en la fila
            maquinaLibre = true; //Actualiza el estado de la máquina a "disponible"
        [] (not empty(buffer)) -> //Si quedan espectadores en la fila, tiene que pasar el que está primero
            Espectador[pop(buffer)]!usarMaquina(); //Avisa al espectador que se recuperó de la cola que es su turno
        end if
    end if
}

// PREGUNTAR SI PUEDO USAR 3 PROCESOS (ESPECTADOR-ADMIN-MAQUINA) --> Si se permite lo puedo resolver de la misma forma de siempre
// Hecho con 3 procesos:

Process Espectador [id:0..E-1] {
    Admin!llegada(id); //Avisa que llegó
    Maquina?usarMaquina(); //Espera a que le avisen que es su turno de usar la máquina
    usandoMaquinaExpendedora();
    Maquina!fin(); //Avisa que terminó de usar la máquina
}

Process Admin {
    int idE; cola buffer;
    do Espectador[*]?llegada(idE) -> //Recibe un espectador
        push(buffer, id); //Agrega el id a la fila
    [] not empty(buffer) ; Maquina?disponible -> //Si hay espectadores en la fila y la máquina está disponible
        Maquina!siguiente(pop(buffer)); //Envía a la máquina quién es el siguiente en la fila
}

Process Maquina {
    int i, idPersona;
    for i in 1..E {
        Admin!disponible(); //Avisa que está disponible
        Admin?siguiente(idPersona); //Espera a saber quién sigue
        Espectador[idPersona]!usarMaquina(); //Avisa a una persona que es su turno de usar la máquina
        Espectador[idPersona]?fin(); //Espera a que la persona termine de usar la máquina
    }
}
