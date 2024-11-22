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
