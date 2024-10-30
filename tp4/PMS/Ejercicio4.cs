/** En una exposición aeronáutica hay un simulador de vuelo (que debe ser usado con exclusión mutua)
y un empleado encargado de administrar su uso. Hay P personas que esperan a que el empleado lo deje
acceder al simulador, lo usa por un rato y se retira.
a) Implemente una solución donde el empleado sólo se ocupa de garantizar la exclusión mutua.
b) Modifique la solución anterior para que el empleado considere el orden de llegada para dar acceso al simulador.

Nota: cada persona usa sólo una vez el simulador. **/

a) Sin considerar orden de llegada.

Process Empleado {
    int i, idPersona;
    for i in 0..P-1 {
        Persona[*]?llegada(idPersona); //Espera a que llegue una persona
        Persona[idPersona]!accesoSimulador(); //Permite acceso al simulador
        Persona[idPersona]?fin(); //Espera a que la persona idPersona termine de usar el simulador
    }
}

Process Persona [id:0..P-1]{
    Empleado!llegada(id); //Avisa que quiere usar el simulador
    Empleado?accesoSimulador(); //Espera a que el empleado le de acceso al simulador
    usarSimulador();
    Empleado!fin(); //Avisa al empleado que terminó de usar el simulador
}


b) Considerando el orden de llegada.

Process Empleado {
    int i, idPersona;
    for i in 0..P-1 {
        Admin!disponible(); //Avisa al admin que se encuentra disponible
        Admin?siguienteTurno(idPersona); //Espera a que admin le diga quién es la persona que sigue
        Persona[idPersona]!accesoSimulador(); //Permite acceso al simulador a una persona
        Persona[idPersona]?fin(); //Espera a que la persona termine de usar el simulador
    }
}

Process Persona [id:0..P-1]{
    Admin!llegada(id); //Avisa que quiere usar el simulador
    Empleado?accesoSimulador(); //Espera a que el empleado le de acceso al simulador
    usarSimulador();
    Empleado!fin(); //Avisa al empleado que terminó de usar el simulador
}

Process Admin {
    int idPersona, cola Buffer;
    do Persona[*]?llegada(idPersona) -> //Espera a que llegue una persona
        push(Buffer(idPersona)); //Agrega idPersona a la fila de espera
    [] not empty(Buffer) ; Empleado?disponible -> //Si hay personas esperando y el empleado está disponible
        Empleado!siguienteTurno(pop(Buffer)); //Envía al empleado el primer id del Buffer
    od
}
