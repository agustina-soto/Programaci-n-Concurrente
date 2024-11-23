/* Resolver los problemas siguientes:
a) En una estación de trenes, asisten P personas que deben realizar una carga de su tarjeta SUBE
en la terminal disponible. La terminal es utilizada en forma exclusiva por cada persona de acuerdo
con el orden de llegada. Implemente una solución utilizando únicamente procesos Persona. Nota:
la función UsarTerminal() le permite cargar la SUBE en la terminal disponible.
b) Resuelva el mismo problema anterior pero ahora considerando que hay T terminales disponibles.
Las personas realizan una única fila y la carga la realizan en la primera terminal que se libera.
Recuerde que sólo debe emplear procesos Persona. Nota: la función UsarTerminal(t) le permite
cargar la SUBE en la terminal t. */

a)
Cola colaEspera[P];
sem mutex = 1;
sem espera_turno[P] = ([P] 0);
boolean libre = true;

Process Persona [id:0..P-1] {
    P(mutex);
    if(libre) -> libre = false; V(mutex);
    [] if(!libre) ->
        colaEspera.push(id);
        V(mutex);
        P(espera_turno[id]);
    end if;
    UsarTerminal();
    P(mutex);
    if(colaEspera.isEmpty()) -> libre = true;
    [] if(!colaEspera.isEmpty()) ->
        V(espera_turno[colaEspera.pop()]);
    end if;
    V(mutex);
}


b)
Cola colaEspera[P];
Cola colaTerminales[T] = (0, 1, 2, ..., T-1);
sem mutex = 1;
sem espera_turno[P] = ([P] 0);
int t_libre = T;

Process Persona [id:0..P-1] {
    P(mutex);
    if(t_libre > 0) -> libres--; V(mutex);  //Si hay terminales libres
    [] if(t_libre == 0) ->  //Si no hay terminales libres
        colaEspera.push(id);
        V(mutex);
        P(espera_turno[id]);
    end if;

    P(mutex_terminales);
    Terminal t = colaTerminales.pop();
    V(mutex_terminales);
    UsarTerminal(t);
    P(mutex_terminales);
    colaTerminales.push(t);
    V(mutex_terminales);

    P(mutex);
    if(colaEspera.isEmpty()) -> t_libre++;
    [] if(!colaEspera.isEmpty()) -> V(espera_turno[colaEspera.pop()]);
    V(mutex);
}
